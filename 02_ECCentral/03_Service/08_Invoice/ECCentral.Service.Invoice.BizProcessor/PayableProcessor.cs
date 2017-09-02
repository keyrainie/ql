using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.PO;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Invoice.BizProcessor.ETPCalculator;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Invoice.BizProcessor.PayItemProcess;

namespace ECCentral.Service.Invoice.BizProcessor
{
    [VersionExport(typeof(PayableProcessor))]
    public class PayableProcessor
    {
        private IPayableDA m_PayableDA = ObjectFactory<IPayableDA>.Instance;

        private IFinanceDA m_FinaceDA = ObjectFactory<IFinanceDA>.Instance;

        /// <summary>
        /// 创建应付款
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual PayableInfo Create(PayableInfo entity)
        {
            if (!entity.OrderSysNo.HasValue || entity.OrderSysNo == 0)
            {
                throw new ArgumentException("entity.OrderSysNo");
            }

            if (!entity.BatchNumber.HasValue ||
                (entity.BatchNumber.HasValue && entity.BatchNumber == 0))
            {
                entity.BatchNumber = 1;
            }

            switch (entity.OrderType)
            {
                case PayableOrderType.PO:
                    entity.EIMSNo = ExternalDomainBroker.GetPurchaseOrderReturnPointSysNo(entity.OrderSysNo.Value);
                    break;
                case PayableOrderType.VendorSettleOrder:
                    entity.EIMSNo = ExternalDomainBroker.GetConsignSettlementReturnPointSysNo(entity.OrderSysNo.Value);
                    break;               
                default:
                    entity.EIMSNo = null;
                    break;
            }
            switch (entity.OrderType)
            {
                case PayableOrderType.POAdjust:               
                case PayableOrderType.RMAPOR:
                case PayableOrderType.CollectionSettlement:
                case PayableOrderType.CollectionPayment:
                case PayableOrderType.Commission:
                case PayableOrderType.CostChange:
                    entity.EstimatedTimeOfPay = DateTime.Now;
                    break;
                default:
                    entity.EstimatedTimeOfPay = null;
                    break;
            }

            return m_PayableDA.Create(entity);
        }

        /// <summary>
        /// 更新付款单发票信息
        /// </summary>
        /// <param name="entity"></param>
        public virtual PayableInfo UpdateInvoice(PayableInfo entity)
        {

            string status = m_PayableDA.QueryInvoiceStatusByPaySysNo(entity.SysNo.Value);
            if (status == "E")
            {
                ThrowBizException("Payable_Update_LockedNotEditStatus");
            }

            m_PayableDA.UpdateInvoiceInfo(entity);

            var payableInfo = LoadBySysNo(entity.SysNo.Value);
            //记录操作日志
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                 GetMessageString("Payable_Log_Update", ServiceContext.Current.UserSysNo, entity.SysNo)
                , BizEntity.Common.BizLogType.Finance_Pay_Update
                , payableInfo.SysNo.Value
                , payableInfo.CompanyCode);

            return payableInfo;
        }

        /// <summary>
        /// 审核应付款
        /// </summary>
        /// <param name="sysNo">应付款系统编号</param>
        /// <param name="objectAuditStatus">审核的目标状态，分待财务审核或已审核</param>
        public virtual void Audit(int sysNo, PayableAuditStatus objectAuditStatus)
        {
            PayableInfo entity = LoadBySysNo(sysNo);

            PreCheckForAudit(entity, objectAuditStatus);

            entity.AuditStatus = objectAuditStatus;
            m_PayableDA.UpdateAuditInfo(entity);
        }

        /// <summary>
        /// 审核前预先检查
        /// </summary>
        /// <param name="entity">需要审核的应付款</param>
        /// <param name="objectAuditStatus">审核的目标状态，分待财务审核或已审核</param>
        protected virtual void PreCheckForAudit(PayableInfo entity, PayableAuditStatus objectAuditStatus)
        {
            //要求审核后的新状态必须为待财务审或已审核
            if (objectAuditStatus != PayableAuditStatus.WaitFNAudit && objectAuditStatus != PayableAuditStatus.Audited)
            {
                ThrowBizException("Payable_Audit_AuditStatusInvalid");
            }

            if (objectAuditStatus == PayableAuditStatus.WaitFNAudit && entity.AuditStatus != PayableAuditStatus.NotAudit)
            {
                ThrowBizException("Payable_Audit_AuditStatusNotMatchNotAuditForPMAudit");
            }
            else if (objectAuditStatus == PayableAuditStatus.Audited && entity.AuditStatus != PayableAuditStatus.WaitFNAudit)
            {
                ThrowBizException("Payable_Audit_AuditStatusNotMatchWaitFNAuditForFinAudit");
            }

            if (entity.PayStatus != PayableStatus.UnPay)
            {
                ThrowBizException("Payable_Audit_StatusNotMatchUnPay");
            }
        }

        /// <summary>
        /// 拒绝审核应付款
        /// </summary>
        /// <param name="entity"></param>
        public virtual void RefuseAudit(int sysNo, int pmSysNo)
        {
            PayableInfo payableInfo = LoadBySysNo(sysNo);

            PreCheckForRefuseAudit(payableInfo);

            payableInfo.AuditStatus = PayableAuditStatus.NotAudit;
            payableInfo.Memo = GetMessageString("Payable_RefuseAudit_Success", payableInfo.OrderSysNo);
            m_PayableDA.UpdateAuditInfo(payableInfo);

            //发送邮件通知
            SendMailAfterRefuseAuditFinancePay(pmSysNo, payableInfo);
        }

        /// <summary>
        /// 拒绝审核应付款预先检查
        /// </summary>
        /// <param name="payableInfo"></param>
        protected void PreCheckForRefuseAudit(PayableInfo payableInfo)
        {
            //拒绝审核时的检测:应付记录尚未支付，并且处于待财务审状态
            if (payableInfo.PayStatus != PayableStatus.UnPay || payableInfo.AuditStatus != PayableAuditStatus.WaitFNAudit)
            {
                ThrowBizException("Payable_RefuseAudit_StatusOrAuditNotMatch");
            }
        }

        /// <summary>
        /// 取得PM和TL的邮件地址，多个邮件地址之间用逗号隔开
        /// </summary>
        /// <param name="pmSysNo"></param>
        /// <returns></returns>
        private string GetPMAndTLMailMailAddress(int pmSysNo)
        {
            //调用IM服务，根据PMSysNo取得PM组信息
            var pmList = ExternalDomainBroker.GetPMListByPMSysNo(pmSysNo);
            if (pmList == null
                || pmList.UserInfo == null
                || pmList.ProductManagerInfoList == null)
            {
                ThrowBizException("Payable_GetPMGroupWhenSendMail_Faults", pmSysNo);
            }
            var emailList = new List<UserInfo>() { pmList.UserInfo }.Union(
                pmList.ProductManagerInfoList
                .Where(w => w.UserInfo.SysNo == pmSysNo)
                .Select(s => s.UserInfo))
            .Where(w => StringUtility.IsEmailAddress(w.EmailAddress))
            .Select(s => s.EmailAddress);

            return string.Join(",", emailList);
        }

        /// <summary>
        /// 财务拒绝审核应付记录,通知归属的PM
        /// </summary>
        /// <param name="pmSysNo"></param>
        /// <param name="payableInfo"></param>
        private void SendMailAfterRefuseAuditFinancePay(int pmSysNo, PayableInfo payableInfo)
        {
            string mailAddress = GetPMAndTLMailMailAddress(pmSysNo);

            //拒绝操作记日志
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                  GetMessageString("Payable_Log_RefuseAuditFinancePay", mailAddress)
                , BizLogType.Invoice_Payable_RefuseAuditFinancePay
                , payableInfo.SysNo.Value
                , payableInfo.CompanyCode);

            //邮件地址不为空时发邮件通知
            if (!string.IsNullOrEmpty(mailAddress))
            {
                KeyValueVariables replaceVariables = new KeyValueVariables();
                replaceVariables.Add("OrderSysNo", payableInfo.OrderSysNo);
                replaceVariables.Add("BatchNumber", payableInfo.BatchNumber);
                replaceVariables.Add("OrderType", payableInfo.OrderType.ToDisplayText());
                replaceVariables.Add("OrderAmt", payableInfo.OrderAmt.Value.ToString(InvoiceConst.StringFormat.DecimalFormat));
                replaceVariables.Add("UserSysNo", ServiceContext.Current.UserSysNo);
                EmailHelper.SendEmailByTemplate(mailAddress, "Payable_RefuseAudit_Notify", replaceVariables, true);
            }
        }

        /// <summary>
        /// 更新应付款状态
        /// </summary>
        /// <param name="entity"></param>
        public virtual void UpdateStatus(PayableInfo payableInfo)
        {
            m_PayableDA.UpdateStatus(payableInfo);
        }

        /// <summary>
        /// 更新应付款审核状态
        /// </summary>
        /// <param name="payableInfo"></param>
        public virtual void UpdateAuditInfo(PayableInfo payableInfo)
        {
            m_PayableDA.UpdateAuditInfo(payableInfo);
        }

        /// <summary>
        /// 按应付款主记录进行支付操作
        /// </summary>
        /// <param name="payableInfo"></param>
        /// <param name="payUserSysNo"></param>
        /// <returns></returns>
        public PayableInfo PayByPayableSysNo(PayableInfo payableInfo, int payUserSysNo)
        {
            try
            {
                PayableInfo oldPayable = null;
                var oldPayableList = m_FinaceDA.PayableQuery(new PayableCriteriaInfo() { SysNo = payableInfo.SysNo });
                if (oldPayableList == null || oldPayableList.Count == 0)
                {
                    //throw new BizException("未找到此单据!");
                    ThrowBizException("Payable_BillNotFount");
                }
                else
                {
                    oldPayable = oldPayableList[0];
                    payableInfo.OrderType = oldPayable.OrderType;
                    payableInfo.BatchNumber = oldPayable.BatchNumber;
                }
                if (!string.Equals(oldPayable.AuditStatus.ToString(), PayableAuditStatus.Audited.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    //throw new BizException("支付操作要求单据的审核状态为[财务审核通过]!");
                    ThrowBizException("Payable_PayJustSupportAuditPassStatus");
                }
                if (oldPayable.PayStatus != PayableStatus.UnPay)
                {
                    //throw new BizException("支付操作要求单据的支付状态为[未支付]!");
                    ThrowBizException("Payable_PayJustSupportPayTypeIsNotPay");
                }
                var pItems = m_FinaceDA.QueryPayItems(new PayableItemCriteriaInfo() { PaySysNo = payableInfo.SysNo, Status = PayItemStatus.Origin });
                if (pItems == null || pItems.Count == 0)
                {
                    //throw new BizException(string.Format("未找到此应付记录[{0}]的未付款信息！", payableInfo.SysNo));
                    ThrowBizException("Payable_NotFoundUnPayInfo", payableInfo.SysNo);
                }
                TransactionOptions option = new TransactionOptions();
                option.IsolationLevel = IsolationLevel.ReadUncommitted;
                option.Timeout = TransactionManager.DefaultTimeout;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, option))
                {
                    foreach (var x in pItems)
                    {
                        x.PayUserSysNo = payUserSysNo;
                        this.PayPayItem(x);
                    }
                    scope.Complete();
                }
            }
            catch (BizException bex)
            {
               // throw new BizException(string.Format("单据【{0}】类型【{1}】支付失败：{2}", (payableInfo.OrderType != PayableOrderType.PO) ? payableInfo.OrderSysNo.ToString() : string.Format("{0}-{1}", payableInfo.OrderSysNo, payableInfo.BatchNumber.Value.ToString().PadLeft(2, '0')), EnumHelper.GetDescription(payableInfo.OrderType), bex.Message));
                ThrowBizException("Payable_BillTypePayError", (payableInfo.OrderType != PayableOrderType.PO) ? payableInfo.OrderSysNo.ToString() : string.Format("{0}-{1}", payableInfo.OrderSysNo, payableInfo.BatchNumber.Value.ToString().PadLeft(2, '0')), EnumHelper.GetDescription(payableInfo.OrderType), bex.Message);
            }
            return payableInfo;
        }
        /// <summary>
        /// 更新应付款状态和已付金额
        /// </summary>
        /// <param name="payableInfo"></param>
        public virtual void UpdateStatusAndAlreadyPayAmt(PayableInfo payableInfo)
        {
            m_PayableDA.UpdateStatusAndAlreadyPayAmt(payableInfo); 
        }

        public void PayPayItem(PayItemInfo entity)
        {
            if (entity.SysNo == null)
            {
                throw new ArgumentNullException("entity.SysNo");
            }
            if (entity.PayUserSysNo == null)
            {
                throw new ArgumentNullException("entity.PayUserSysNo");
            }
            var payItems = m_FinaceDA.QueryPayItems(new PayableItemCriteriaInfo
            {
                SysNo = entity.SysNo
            });
            if (payItems == null || payItems.Count == 0)
            {
                //throw new BizException(string.Format("编号为[{0}]的付款单不存在", entity.SysNo));
                ThrowBizException("Payable_PayBillNotFound");
            }
            var item = payItems[0];
            if (item.Status != PayItemStatus.Origin)
            {
                //throw new BizException("只有未支付的付款单才能支付.");
                ThrowBizException("Payable_JustUnPayBillCanPay");
            }

            item.PayUserSysNo = entity.PayUserSysNo;

            var payList = m_FinaceDA.PayableQuery(new PayableCriteriaInfo { SysNo = item.PaySysNo });
            if (payList == null || payList.Count == 0)
            {
                //throw new BizException("未找到该付款单对应的应付款信息");
                ThrowBizException("Payable_PayBillNotFoundPayInfo");
            }
            var pay = payList[0];

            if (!string.Equals(pay.AuditStatus.ToString(), PayableAuditStatus.Audited.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                //throw new BizException("只有财务审核通过的付款单才能支付.");
                ThrowBizException("Payable_JustAuditPassCanPay");
            }

            item.OrderSysNo = pay.OrderSysNo;
            item.BatchNumber = pay.BatchNumber;

            //现金支付的必须有银行科目号
            //if (pay.OrderType == PayableOrderType.ReturnPointCashAdjust)
            //{
            //    if (string.IsNullOrEmpty(entity.BankGLAccount))
            //    {
            //        throw new BizException("现金支付必须有银行科目账号");
            //    }
            //    item.BankGLAccount = entity.BankGLAccount;
            //}

            if (pay.OrderType == PayableOrderType.PO
                || pay.OrderType == PayableOrderType.POAdjust)
            {
                item = new POProcess().Pay(item, true);
            }
            //else if (pay.OrderType == PayableOrderType.ReturnPointCashAdjust
            //    || pay.OrderType == PayableOrderType.SubAccount
            //    || pay.OrderType == PayableOrderType.SubInvoice)
            //{
            //    item = new EIMSReturnPointCashProcess().Pay(item, true);
            //}
            else
            {
                item = new OtherOrderTypeProcess().Pay(item, true);
            }
        }

        /// <summary>
        /// 更新应付款状态和单据金额
        /// </summary>
        /// <param name="entity"></param>
        public virtual void UpdateStatusAndOrderAmt(PayableInfo entity)
        {
            m_PayableDA.UpdateStatusAndOrderAmt(entity);
        }

        /// <summary>
        /// 根据应付款系统编号取得应付款信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual PayableInfo LoadBySysNo(int sysNo)
        {
            var entity = m_PayableDA.LoadBySysNo(sysNo);
            if (entity == null)
            {
                ThrowBizException("Payable_RecordNotExistFormat", sysNo);
            }
            return entity;
        }

        /// <summary>
        /// 根据查询条件取得应付款列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual List<PayableInfo> GetListByCriteria(PayableInfo query)
        {
            return m_PayableDA.GetListByCriteria(query);
        }

        /// <summary>
        /// 更新预计付款时间
        /// </summary>
        public virtual void UpdatePayableETP(PayableInfo payableInfo)
        {
            m_PayableDA.UpdateETP(payableInfo);
        }

        /// <summary>
        /// 更新预EGP ETP
        /// </summary>
        public virtual void UpdatePayableEGPAndETP(PayableInfo payableInfo)
        {
            m_PayableDA.UpdatePayableEGPAndETP(payableInfo);
        }

        /// <summary>
        /// 更新应付款发票状态
        /// </summary>
        /// <param name="payable"></param>
        public virtual void UpdatePayableInvoiceStatus(PayableInfo payable)
        {
            m_PayableDA.UpdatePayableInvoiceStatus(payable);
            //记录操作日志
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                 GetMessageString("Payable_Log_Update", ServiceContext.Current.UserSysNo, payable.SysNo)
                , BizLogType.Finance_Pay_Item_Add
                , payable.SysNo.Value
                , payable.CompanyCode);
        }

        /// <summary>
        /// 更新应付款发票状态和ETP
        /// </summary>
        /// <param name="payable"></param>
        public virtual void UpdatePayableInvoiceStatusWithEtp(PayableInfo payable)
        {
            m_PayableDA.UpdatePayableInvoiceStatusWithEtp(payable);
            //记录操作日志
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                 GetMessageString("Payable_Log_Update", ServiceContext.Current.UserSysNo, payable.SysNo)
                , BizLogType.Finance_Pay_Item_Add
                , payable.SysNo.Value
                , payable.CompanyCode);
        }

        #region [For PO Domain]

        public virtual bool IsAbandonGatherPayItem(int sysNo)
        {
            return m_PayableDA.IsAbandonGatherPayItem(sysNo);
        }

        public bool IsAbandonPayItem(int sysNo)
        {
            return m_PayableDA.IsAbandonPayItem(sysNo);
        }

        public virtual decimal GetVendorPayBalanceByVendorSysNo(int vendorSysNo)
        {
            return ObjectFactory<IPayableDA>.Instance.GetVendorPayBalanceByVendorSysNo(vendorSysNo);
        }

        public virtual PayItemStatus? GetPOPrePayItemStatus(int poSysNo)
        {
            return ObjectFactory<IPayableDA>.Instance.GetPOPrePayItemStatus(poSysNo);
        }

        public virtual void CreateByVendor(PayableInfo entity)
        {
            if (!entity.EIMSAmt.HasValue)
            {
                entity.EIMSAmt = 0;
            }
            if (!entity.InStockAmt.HasValue)
            {
                entity.InStockAmt = 0;
            }

            //PO调整单
            if (entity.OrderType == PayableOrderType.POAdjust)
            {
                CreatePOAdjustByVendor(entity);
            }
            else if (entity.OrderType == PayableOrderType.PO)
            {
                #region CRL17366 代销采购单不生成付款单

                var poEntity = ExternalDomainBroker.GetPurchaseOrderInfo(entity.OrderSysNo.Value, entity.BatchNumber.Value);
                if (poEntity != null && poEntity.PurchaseOrderBasicInfo.ConsignFlag == PurchaseOrderConsignFlag.Consign)
                {
                    return;
                }

                #endregion CRL17366 代销采购单不生成付款单

                entity.EstimatedTimeOfPay = ETPCalculatorHelper.GetETPByPayPeriod(entity, DateTime.MinValue);

                //获取付款时间
                DateTime payTime = DateTime.MinValue;
                var result = ObjectFactory<PayItemProcessor>.Instance.GetListByCriteria(new PayItemInfo
                {
                    OrderSysNo = entity.OrderSysNo.Value,
                    OrderType = entity.OrderType,
                    Status = PayItemStatus.Paid
                });

                if (result.Count > 0 && result[0].PayTime.HasValue)
                {
                    payTime = result[0].PayTime.Value;
                }
                PrePayPOByVendor(entity);
            }
        }

        protected virtual void PrePayPOByVendor(PayableInfo entity)
        {
            //PO单支付类型是否是预付款
            var isAdvanced = ObjectFactory<PayItemProcessor>.Instance.IsAdvanced(PayableOrderType.PO, entity.OrderSysNo.Value);
            if (isAdvanced)
            {
                var result = ObjectFactory<PayItemProcessor>.Instance.GetListByCriteria(new PayItemInfo
                {
                    OrderSysNo = entity.OrderSysNo,
                    OrderType = entity.OrderType,
                    PayStyle = PayItemStyle.Advanced
                });
                result = result.FindAll(p => p.Status != PayItemStatus.Abandon);
                if (result.Count > 1)
                {
                    ThrowBizException("Payable_PrePayPOByVendor_ExistManyOfDifferentStateOfAdvancePO");
                }

                if (result.Count == 1 && result.Exists(x => x.Status == PayItemStatus.Origin))
                {
                    ProcessUnpaidPrepay(entity, result[0]);
                }
                else if (result.Count == 1 && result.Exists(x => x.Status == PayItemStatus.Paid))
                {
                    ProcessPayedPrepay(entity, result[0]);
                }
            }
            else
            {
                if (entity.OrderStatus.HasValue)
                {
                    POInStock(entity);
                    //记录操作日志
                    ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                     GetMessageString("Payable_Log_POInStock", ServiceContext.Current.UserSysNo, entity.OrderSysNo, entity.BatchNumber, entity.OrderStatus)
                     , BizLogType.PO_InStock
                     , entity.OrderSysNo.Value
                     , entity.CompanyCode);
                }
                else
                {
                    ThrowBizException("Payable_PrePayPOByVendor_POStateIsNull");
                }
            }
        }

        protected virtual void ProcessPayedPrepay(PayableInfo entity, PayItemInfo payitem)
        {
            PayableInfo payable = null;
            if (payitem.AvailableAmt < entity.InStockAmt - entity.EIMSAmt)
            {
                ThrowBizException("Payable_ProcessPayedPrepay_InStockAmtMoreThanAvailableAmt");
            }
            //更新付款单可用金额
            payitem.AvailableAmt -= entity.InStockAmt - entity.EIMSAmt;
            ObjectFactory<PayItemProcessor>.Instance.UpdateAvailableAmt(payitem.OrderType.Value, payitem.OrderSysNo.Value, payitem.AvailableAmt.Value);

            var pEntity = m_PayableDA.GetFirstPay(entity.OrderType.Value, entity.OrderSysNo.Value);
            if (entity.BatchNumber == 1)
            {
                if (pEntity == null)
                {
                    ThrowBizException("Payable_ProcessPayedPrepay_NotExistPaidRecord");
                }
                payable = new PayableInfo();
                payable.OrderAmt = pEntity.OrderAmt;
                payable.SysNo = pEntity.SysNo;
                payable.PayStatus = PayableStatus.FullPay;
                payable.OrderStatus = entity.OrderStatus;
                payable.InStockAmt = entity.InStockAmt;
                payable.EIMSAmt = entity.EIMSAmt;
                payable.RawOrderAmt = entity.RawOrderAmt;
                m_PayableDA.UpdateFirstPay(payable);
            }
            else
            {
                payable = new PayableInfo();
                payable.OrderSysNo = entity.OrderSysNo;
                payable.OrderType = entity.OrderType;
                payable.BatchNumber = entity.BatchNumber;
                payable.PayStatus = PayableStatus.FullPay;
                payable.AlreadyPayAmt = 0;
                payable.OrderAmt = payable.AlreadyPayAmt;
                payable.RawOrderAmt = entity.RawOrderAmt;
                payable.InStockAmt = entity.InStockAmt;
                payable.EIMSAmt = entity.EIMSAmt;
                payable.Note = "auto create during in stock";
                payable.CurrencySysNo = 1;
                payable.OrderStatus = entity.OrderStatus;
                payable = Create(payable);
            }

            PayItemInfo pie = new PayItemInfo();
            pie.OrderSysNo = entity.OrderSysNo;
            pie.OrderType = (PayableOrderType)entity.OrderType;
            pie.PaySysNo = payable.SysNo.Value;
            pie.PayAmt = entity.InStockAmt.Value - entity.EIMSAmt.Value;
            pie.SysNo = payitem.SysNo;
            pie.OrderStatus = entity.OrderStatus;
            pie.CompanyCode = entity.CompanyCode;

            if (entity.BatchNumber != 1)
            {
                ObjectFactory<PayItemProcessor>.Instance.CreatePayEx(pie);
            }

            if (entity.OrderStatus.Value == (int)PurchaseOrderStatus.PartlyInStocked)
            {
                //调用PO接口中止入库
                ExternalDomainBroker.StopInStock(entity.OrderSysNo.Value);
            }
        }

        protected virtual void ProcessUnpaidPrepay(PayableInfo entity, PayItemInfo payItem)
        {
            //作废付款单
            ObjectFactory<PayItemProcessor>.Instance.Abandon(payItem);

            //新建应付款
            POInStock(entity);
        }

        protected virtual void POInStock(PayableInfo entity)
        {
            PayItemInfo payitem = new PayItemInfo();
            payitem.PayStyle = PayItemStyle.Normal;
            payitem.PayAmt = entity.InStockAmt.Value - entity.EIMSAmt.Value;
            payitem.Note = "auto create during in stock";
            payitem.Status = PayItemStatus.Origin;
            payitem.OrderSysNo = entity.OrderSysNo;
            payitem.OrderType = entity.OrderType;
            payitem.EstimatedTimeOfPay = entity.EstimatedTimeOfPay;
            payitem.EstimatePayTime = entity.EstimatedTimeOfPay;
            payitem.InStockAmt = entity.InStockAmt;
            payitem.RawOrderAmt = entity.RawOrderAmt;
            payitem.BatchNumber = entity.BatchNumber;
            payitem.EIMSAmt = entity.EIMSAmt;
            payitem.OrderStatus = entity.OrderStatus;
            payitem.CompanyCode = entity.CompanyCode;

            ObjectFactory<PayItemProcessor>.Instance.Create(payitem);
        }

        protected virtual void CreatePOAdjustByVendor(PayableInfo entity)
        {
            var result = ObjectFactory<PayItemProcessor>.Instance.GetListByCriteria(new PayItemInfo
            {
                OrderSysNo = entity.OrderSysNo,
                OrderType = PayableOrderType.PO,
                PayStyle = PayItemStyle.Advanced,
                Status = PayItemStatus.Paid
            });
            if (result.Count <= 0)
            {
                return;
            }
            var item = result[0];
            if (item.PayAmt <= 0 || item.AvailableAmt.Value <= 0)
            {
                return;
            }
            if (entity.OrderStatus != 6)
            {
                ThrowBizException("Payable_CreatePOAdjustByVendor_StatusInValid", entity.OrderStatus);
            }
            if (entity.InStockAmt != item.AvailableAmt)
            {
                ThrowBizException("Payable_CreatePOAdjustByVendor_InStockAmtNotEqualAvailableAmt", (entity.InStockAmt.HasValue ? entity.InStockAmt.Value.ToString("#.00") : "0.00"), (item.AvailableAmt.HasValue ? item.AvailableAmt.Value.ToString("#.00") : "0:00"));
            }
            TransactionOptions option = new TransactionOptions();
            //option.IsolationLevel = IsolationLevel.ReadUncommitted;
            option.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, option))
            {
                PayItemInfo payitem = new PayItemInfo();
                payitem.OrderType = PayableOrderType.POAdjust;
                payitem.OrderSysNo = entity.OrderSysNo;
                payitem.PayStyle = PayItemStyle.Normal;
                payitem.PayAmt = -item.AvailableAmt.Value;
                payitem.Note = "auto create Before Close PO";
                payitem.Status = PayItemStatus.Origin;
                payitem.OrderStatus = entity.OrderStatus;
                payitem.EstimatedTimeOfPay = entity.EstimatedTimeOfPay;
                payitem.RawOrderAmt = -item.AvailableAmt.Value;
                payitem.EIMSAmt = 0;

                ObjectFactory<PayItemProcessor>.Instance.Create(payitem);
                ObjectFactory<PayItemProcessor>.Instance.UpdateAvailableAmt(item.OrderType.Value, item.OrderSysNo.Value, 0);

                scope.Complete();
            }
            //记录操作日志
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                 GetMessageString("Payable_Log_CreatePOAdjustByVendor", ServiceContext.Current.UserSysNo, item.SysNo)
                , BizLogType.Finance_Pay_Item_Add
                , item.SysNo.Value
                , item.CompanyCode);
        }

        /// <summary>
        /// 查找未支付或部分支付的应付款
        /// 这里只查询PO、代销结算单和代收结算单三种单据类型
        /// </summary>
        /// <returns></returns>
        public virtual List<PayableInfo> GetUnPayOrPartlyPayList()
        {
            return m_PayableDA.GetUnPayOrPartlyPayList();
        }

        #endregion [For PO Domain]

        #region [Private Helper Methods]

        private void ThrowBizException(string msgKeyName, params object[] args)
        {
            throw new BizException(GetMessageString(msgKeyName, args));
        }

        private string GetMessageString(string msgKeyName, params object[] args)
        {
            return string.Format(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.Payable, msgKeyName), args);
        }

        #endregion [Private Helper Methods]
    }
}
