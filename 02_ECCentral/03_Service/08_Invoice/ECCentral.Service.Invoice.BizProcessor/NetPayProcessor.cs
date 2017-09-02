using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.Service.EventMessage.Invoice;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Invoice.IDataAccess;
//using ECCentral.Service.ThirdPart.Interface;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.BizProcessor
{
    [VersionExport(typeof(NetPayProcessor))]
    public class NetPayProcessor
    {
        private INetPayDA m_NetpayDA = ObjectFactory<INetPayDA>.Instance;
        TransactionOptions options = new TransactionOptions();
        /// <summary>
        /// 创建NetPay
        /// </summary>
        /// <param name="netpayEntity">网上支付实体</param>
        /// <param name="refundEntity">退款实体</param>
        /// <param name="isForceCheck">是否强制核收，如果是强制核收，refundEntity必须要有值</param>
        /// <returns>创建好的netpay实体</returns>
        public virtual NetPayInfo Create(NetPayInfo netpayEntity, SOIncomeRefundInfo refundEntity, bool isForceCheck)
        {
            SOBaseInfo soBaseInfo = ExternalDomainBroker.GetSOBaseInfo(netpayEntity.SOSysNo.Value);

            //创建前预检查
            PreCheckForCreate(netpayEntity, refundEntity, soBaseInfo, isForceCheck);

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = IsolationLevel.ReadCommitted;
            options.Timeout = TimeSpan.FromMinutes(2);
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, options))
            {
                //Step 1:写入退款信息
                bool isOverpay = netpayEntity.PayAmount > soBaseInfo.ReceivableAmount;
                if (isForceCheck && isOverpay)
                {
                    refundEntity.CompanyCode = soBaseInfo.CompanyCode;
                    CreateRefundInfoForForceCheck(netpayEntity, refundEntity);
                }

                //Step 2:写入网上支付信息
                netpayEntity.Status = NetPayStatus.Origin;
                netpayEntity.ExternalKey = m_NetpayDA.GetExternalKeyBySOSysNo(netpayEntity.SOSysNo.Value);
                netpayEntity.MasterSoSysNo = soBaseInfo.SOSplitMaster;
                netpayEntity.CompanyCode = soBaseInfo.CompanyCode;
                if (string.IsNullOrEmpty(netpayEntity.Note))
                {
                    netpayEntity.Note = "add by newegg employees.";
                }
                netpayEntity = m_NetpayDA.Create(netpayEntity);

                //如果生成netpay记录的订单有母单，则更新netpay的母单信息
                if (soBaseInfo.SOSplitMaster.HasValue && soBaseInfo.SOSplitMaster.Value > 0)
                {
                    var masterSO = ExternalDomainBroker.GetSOBaseInfo(soBaseInfo.SOSplitMaster.Value);
                    m_NetpayDA.UpdateMasterSOAmt(masterSO);
                }

                //发送创建netpay消息
                EventPublisher.Publish(new CreateNetpayMessage()
                {
                    NetpaySysNo = netpayEntity.SysNo.Value,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });

                ts.Complete();
            }

            //记录业务Log
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                GetMessageString("NetPay_Log_Create", ServiceContext.Current.UserSysNo, netpayEntity.SysNo)
                , ECCentral.BizEntity.Common.BizLogType.Finance_NetPay_AddVerified
                , netpayEntity.SysNo.Value
                , netpayEntity.CompanyCode);

            return netpayEntity;
        }

        /// <summary>
        /// 审核网上支付
        /// </summary>
        /// <param name="netpaySysNo">netpay系统编号</param>
        public virtual void Audit(int netpaySysNo)
        {


            NetPayInfo netpayInfo = LoadBySysNo(netpaySysNo);

            if (netpayInfo == null)
            {
                ThrowBizException("NetPay_NeyPayRecordNotExist", netpaySysNo);
            }

            //if (netpayInfo.InputUserSysNo != AuditInfo.AuditUserSysNo)
            //{
            //    ThrowBizException("NetPay_InputAndAuditUserCannotSame", netpaySysNo);
            //}

            SOBaseInfo soBaseInfo = ExternalDomainBroker.GetSOBaseInfo(netpayInfo.SOSysNo.Value);

            SOInfo soInfo = ExternalDomainBroker.GetSOInfo(netpayInfo.SOSysNo.Value);

            if (soBaseInfo == null)
            {
                ThrowBizException("NetPay_SORecordNotExist", netpayInfo.SOSysNo);
            }

            //审核前检查
            PreCheckForAudit(netpayInfo, soBaseInfo);

            SOIncomeInfo soIncomeInfo;

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = IsolationLevel.ReadCommitted;
            options.Timeout = TimeSpan.FromMinutes(2);

            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, options))
            {
                //1.创建销售收款
                //如果有主单，则更新主单的收款单金额
                soIncomeInfo = CreateSOIncomeInfo(netpayInfo, soBaseInfo);
                if (soBaseInfo.SOSplitMaster.HasValue && soBaseInfo.SOSplitMaster.Value > 0)
                {
                    var masterSO = ExternalDomainBroker.GetSOBaseInfo(soBaseInfo.SOSplitMaster.Value);
                    ObjectFactory<SOIncomeProcessor>.Instance.UpdateMasterSOAmt(masterSO);
                }

                //2.审核网上支付,将网上支付记录的状态修改为审核通过
                m_NetpayDA.UpdateApproveInfo(netpaySysNo, NetPayStatus.Approved);

                //3.审核财务多付退款记录
                var refundList = ObjectFactory<SOIncomeRefundProcessor>.Instance.GetListByCriteria(new SOIncomeRefundInfo()
                {
                    OrderSysNo = soIncomeInfo.OrderSysNo.Value,
                    OrderType = RefundOrderType.OverPayment,
                    Status = RefundStatus.Origin
                });
                SOIncomeRefundInfo overpayRefund = null;
                if (refundList != null && refundList.Count > 0)
                {
                    overpayRefund = refundList[0];
                }
                if (overpayRefund != null)
                {
                    //如果是现金退款，则退款信息自动审核通过
                    if (overpayRefund.RefundPayType == RefundPayType.CashRefund)
                    {
                        overpayRefund.Status = RefundStatus.Audit;
                        ObjectFactory<SOIncomeRefundProcessor>.Instance.Update(overpayRefund);
                    }
                    //创建财务负收款单
                    overpayRefund.PayAmount = netpayInfo.PayAmount;
                    ObjectFactory<SOIncomeProcessor>.Instance.CreateNegative(overpayRefund);
                }

                //发送netpay审核完成Message
               
                ts.Complete();
            }
            //待办事项通知，异常不处理，不影响正常业务
            try
            {
                EventPublisher.Publish<InvoiceNetpayAuditedMessage>(new InvoiceNetpayAuditedMessage()
                      {
                          SoSysNo = netpayInfo.SOSysNo.Value,
                          MerchantSysNo = soInfo.Merchant.SysNo.GetValueOrDefault(),
                          SOType = (int)soInfo.BaseInfo.SOType,
                          ReferenceSysNo = soInfo.BaseInfo.ReferenceSysNo??0,
                          AuditUserName = ServiceContext.Current.UserDisplayName,
                          AuditUserSysNo = ServiceContext.Current.UserSysNo,
                          NetpaySysNo = netpayInfo.SysNo.Value,
                          SplitType = (int)soInfo.BaseInfo.SplitType
                      });
            }
            catch(Exception ex)
            {
                string ere = ex.Message;
            }
            //记录操作日志，用户审核了网上收款单
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                GetMessageString("NetPay_Log_Audit", ServiceContext.Current.UserSysNo, netpaySysNo)
                , ECCentral.BizEntity.Common.BizLogType.Finance_NetPay_Verify
                , soIncomeInfo.SysNo.Value
                , soIncomeInfo.CompanyCode);

            //4、审核移仓单
            VerifyShiftRequest(netpayInfo);
            //库存模式同步送货单给ERP,此处仅限在线支付。货到的在订单审核时会发送
            if (soBaseInfo.PayWhenReceived==false)
            {
                SyncSHD(netpayInfo.SOSysNo.Value);
            }
          

        }
        //同步送货单
        public void SyncSHD(int SOSysNo)
        {
            var soItem = ExternalDomainBroker.GetSOItemList(SOSysNo);
            soItem = soItem.Where(p => p.InventoryType == ProductInventoryType.Company || p.InventoryType == ProductInventoryType.TwoDoor).ToList();
            //同步送货单
           
            //foreach (var item in soItem)
            //{
            //    ERPSHDInfo erpinfo = new ERPSHDInfo();
            //    erpinfo.SHDTypeMemo = "送货单";
            //    erpinfo.RefOrderNo = SOSysNo.ToString();
            //    erpinfo.RefOrderType = "销售订单";
            //    erpinfo.SysMemo = erpinfo.RefOrderNo + "/" + erpinfo.RefOrderType;
            //    erpinfo.ZDR = ServiceContext.Current.UserSysNo;
            //    erpinfo.ZDSJ = DateTime.Now;
            //    erpinfo.ZXR = ServiceContext.Current.UserSysNo;
            //    erpinfo.ZXSJ = DateTime.Now;
            //    erpinfo.SHDItemList = new List<ERPSHDItem>();
            //    ERPSHDItem erpitem = new ERPSHDItem();
            //    erpitem.ProductSysNo = item.ProductSysNo;
            //    erpitem.SL = item.Quantity;           
            //    erpinfo.SHDItemList.Add(erpitem);
            //    ObjectFactory<ISyncERPBizRecord>.Instance.CreateSHD(erpinfo);           
            //}

            var Inventory = soItem.Where(p => p.InventoryType == ProductInventoryType.Company || p.InventoryType == ProductInventoryType.TwoDoor);
            if (Inventory.Count() > 0)
            {
                ERPInventoryAdjustInfo erpAdjustInfo = new ERPInventoryAdjustInfo
                {
                    OrderSysNo = SOSysNo,
                    OrderType = "SO",
                    AdjustItemList = new List<ERPItemInventoryInfo>(),
                    Memo = "在线订单支付确认"
                };
                foreach (var item in Inventory)
                {
                    ERPItemInventoryInfo adjustItem = new ERPItemInventoryInfo
                    {
                        ProductSysNo = item.ProductSysNo,
                        HQQuantity = -item.Quantity.Value
                    };

                    erpAdjustInfo.AdjustItemList.Add(adjustItem);

                }
                if (erpAdjustInfo.AdjustItemList.Count > 0)
                {
                    string adjustXml = ECCentral.Service.Utility.SerializationUtility.ToXmlString(erpAdjustInfo);
                    ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(string.Format("支付确认扣除ERP【总部家电/双开门】库存：{0}", adjustXml)
                      , BizLogType.Finance_NetPay_Verify
                      , SOSysNo
                      , "8601");
                    //ObjectFactory<IAdjustERPInventory>.Instance.AdjustERPInventory(erpAdjustInfo);

                }
            } 
        }
        /// <summary>
        /// 审核移仓单
        /// </summary>
        protected virtual void VerifyShiftRequest(NetPayInfo netpayInfo)
        {
            int shiftSysNo = ExternalDomainBroker.GetShiftSysNoBySOSysNo(netpayInfo.SOSysNo.Value);
            if (shiftSysNo > 0)
            {
                try
                {
                    //调用Inventory接口审核移仓单
                    ExternalDomainBroker.VerifyShiftRequest(shiftSysNo);
                    //记录操作日志，该支付单的移仓单审核成功
                    ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                        GetMessageString("NetPay_Log_VerifyShiftRequestSuccess", ServiceContext.Current.UserSysNo, netpayInfo.SysNo.Value)
                        , ECCentral.BizEntity.Common.BizLogType.Finance_NetPay_Verify
                        , netpayInfo.SysNo.Value
                        , netpayInfo.CompanyCode);
                }
                catch (System.Exception ex)
                {
                    //审核移仓单失败，记录操作日志，该支付单的移仓单审核失败
                    ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                     GetMessageString("NetPay_Log_VerifyShiftRequestFailed", ServiceContext.Current.UserSysNo, netpayInfo.SysNo.Value, ex.Message)
                    , ECCentral.BizEntity.Common.BizLogType.Finance_NetPay_Verify
                    , netpayInfo.SysNo.Value
                    , netpayInfo.CompanyCode);
                }
            }
        }

        /// <summary>
        /// 作废网上支付
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void Abandon(int sysNo)
        {
            NetPayInfo netpay = LoadBySysNo(sysNo);
            if (netpay == null)
            {
                ThrowBizException("NetPay_RecordNotExist", sysNo);
            }

            if (netpay.Status != NetPayStatus.Origin)
            {
                ThrowBizException("NetPay_StatusNotAllowAbandon");
            }

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = IsolationLevel.ReadUncommitted;
            options.Timeout = TimeSpan.FromMinutes(2);
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, options))
            {
                //更新netpay状态为已作废
                m_NetpayDA.UpdateStatus(sysNo, NetPayStatus.Abandon);

                //2012-06-13 fixbug：检查订单是否有多付退款记录，有则将多付退款记录作废
                var overpayRefundList = ObjectFactory<SOIncomeRefundProcessor>.Instance.GetListByCriteria(new SOIncomeRefundInfo()
                {
                    OrderSysNo = netpay.SOSysNo,
                    OrderType = RefundOrderType.OverPayment,
                    Status = RefundStatus.Origin
                });
                if (overpayRefundList != null && overpayRefundList.Count > 0)
                {
                    //作废多付退款记录
                    ObjectFactory<ISOIncomeRefundDA>.Instance.UpdateStatus(overpayRefundList[0].SysNo.Value
                        , ServiceContext.Current.UserSysNo, RefundStatus.Abandon, DateTime.Now);
                }

                //发送netpay作废Message
                EventPublisher.Publish<NetpayAbandonedMessage>(new NetpayAbandonedMessage()
                {
                    NetpaySysNo = sysNo,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });

                ts.Complete();
            }
            //记录操作日志
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                GetMessageString("NetPay_Log_Abandon", ServiceContext.Current.UserSysNo, netpay.SysNo)
                , ECCentral.BizEntity.Common.BizLogType.Finance_NetPay_Abandon
                , netpay.SysNo.Value
                , netpay.CompanyCode);
        }

        /// <summary>
        /// 根据订单编号作废netpay
        /// </summary>
        /// <param name="soSysNo"></param>
        public virtual void AbandonBySOSysNo(int soSysNo)
        {
            NetPayInfo netpayInfo = GetValidBySOSysNo(soSysNo);

            if (netpayInfo != null)
            {
                TransactionOptions options = new TransactionOptions();
                options.IsolationLevel = IsolationLevel.ReadUncommitted;
                options.Timeout = TimeSpan.FromMinutes(2);
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, options))
                {

                    m_NetpayDA.AbandonBySOSysNo(soSysNo);

                    //发送netpay作废Message
                    EventPublisher.Publish<NetpayAbandonedMessage>(new NetpayAbandonedMessage()
                    {
                        NetpaySysNo = netpayInfo.SysNo.Value,
                        CurrentUserSysNo = ServiceContext.Current.UserSysNo
                    });
                    ts.Complete();
                }
              
            }
        }

        /// <summary>
        ///  Review网上支付
        /// </summary>
        /// <param name="input"></param>
        public virtual void Review(int soSysNo)
        {
            if (GetValidBySOSysNo(soSysNo) == null)
            {
                ThrowBizException("NetPay_SONotHaveValidNetPay");
            }
            m_NetpayDA.UpdateReviewInfo(soSysNo);
        }

        /// <summary>
        /// 根据netpay系统编号获取netpay信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual NetPayInfo LoadBySysNo(int sysNo)
        {
            return m_NetpayDA.LoadBySysNo(sysNo);
        }

        /// <summary>
        /// 根据查询条件取得netpay列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual List<NetPayInfo> GetListByCriteria(NetPayInfo query)
        {
            return m_NetpayDA.GetListByCriteria(query);
        }

        /// <summary>
        /// 根据订单系统编号取得订单有效的netpay
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <returns></returns>
        public virtual NetPayInfo GetValidBySOSysNo(int soSysNo)
        {
            return m_NetpayDA.GetValidBySOSysNo(soSysNo);
        }

        /// <summary>
        /// 审核netpay预检查
        /// </summary>
        /// <param name="netpayInfo"></param>
        /// <param name="soBaseInfo"></param>
        /// <param name="isForceCheck"></param>
        protected virtual void PreCheckForAudit(NetPayInfo netpayInfo, SOBaseInfo soBaseInfo)
        {
            if (netpayInfo.Status != NetPayStatus.Origin)
            {
                ThrowBizException("NetPay_StatusNotAllowCheck");
            }
            if (soBaseInfo.ReferenceSysNo.HasValue && soBaseInfo.SOType == SOType.GroupBuy)
            {
                //团购正在进行中，请待团购结束后再进行审核   部分失败的团购订单也予以审核通过
                if (soBaseInfo.SettlementStatus != SettlementStatus.Success && soBaseInfo.SettlementStatus != SettlementStatus.PlanFail)
                {
                    ThrowBizException("NetPay_GroupingBuyOrderNotAllowCheck", netpayInfo.SOSysNo);
                }
            }
            if (soBaseInfo.HoldStatus == SOHoldStatus.WebHold)
            {
                ThrowBizException("NetPay_HoldOrderNotAllowCheck", netpayInfo.SOSysNo);
            }

            //输入的订单的支付方式要与订单中的支付方式一致
            if (netpayInfo.PayTypeSysNo != soBaseInfo.PayTypeSysNo)
            {
                ThrowBizException("NetPay_PayTypeNotSameNotAllowCheck");
            }

            //验证订单的付款方式是不是“网上支付”
            if (!IsNetPayType(soBaseInfo.PayTypeSysNo.Value, soBaseInfo.SysNo.Value))
            {
                ThrowBizException("NetPay_SOPayTypeIsNotNet");
            }

            //Nick.Y.Zheng 实收金额与订单金额不相等，且误差超过1分钱，则不通过，否则通过
            //实收金额少于订单金额，不允许核收。
            decimal currPayAmount = netpayInfo.PayAmount.HasValue ? netpayInfo.PayAmount.Value : 0m;
            //if (netpayInfo.PayAmount != soBaseInfo.ReceivableAmount
            //    && Math.Abs(currPayAmount - soBaseInfo.ReceivableAmount) > 0.01m)
            //{
            //    ThrowBizException("NetPay_PayAmoutNotEqualNotAllowCheck");
            //}
            //实收金额少于订单金额，不允许核收。
            if (netpayInfo.PayAmount < soBaseInfo.ReceivableAmount  && Math.Abs(currPayAmount - soBaseInfo.ReceivableAmount) > 0.01m)
            {
                ThrowBizException("NetPay_PayAmoutNotEnoughNotAllowCheck");
            }
        }

        /// <summary>
        /// 创建netpay预检查
        /// </summary>
        /// <param name="netpayEntity"></param>
        protected virtual void PreCheckForCreate(NetPayInfo netpayEntity, SOIncomeRefundInfo refundEntity, SOBaseInfo soBaseInfo, bool isForceCheck)
        {
            //1.输入的订单的支付方式要与订单中的支付方式一致
            if (netpayEntity.PayTypeSysNo != soBaseInfo.PayTypeSysNo)
            {
                ThrowBizException("NetPay_PayTypeNotSame");
            }

            //2.需要提供验证订单的付款方式是不是“网上支付”
            if (!IsNetPayType(soBaseInfo.PayTypeSysNo.Value, soBaseInfo.SysNo.Value))
            {
                ThrowBizException("NetPay_SOPayTypeIsNotNet");
            }

            //3.检查订单是否已经存在有效的网上支付信息
            if (GetValidBySOSysNo(netpayEntity.SOSysNo.Value) != null)
            {
                ThrowBizException("NetPay_AlreadyExistValidRecord");
            }

            #region 检查旧NetPay余额是否够创建新的NetPay

            SOBaseInfo originalSO = ExternalDomainBroker.GetSOBaseInfo(netpayEntity.RelatedSoSysNo.Value);
            if (originalSO == null)
            {
                ThrowBizException("NetPay_PreCheckForCreate_SONotExist", netpayEntity.RelatedSoSysNo);
            }
            if (netpayEntity.SOSysNo != netpayEntity.RelatedSoSysNo.Value)
            {
                if ((int)originalSO.Status > 0)//原订单没有被作废
                {
                    ThrowBizException("NetPay_RelatedSONotAbandon");
                }

                if (GetValidBySOSysNo(netpayEntity.RelatedSoSysNo.Value) != null)
                {
                    ThrowBizException("NetPay_RelatedSONetPayExist");
                }
            }

            //验证规则：原订单核收金额合计加当前核收金额大于原订单金额,请修改该输入的订单金额
            //2012-06-13 freegod:人工输入的不再检查。
            decimal? totalReceivableAmt = netpayEntity.PayAmount;
            SOBaseInfo tempSo = null;
            List<NetPayInfo> relatedSoNetPayList = m_NetpayDA.GetListByRelatedSoSysNo(netpayEntity.RelatedSoSysNo.Value);
            if (relatedSoNetPayList != null && relatedSoNetPayList.Count > 0)
            {
                relatedSoNetPayList.ForEach(relatedNetPay =>
                {
                    tempSo = ExternalDomainBroker.GetSOBaseInfo(relatedNetPay.SOSysNo.Value);
                    totalReceivableAmt += tempSo.ReceivableAmount;
                });
            }

            //获取最后一笔作废的NetPay记录
            NetPayInfo lastAboundNetPay = m_NetpayDA.GetLastAboundedByRelatedSoSysNo(netpayEntity.RelatedSoSysNo.Value);
            if (lastAboundNetPay != null)
            {
                decimal avalAmount = lastAboundNetPay.PayAmount.Value;

                //关联原订单的金额合计总和大于原订单的金额,请修改该输入的订单金额
                if (totalReceivableAmt > avalAmount)
                {
                    ThrowBizException("NetPay_RelatedSOAmtNotEnough"
                        , (totalReceivableAmt.Value - netpayEntity.PayAmount.Value).ToString(InvoiceConst.StringFormat.CurrencyFormat),
                        netpayEntity.PayAmount.Value.ToString(InvoiceConst.StringFormat.CurrencyFormat), avalAmount.ToString(InvoiceConst.StringFormat.CurrencyFormat));
                }
            }

            #endregion 检查旧NetPay余额是否够创建新的NetPay

            #region 强制核收数据检查

            if (netpayEntity.PayAmount < soBaseInfo.ReceivableAmount)
            {
                ThrowBizException("NetPay_PayAmoutNotEnoughNotAllowCheck");
            }
            if (isForceCheck && netpayEntity.PayAmount == soBaseInfo.ReceivableAmount)
            {
                ThrowBizException("NetPay_NotAllowForceCheck");
            }
            if (!isForceCheck && netpayEntity.PayAmount > soBaseInfo.ReceivableAmount)
            {
                ThrowBizException("NetPay_MustChooseForceCheck");
            }
            if (isForceCheck && (refundEntity.RefundCashAmt ?? 0) <= 0M)
            {
                ThrowBizException("NetPay_ForceCheckReturnAmt");
            }
            if (isForceCheck && Math.Abs(refundEntity.ToleranceAmt ?? 0) > 0.1M)
            {
                ThrowBizException("NetPay_ForceCheckToleranceAmt");
            }

            #endregion 强制核收数据检查
        }

        #region [For SO Domain]

        /// <summary>
        /// 是否存在待审核的网上支付记录
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        public virtual bool IsExistOriginalBySOSysNo(int soSysNo)
        {
            return m_NetpayDA.IsExistOriginalBySOSysNo(soSysNo);
        }

        /// <summary>
        /// 拆分NetPay时更新NetPay
        /// </summary>
        /// <param name="entity"></param>
        public virtual void SplitForSO(NetPayInfo entity)
        {
            if (!entity.SysNo.HasValue)
            {
                throw new ArgumentNullException("entity.SysNo");
            }
            m_NetpayDA.UpdateStatusSplitForSO(entity);
        }

        /// <summary>
        /// 作废拆分NetPay
        /// </summary>
        /// <param name="master">主单信息</param>
        /// <param name="subList">子单列表</param>
        /// <param name="externalKey">externalKey</param>
        public virtual void AbandonSplitForSO(SOBaseInfo master, List<SOBaseInfo> subList, string externalKey)
        {
            m_NetpayDA.AbandonSplitForSO(master, subList, externalKey);
        }

        public virtual void AuditNetPay4GroupBuy(int netPaySysNo)
        {

            //BizLogger.WriteLog<NetPayEntity>(inputNetPay, "[团购]核对网上支付",
            //    string.Format("Begin:[团购]用户\"{0}\"开始审核", inputNetPay.ApproveUserSysNo.Value, inputNetPay.SOSysNo), inputNetPay.SOSysNo.ToString(), (int)LogType.Finance_NetPay_Verify_GroupBuy);
            int userSysNo = ServiceContext.Current.UserSysNo;
            var netPayInfo = LoadBySysNo(netPaySysNo);
            var so = ExternalDomainBroker.GetSOBaseInfo(netPayInfo.SOSysNo.Value);
            #region PreCheck

            if (so.Status == SOStatus.Abandon)
            {
                ThrowBizException("NetPay_Audit_SOAlreadyAbandon", so.SysNo);
            }
            if (netPayInfo.Status != NetPayStatus.Origin)
            {
                ThrowBizException("NetPay_StatusNotAllowCheck");
            }

            //验证订单的付款方式是不是“网上支付”
            //if (!CommonService.IsNetPayType(so.PayTypeSysNo, so.SysNo))
            //{
            //    throw new BusinessException(MessageResource.NetPay_SOPayTypeIsNotNetCode, MessageResource.NetPay_SOPayTypeIsNotNetValue);
            //}          

            if (netPayInfo.PayAmount < so.ReceivableAmount)
            {
                ThrowBizException("NetPay_PayAmoutNotEnoughNotAllowCheck");
            }
            #endregion

            #region Prepare data
            //SOIncomeInfo soIncome = new SOIncomeInfo();
            //soIncome.CompanyCode = netPayInfo.CompanyCode;
            //soIncome.IncomeStyle = SOIncomeOrderStyle.Advanced;
            //soIncome.OrderType = SOIncomeOrderType.SO;
            //soIncome.OrderSysNo = netPayInfo.SOSysNo;
            //soIncome.OrderAmt = so.SOTotalAmount;
            //soIncome.IncomeAmt = netPayInfo.PayAmount;
            //soIncome.PrepayAmt = Math.Max(0.00M, so.PrepayAmount??0M);//???
            ////soIncome.IncomeUserSysNo = userSysNo;
            //soIncome.Status = SOIncomeStatus.Origin;
            ////soIncome.MasterSoSysNo = so.;
            //soIncome.PointPay = so.PointPay;
            //soIncome.GiftCardPayAmt = so.GiftCardPay;
            //soIncome.PayAmount = netPayInfo.PayAmount;

            //是否需要生成多付退款单
            SOIncomeRefundInfo refund = null;
            if (netPayInfo.PayAmount > so.ReceivableAmount)
            {
                //构造退款信息
                refund = new SOIncomeRefundInfo();
                refund.CompanyCode = netPayInfo.CompanyCode;
                //netPayInfo.ReturnAmt = netPayInfo.PayAmount - so.ReceivableAmount;
                refund.RefundCashAmt = netPayInfo.PayAmount - so.ReceivableAmount;

                //Jack:目前没有这个信息
                //if (netPayInfo.ToleranceAmt.HasValue)
                //{
                //    refund.ToleranceAmt = Math.Abs(netPay.ToleranceAmt.Value);
                //}

                //CRL18174:团购订单退款调整,团购订单退款需视不同的支付方式，采用不同的退款类型
                //todo:需要Check 如果团购订单的退款类型为空，则默认为“退入余额帐户”方式
                refund.RefundPayType = RefundPayType.PrepayRefund;

                refund.RefundPoint = 0;
                //todo:jack需要确认一下
                refund.CreateUserSysNo = userSysNo;
                refund.RefundReason = 5;//客户多付款
                refund.HaveAutoRMA = false;//非物流拒收
                refund.Note = "团购成团后的多付款退款";
                refund.Status = RefundStatus.Origin;
                refund.OrderType = RefundOrderType.OverPayment;
                refund.SOSysNo = netPayInfo.SOSysNo;
                refund.OrderSysNo = netPayInfo.SOSysNo;
            }

            #endregion

            //#region PreCheck
            //m_soIncomeBL.PreCheckForCreate(soIncome);
            //if (refund != null)
            //{
            //    m_soIncomeRefundBL.PreCheckCreate(refund);
            //}
            //#endregion PreCheck

            #region Action
            SOIncomeInfo soIncomeInfo = null;
            TransactionOptions options = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromMinutes(2)
            };
            try
            {
                using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, options))
                {
                    //Step 1:向SOIncome写入一条收款记录
                    soIncomeInfo = CreateSOIncomeInfo(netPayInfo, so);

                    //Step 2:写退款信息
                    if (refund != null)
                    {
                        refund.SOIncomeSysNo = soIncomeInfo.SysNo;
                        refund.Status = RefundStatus.Origin;
                        ObjectFactory<SOIncomeRefundProcessor>.Instance.Create(refund);
                    }
                    //Step 3:更新NetPay记录
                    m_NetpayDA.UpdateApproveInfo(netPaySysNo, NetPayStatus.Approved);

                    ts.Complete();
                }
            }
            catch
            {
                //Do not skip out
                //Log
            }

            #endregion

            //记录业务Log
            //BizLogger.WriteLog<NetPayEntity>(netPay, LogCategories.核对网上支付.ToString(),
            //    string.Format("End:[团购]用户\"{0}\"审核了编号为\"{1}\"网上支付单", netPay.OperationUserUniqueName, netPay.SysNo), netPay.SysNo.ToString(), (int)LogType.Finance_NetPay_Verify);
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                GetMessageString("NetPay_Log_Audit", ServiceContext.Current.UserSysNo, netPaySysNo)
                , ECCentral.BizEntity.Common.BizLogType.Finance_NetPay_Verify
                , soIncomeInfo.SysNo.Value
                , soIncomeInfo.CompanyCode);

        }
        #endregion [For SO Domain]

        #region Private Helper Methods

        /// <summary>
        /// 构造销售收款单信息
        /// </summary>
        private SOIncomeInfo CreateSOIncomeInfo(NetPayInfo netpayInfo, SOBaseInfo soBaseInfo)
        {
            var soIncomeInfo = new SOIncomeInfo()
            {
                IncomeStyle = SOIncomeOrderStyle.Advanced,
                OrderType = SOIncomeOrderType.SO,
                OrderSysNo = netpayInfo.SOSysNo,
                OrderAmt = soBaseInfo.SOTotalAmount,
                IncomeAmt = netpayInfo.PayAmount,
                PrepayAmt = soBaseInfo.PrepayAmount,
                Status = SOIncomeStatus.Origin,
                MasterSoSysNo = soBaseInfo.SOSplitMaster,
                PointPay = soBaseInfo.PointPay / decimal.Parse(AppSettingManager.GetSetting("Invoice", "PointExChangeRate")),
                GiftCardPayAmt = soBaseInfo.GiftCardPay,
                PayAmount = netpayInfo.PayAmount,
                CompanyCode = soBaseInfo.CompanyCode //取订单上的CompanyCode
            };
            //SOIncomeProcessor检查，如果已经存在收款单记录，则不允许创建
            return ObjectFactory<SOIncomeProcessor>.Instance.Create(soIncomeInfo);
        }

        /// <summary>
        /// 强制核收时创建多付退款单
        /// </summary>
        /// <param name="netpayEntity">支付信息实体</param>
        /// <param name="refundEntity">退款信息实体</param>
        private void CreateRefundInfoForForceCheck(NetPayInfo netpayEntity, SOIncomeRefundInfo refundEntity)
        {
            refundEntity.RefundPoint = 0;
            refundEntity.RefundReason = 5;     //客户多付款
            refundEntity.HaveAutoRMA = false;  //非物流拒收
            refundEntity.Status = RefundStatus.Origin;
            refundEntity.OrderType = RefundOrderType.OverPayment;
            refundEntity.SOSysNo = netpayEntity.SOSysNo;
            refundEntity.OrderSysNo = netpayEntity.SOSysNo;

            //查询是否已经存在退款信息
            var refundList = ObjectFactory<SOIncomeRefundProcessor>.Instance.GetListByCriteria(new SOIncomeRefundInfo()
            {
                OrderSysNo = netpayEntity.SOSysNo.Value,
                OrderType = RefundOrderType.OverPayment
            });

            if (refundList != null && refundList.Count > 0)
            {
                refundEntity.SysNo = refundList[0].SysNo;
                ObjectFactory<SOIncomeRefundProcessor>.Instance.Update(refundEntity);
            }
            else
            {
                refundEntity.PayAmount = netpayEntity.PayAmount;
                ObjectFactory<SOIncomeRefundProcessor>.Instance.Create(refundEntity);
            }
        }

        /// <summary>
        /// 判断支付方式是否是网上支付
        /// </summary>
        /// <param name="payTypeSysNo"></param>
        /// <returns></returns>
        private bool IsNetPayType(int payTypeSysNo, int soSysNo)
        {
            var payTypes = ExternalDomainBroker.GetPayTypeList();
            foreach (var item in payTypes)
            {
                if (item.SysNo == payTypeSysNo)
                {
                    return item.IsNet.Value;
                }
            }
            throw new BizException(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.NetPay, "NetPay_OrderPayTypeInvalid"));
        }

        private void ThrowBizException(string msgKeyName, params object[] args)
        {
            throw new BizException(GetMessageString(msgKeyName, args));
        }

        private string GetMessageString(string msgKeyName, params object[] args)
        {
            return string.Format(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.NetPay, msgKeyName), args);
        }

        #endregion Private Helper Methods
    }
}