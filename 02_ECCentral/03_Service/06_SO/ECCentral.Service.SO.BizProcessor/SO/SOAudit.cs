using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using ECCentral.BizEntity.SO;
using ECCentral.Service.SO.IDataAccess;
using ECCentral.Service.Utility;
using System.IO;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.ExternalSYS;
//using ECCentral.Service.ThirdPart.Interface;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.SO.BizProcessor
{
    #region 订单审核

    /// <summary>
    /// 审核订单。
    /// 属性 Parameter 说明:
    /// Parameter[0] : bool , 表示是否强制审核订单,默认为false；
    /// Parameter[1] : bool , 表示是否是主管审核,默认为false；
    /// Parameter[2] : bool , 表示是否要审核网上支付,默认为false；
    /// </summary>
    [VersionExport(typeof(SOAction), new string[] { "General", "Audit" })]
    public class SOAudit : SOAction
    {
        #region Parameter参数
        /* Parameter说明:
         * Parameter[0] : bool , 是否强制审核订单
         * Parameter[1] : bool , 是否是主管审核
         * Parameter[2] : bool , 是否要审核网上支付
         *
         *
         */
        /// <summary>
        /// 是否强制审核,由 Parameter[0] 传入,默认为false.
        /// </summary>
        protected bool IsForce
        {
            get
            {
                return GetParameterByIndex<bool>(0);
            }
        }
        /// <summary>
        /// 是否是主管审核,由 Parameter[1] 传入,默认为false.
        /// </summary>
        protected bool IsManagerAudit
        {
            get
            {
                return GetParameterByIndex<bool>(1);
            }
        }
        /// <summary>
        /// 是否要审核网上支付,由 Parameter[2] 传入,默认为false.
        /// </summary>
        protected bool IsAuditNetPay
        {
            get
            {
                return GetParameterByIndex<bool>(2);
            }
        }

        private ISODA _soDA;
        protected ISODA SODA
        {
            get
            {
                _soDA = _soDA ?? ObjectFactory<ISODA>.Instance;
                return _soDA;
            }
        }
        #endregion
        /// <summary>
        /// 当前订单编号
        /// </summary>
        protected int SOSysNo
        {
            get
            {
                return CurrentSO.SysNo.Value;
            }
        }
        private SOHolder _holder;
        /// <summary>
        /// 当前订单的锁定操作
        /// </summary>
        protected SOHolder Holder
        {
            get
            {
                _holder = _holder ?? ObjectFactory<SOHolder>.Instance;
                _holder.CurrentSO = CurrentSO;
                return _holder;
            }
        }
        private ECCentral.BizEntity.Invoice.NetPayInfo _currentSONetPayInfo;
        protected ECCentral.BizEntity.Invoice.NetPayInfo CurrentSONetPayInfo
        {
            get
            {
                _currentSONetPayInfo = _currentSONetPayInfo ?? ExternalDomainBroker.GetSOValidNetPay(SOSysNo);
                return _currentSONetPayInfo;
            }
        }
        private BizEntity.Invoice.SOIncomeInfo _currentSOIncomeInfo;
        protected BizEntity.Invoice.SOIncomeInfo CurrentSOIncomeInfo
        {
            get
            {
                _currentSOIncomeInfo = _currentSOIncomeInfo ?? ExternalDomainBroker.GetValidSOIncomeInfo(SOSysNo, BizEntity.Invoice.SOIncomeOrderType.SO);
                return _currentSOIncomeInfo;
            }
        }
        public override void Do()
        {
            TransactionOptions option = new TransactionOptions();
            option.IsolationLevel = IsolationLevel.ReadUncommitted;
            option.Timeout = TransactionManager.DefaultTimeout;
            bool flag = false;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, option))
            {
                if (IsManagerAudit)
                {
                    ManagerAudit();
                }
                else
                {
                    Audit();
                }
                this.PublishMessage();

                flag = true;

                scope.Complete();
            }

            //同步送货单逻辑移出到事务外解决跨实例异常 by nolan 
            if (flag)
            {
                SyncSHD();
            }
        }

        protected void WriteLog(string operationName)
        {
            WriteLog(BizEntity.Common.BizLogType.Sale_SO_Audit, operationName);
        }

        #region 审核订单

        private void PreCheckSO()
        {
            //  1.  检查订单是否被锁定，锁定后不能操作

            switch (CurrentSO.BaseInfo.HoldStatus.Value)
            {
                case SOHoldStatus.WebHold:
                    BizExceptionHelper.Throw("SO_Hold_WebHold");
                    break;
                case SOHoldStatus.Processing:
                    BizExceptionHelper.Throw("SO_Hold_Processing");
                    break;
            }
            //  2.  检查订单是否BackOrder，
            if (CurrentSO.BaseInfo.IsBackOrder.HasValue && CurrentSO.BaseInfo.IsBackOrder.Value)
            {
                BizExceptionHelper.Throw("SO_Audit_IsBackOrder");
            }
            //  3.  检查订单状态是否可以被审核
            switch (CurrentSO.BaseInfo.Status.Value)
            {
                case SOStatus.Split:
                    BizExceptionHelper.Throw("SO_Hold_SplitComplete");
                    break;
                case SOStatus.Abandon:
                    BizExceptionHelper.Throw("SO_Hold_Abandoned");
                    break;
                case SOStatus.OutStock:
                    BizExceptionHelper.Throw("SO_Hold_OutStock");
                    break;
            }
        }

        protected virtual void AuditPreCheck()
        {
            PreCheckSO();
            if (CurrentSO.BaseInfo.Status.Value != SOStatus.Origin)
            {
                BizExceptionHelper.Throw("SO_Audit_IsNotOrigin");
            }
        }
        protected void ValidateSONetPay()
        {
            ECCentral.BizEntity.Invoice.NetPayInfo netPayInfo = CurrentSONetPayInfo;
            if (netPayInfo == null)
            {
                BizExceptionHelper.Throw("SO_Audit_NetPayIsNull");
            }
            if (netPayInfo.Status != BizEntity.Invoice.NetPayStatus.Origin)
            {
                BizExceptionHelper.Throw("SO_Audit_NetPayIsAudited");
            }
            if (netPayInfo.Source == BizEntity.Invoice.NetPaySource.Employee)
            {
                BizExceptionHelper.Throw("SO_Audit_ManualAddNetPay");
            }
            if (netPayInfo.PayAmount != CurrentSO.BaseInfo.OriginalReceivableAmount)
            {
                BizExceptionHelper.Throw("SO_Audit_NetPayAmountIsInequality");
            }
            if (netPayInfo.PayTypeSysNo != CurrentSO.BaseInfo.PayTypeSysNo)
            {
                BizExceptionHelper.Throw("SO_Audit_PayTypeIsError");
            }
        }
        protected virtual bool ValidateSOIncome()
        {
            bool result = true;
            if (IsAuditNetPay)
            {
                ValidateSONetPay();
                ExternalDomainBroker.AuditNetPay(CurrentSONetPayInfo.SysNo.Value);
            }
            BizEntity.Invoice.SOIncomeInfo incomeInfo = CurrentSOIncomeInfo;
            if (incomeInfo == null)
            {
                result = false;
                BizExceptionHelper.Throw("SO_Audit_SOIncomeIsNull");
            }
            else
            {
                //Nick.Y.Zheng 金额不相等，并且误差在1分钱以上则不通过，否则通过
                //使用了余额支付或礼品卡支付需要强制审核
                decimal incomeAmount = incomeInfo.IncomeAmt.HasValue ? incomeInfo.IncomeAmt.Value : 0m;
                if (incomeInfo.IncomeAmt != CurrentSO.BaseInfo.ReceivableAmount
                    && Math.Abs(incomeAmount - CurrentSO.BaseInfo.ReceivableAmount) > 0.01m
                    && !IsForce)
                {
                    result = false;
                    //支付金额不足
                    BizExceptionHelper.Throw("SO_Audit_IncomeUnequalSOAmount");
                }
            }
            return result;
        }

        /// <summary>
        /// 验证订单支付状态
        /// </summary>
        /// <param name="isForce"></param>
        /// <returns></returns>
        private SOStatus ValidateSOPayStatus()
        {
            SOStatus status = CurrentSO.BaseInfo.Status.Value;
            BizEntity.Common.PayType ptInfo = ExternalDomainBroker.GetPayTypeBySysNo(CurrentSO.BaseInfo.PayTypeSysNo.Value);

            if (ptInfo == null)
            {
                BizExceptionHelper.Throw("SO_Audit_PayTypeNotExist", CurrentSO.BaseInfo.PayTypeSysNo.ToString());
            }
            else if (CurrentSO.BaseInfo.PayWhenReceived.HasValue && CurrentSO.BaseInfo.PayWhenReceived.Value)
            {
                //货到付款可以直接出库
                status = SOStatus.WaitingOutStock;
            }
            else if (ValidateSOIncome())
            {
                //支付无误，为待出库（电子卡直接完成）
                status = SOStatus.WaitingOutStock;
            }
            return status;
        }

        protected virtual SOStatus GetAuditStatus()
        {
            SOStatus status = CurrentSO.BaseInfo.Status.Value;

            //是批发则转为等待经理审核状态
            if ((CurrentSO.BaseInfo.IsWholeSale.HasValue && CurrentSO.BaseInfo.IsWholeSale.Value))
            {
                status = SOStatus.WaitingManagerAudit;
            }
            else if (CurrentSO.BaseInfo.SplitType == SOSplitType.SubSO) //拆分后子订单
            {
                status = SOStatus.WaitingOutStock;
            }
            else
            {
                status = ValidateSOPayStatus();
            }
            return status;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="isManagerAuditSO"></param>
        public virtual void SendMessage()
        {
            /*
            SOSendMessageProcessor messageProcessor = ObjectFactory<SOSendMessageProcessor>.Instance;
            SOStatus soStatus = CurrentSO.BaseInfo.Status.Value;

            if (soStatus == SOStatus.WaitingOutStock)
            {
                // 给客户发邮件
                messageProcessor.SOAuditedSendEmailToCustomer(CurrentSO);
                // 并单不发消息
                if (!CurrentSO.ShippingInfo.IsCombine.Value)
                {
                    messageProcessor.SendSMS(CurrentSO, BizEntity.Customer.SMSType.OrderAudit);
                }
            }*/
        }

        protected virtual void SaveAudit(SOStatus nextStatus)
        {
            if (nextStatus == CurrentSO.BaseInfo.Status)
            {
                BizExceptionHelper.Throw("SO_Audit_SOIncomeIsNull");
                return;
            }
            SOStatusChangeInfo statusChangeInfo = new SOStatusChangeInfo
            {
                SOSysNo = SOSysNo,
                ChangeTime = DateTime.Now,
                IsSendMailToCustomer = true,
                OldStatus = CurrentSO.BaseInfo.Status,
                OperatorSysNo = ECCentral.Service.Utility.ServiceContext.Current.UserSysNo,
                OperatorType = SOOperatorType.User,
                Status = nextStatus
            };
            //更新审核状态到DB
            bool isUpdateSuccess = SODA.UpdateSOStatusForAudit(statusChangeInfo);
            if (isUpdateSuccess)
            {
                CurrentSO.BaseInfo.Status = statusChangeInfo.Status;
            }
            else
            {
                BizExceptionHelper.Throw("SO_Audit_SOStatusIsError");
            }
        }

        protected virtual void CheckSOIsCombine()
        {
            SODA.UpdateSOCombineInfo(SOSysNo);
        }

        /// <summary>
        /// 根据仓库拆分订单价格
        /// </summary>
        protected virtual void SplitPrice()
        {
            if (CurrentSO.BaseInfo.Status == SOStatus.WaitingOutStock || CurrentSO.BaseInfo.Status == SOStatus.OutStock)
            {
                SOPriceSpliter priceSpliter = new SOPriceSpliter();
                priceSpliter.CurrentSO = CurrentSO;
                priceSpliter.SplitSO();
            }
        }

        private void Unhold()
        {
            if (CurrentSO.BaseInfo.HoldStatus == SOHoldStatus.BackHold)
            {
                SOUnholder Unholder = ObjectFactory<SOUnholder>.Instance;
                Unholder.CurrentSO = CurrentSO;
                Unholder.Unhold(SOUnholder.SOUnholdReason.Audit);
            }
        }
        /// <summary>
        /// 审核订单
        /// </summary>
        public virtual void Audit()
        {

            //  1.  检查订单状态是否可以被审核
            //  2.  检查订单是否被锁定，锁定后不能操作
            //  3.  检查订单否满足审单条件
            AuditPreCheck(); // 1、2、3 整合到一起

            //  4.  计算订单审核后状态
            SOStatus nextStatus = GetAuditStatus();

            if (IsAuditNetPay && nextStatus == SOStatus.WaitingManagerAudit)
            {
                BizExceptionHelper.Throw("SO_AuditNetPay_NeedManagerAudit");
            }

            //  5.  如果是后台锁定了，就先解锁
            Unhold();

            //配送时间的校验
            CheckDeliveryDate();

            //  6.  保存审核结果,如果是电子卡订单，则激活电子卡。
            SaveAudit(nextStatus);

            //  7.  审核订单后重新判断订单是否并单
            CheckSOIsCombine();
            //  8.  根据仓库拆分订单价格
            SplitPrice();
            //  9.  发送邮件
            SendMessage();
            
            //  10. 写日志
            WriteLog("订单审核通过");           

        }
        public void SyncSHD()
        {
            //经gamal确认，款到发货、货到付款订单审核时均生成送货单记录
            //if (CurrentSO.BaseInfo.PayWhenReceived == false)
            //{
            //    return;
            //}
            var soItem = CurrentSO.Items;
            soItem = soItem.Where(p => p.InventoryType == ProductInventoryType.Company || p.InventoryType == ProductInventoryType.TwoDoor).ToList();
            //同步送货单
           
            foreach (var item in soItem)
            {
                //大件一个商品一个送货单。
                ERPSHDInfo erpinfo = new ERPSHDInfo();
                erpinfo.SHDTypeMemo = "送货单";
                erpinfo.RefOrderNo = SOSysNo.ToString();
                erpinfo.RefOrderType = "销售订单";
                erpinfo.SysMemo = erpinfo.RefOrderNo + "/" + erpinfo.RefOrderType;
                erpinfo.ZDR = ServiceContext.Current.UserSysNo;
                erpinfo.ZDSJ = DateTime.Now;
                erpinfo.ZXR = ServiceContext.Current.UserSysNo;
                erpinfo.ZXSJ = DateTime.Now;
                erpinfo.SHDItemList = new List<ERPSHDItem>();
                ERPSHDItem erpitem = new ERPSHDItem();
                erpitem.ProductSysNo = item.ProductSysNo;
                erpitem.SL = item.Quantity;
                erpinfo.SHDItemList.Add(erpitem);
                //ObjectFactory<ISyncERPBizRecord>.Instance.CreateSHD(erpinfo);
            }
           
        }
        /// <summary>
        /// 配送时间的校验
        /// </summary>
        protected virtual void CheckDeliveryDate()
        {
            bool isInvalid = IsInValidDeliveryTime(this.CurrentSO, null, null, IsForce);
            if (isInvalid)
            {
                BizExceptionHelper.Throw("SO_Audit_InvalidDeliveryDate");
            }
        }

        #endregion 审核订单

        #region 主管审核

        protected virtual void ManagerAuditPreCheck()
        {
            PreCheckSO();
            if (CurrentSO.BaseInfo.Status.Value != SOStatus.WaitingManagerAudit)
            {
                BizExceptionHelper.Throw("SO_Audit_StatusIsNotWaittingManagerAudit");
            }
        }

        /// <summary>
        /// 取得主管审核订单后的订单的状态
        /// </summary>
        /// <returns></returns>
        protected virtual SOStatus GetManagerAuditStatus()
        {
            SOStatus status = CurrentSO.BaseInfo.Status.Value;
            if (CurrentSO.BaseInfo.SplitType == SOSplitType.SubSO)
            {
                status = SOStatus.WaitingOutStock;
            }
            else if (CurrentSO.BaseInfo.PayWhenReceived.HasValue && CurrentSO.BaseInfo.PayWhenReceived.Value) //检验支付方式
            {
                //货到付款可以直接出库
                status = SOStatus.WaitingOutStock;
            }
            else if (ValidateSOIncome())
            {
                //支付无误，电子卡直接完成，其他待出库
                status = SOStatus.WaitingOutStock;
            }
            return status;
        }

        /// <summary>
        /// 保存主管审核
        /// </summary>
        /// <param name="nextStatus"></param>
        protected virtual void SaveManagerAudit(SOStatus nextStatus)
        {
            if (CurrentSO.BaseInfo.Status == nextStatus)
            {
                BizExceptionHelper.Throw("SO_Audit_SOIncomeIsNull");
            }
            SOStatusChangeInfo statusChangeInfo = new SOStatusChangeInfo
            {
                SOSysNo = SOSysNo,
                ChangeTime = DateTime.Now,
                IsSendMailToCustomer = true,
                OldStatus = CurrentSO.BaseInfo.Status,
                OperatorSysNo = ECCentral.Service.Utility.ServiceContext.Current.UserSysNo,
                OperatorType = SOOperatorType.User,
                Status = nextStatus
            };
            //更新审核状态到DB
            if (SODA.UpdateSOStatusForManagerAudit(statusChangeInfo))
            {
                CurrentSO.BaseInfo.Status = statusChangeInfo.Status;
            }
            else
            {
                BizExceptionHelper.Throw("SO_ManagerAudit_SOStatusIsError");
            }
        }

        /// <summary>
        /// 主管审核订单
        /// </summary>
        /// <param name="isForce">是否强制审核</param>
        public void ManagerAudit()
        {
            //  1.  检查订单状态是否可以被审核
            //  2.  检查订单是否被锁定，锁定后不能操作
            //  3.  检查订单否满足审单条件
            ManagerAuditPreCheck(); // 1、2、3 整合到一起

            //  4.  计算订单审核后状态
            SOStatus nextStatus = GetManagerAuditStatus();

            //  5.  如果是后台锁定了，就先解锁
            Unhold();

            //  6.  保存审核结果,如果是电子卡订单，则激活电子卡。
            SaveManagerAudit(nextStatus);

            //  7.  相比订单审核，主管审核 少了订单审核中的第7步骤：审核订单后重新判断订单是否并单

            //  8.  根据仓库拆分订单价格
            SplitPrice();

            //  9.  发送邮件
            SendMessage();

            //  10. 写日志
            WriteLog("订单主管审核");
        }

        protected virtual void ManagerAuditSendMessage()
        {
            SendMessage();
            // 如果是主管审核，判断产品价格是否改动，如果改动就发送邮件给相应PM
            if (CurrentSO.BaseInfo.IsWholeSale.Value)
            {
                ObjectFactory<SOSendMessageProcessor>.Instance.PriceChangedSendMail(CurrentSO);
            }
        }

        #endregion 主管审核

        #region 扩展静态方法

        /// <summary>
        /// 将时间区域的字符串转为时间区域集合
        /// </summary>
        /// <param name="timestring">时间区域字符串的集合</param>
        /// <returns>时间区域点</returns>
        internal static List<TimeSpan> ParseTimeSpot(string timestring)
        {
            List<TimeSpan> dtList = new List<TimeSpan>();
            if (!string.IsNullOrEmpty(timestring))
            {
                timestring.Split(',').ForEach(p => dtList.Add(Convert.ToDateTime(p).TimeOfDay));
            }
            return dtList;
        }

        //获取配送时间点
        internal static List<TimeSpan> GetDeliveryTimePointBySO(SOInfo so)
        {
            //获取自动审单检查项信息
            var csTBOrderCheckMasterList = ExternalDomainBroker.GetCSTBOrderCheckMasterList(so.CompanyCode);

            //获取自动审单检查项具体的明细项信息
            var csTBOrderCheckItemList = new List<OrderCheckItem>();
            csTBOrderCheckMasterList.ForEach(p =>
            {
                csTBOrderCheckItemList.AddRange(p.OrderCheckItemList);
            });

            var csItem = csTBOrderCheckItemList.Find(x =>
            {
                return ((x.ReferenceType == "DT11" || x.ReferenceType == "DT12")
                    //&& x.Status == OrderCheckStatus.Invalid
                    && x.ReferenceContent.Contains(so.ShippingInfo.ShipTypeSysNo.ToString()));
            });
            if (csItem == null)
            {
                return new List<TimeSpan>();
            }
            return ParseTimeSpot(csItem.Description);
        }

        /// <summary>
        /// 是否是无效配送时间点
        /// </summary>
        /// <param name="so">订单</param>
        /// <param name="timestring">时间区域字符串的集合</param>
        /// <param name="holidays">所有假期,如果传值为null,将在程序中重新读取</param>
        /// <returns>无效返回真，否则返回假</returns>
        internal static bool IsInValidDeliveryTime(SOInfo so, string timestring, List<Holiday> holidays, bool isForceAudit)
        {
            bool isInvalid = false;

            //强制审核不用效验
            if (isForceAudit)
            {
                return isInvalid;
            }

            //未有配送时间无需判断
            if (!so.ShippingInfo.DeliveryDate.HasValue)
            {
                return isInvalid;
            }

            //订单类型团购不用判断
            if (so.BaseInfo.SOType == SOType.GroupBuy)
            {
                return isInvalid;
            }

            //配送信息
            var shipType = ExternalDomainBroker.GetShippingTypeBySysNo(so.ShippingInfo.ShipTypeSysNo.Value);

            //配送类型是0,3的不检查
            //只检查1和2
            if (shipType.DeliveryType != ShipDeliveryType.OneDayOnce
                && shipType.DeliveryType != ShipDeliveryType.OneDayTwice)
                return isInvalid;

            //节假日
            List<Holiday> shipHolidays = null;
            if (holidays == null)
            {
                shipHolidays = ExternalDomainBroker.GetHolidayAfterToday(so.CompanyCode)
                                .FindAll(x => x.ShipTypeSysNo == shipType.SysNo || !x.ShipTypeSysNo.HasValue);
            }
            else
            {
                shipHolidays = holidays.FindAll(x => x.ShipTypeSysNo == shipType.SysNo || !x.ShipTypeSysNo.HasValue);
            }
            //时间节点
            List<TimeSpan> timepoints = null;
            if (string.IsNullOrEmpty(timestring))
            {
                timepoints = GetDeliveryTimePointBySO(so);
            }
            else
            {
                timepoints = ParseTimeSpot(timestring);
            }

            if (timepoints.Count == 0)
            {
                return isInvalid;
            }

            //计算器
            DeliveryIteration calculator = new DeliveryIteration(DateTime.Now, (int)shipType.DeliveryType, shipHolidays, timepoints, shipType.IntervalDays ?? 0, null);

            calculator.Roll();

            DateTime latestDate = calculator.LatestDate;
            int finalSection = calculator.FinalSection;

            //比较
            if (so.ShippingInfo.DeliveryDate < latestDate)
            {
                isInvalid = true;
            }
            else if (so.ShippingInfo.DeliveryDate == latestDate)
            {
                if (so.ShippingInfo.DeliveryTimeRange < finalSection)
                {
                    isInvalid = true;
                }
            }

            return isInvalid;
        }

        internal static void CreateInvoice(SOInfo soInfo)
        {
            int soSysNo = soInfo.SysNo.Value;
            List<SOPriceMasterInfo> soPriceList = ObjectFactory<ISOPriceDA>.Instance.GetSOPriceBySOSysNo(soSysNo);
            if (soPriceList != null)
            {
                soPriceList.RemoveAll(priceInfo =>
                {
                    return priceInfo.Status == SOPriceStatus.Deactivate;
                });
            }

            if (soPriceList == null || soPriceList.Count < 1)
            {
                ObjectFactory<SOSendMessageProcessor>.Instance.ElectronicSOPriceNotExists(soInfo);
                BizExceptionHelper.Throw("SO_CreateInvoice_PriceInfoIsNull");
            }

            List<ECCentral.BizEntity.Invoice.InvoiceMasterInfo> invoiceList = ExternalDomainBroker.GetSOInvoiceMaster(soSysNo);
            if (invoiceList == null || invoiceList.Count < 1)
            {
                SOItemInfo couponItem = soInfo.Items.Find(item => item.ProductType == SOProductType.Coupon);
                int? couponCodeSysNo = couponItem == null ? null : couponItem.ProductSysNo;
                SOPriceMasterInfo priceInfo = soPriceList[0];
                BizEntity.Invoice.InvoiceInfo invoiceInfo = new BizEntity.Invoice.InvoiceInfo();
                invoiceInfo.InvoiceTransactionInfoList = new List<BizEntity.Invoice.InvoiceTransactionInfo>();
                invoiceInfo.MasterInfo = new BizEntity.Invoice.InvoiceMasterInfo
                {
                    CustomerID = soInfo.BaseInfo.CustomerID,
                    CustomerSysNo = soInfo.BaseInfo.CustomerSysNo,
                    SONumber = soSysNo,
                    InvoiceDate = priceInfo.InDate,
                    InvoiceAmt = priceInfo.InvoiceAmount,
                    PayTypeSysNo = soInfo.BaseInfo.PayTypeSysNo,
                    PayTypeName = "",
                    RMANumber = 0,
                    OriginalInvoiceNumber = 0,
                    InvoiceMemo = null,
                    ShippingCharge = priceInfo.ShipPrice,
                    StockSysNo = priceInfo.StockSysNo,
                    OrderDate = soInfo.BaseInfo.CreateTime,
                    DeliveryDate = soInfo.ShippingInfo.DeliveryDate,
                    SalesManSysNo = soInfo.BaseInfo.SalesManSysNo,
                    IsWholeSale = soInfo.BaseInfo.IsWholeSale,
                    IsPremium = soInfo.BaseInfo.IsPremium,
                    PremiumAmt = priceInfo.PremiumAmount,
                    ShipTypeSysNo = soInfo.ShippingInfo.ShipTypeSysNo,
                    ExtraAmt = priceInfo.PayPrice,
                    SOAmt = priceInfo.SOAmount,
                    DiscountAmt = priceInfo.PromotionAmount,
                    GainPoint = priceInfo.GainPoint,
                    PointPaid = -priceInfo.PointPayAmount,
                    PrepayAmt = -priceInfo.PrepayAmount,
                    PromotionAmt = priceInfo.CouponAmount,
                    ReceiveAreaSysNo = soInfo.ReceiverInfo.AreaSysNo,
                    ReceiveContact = soInfo.ReceiverInfo.Name,
                    ReceiveAddress = soInfo.ReceiverInfo.Address,
                    ReceiveCellPhone = soInfo.ReceiverInfo.MobilePhone,
                    ReceivePhone = soInfo.ReceiverInfo.Phone,
                    ReceiveZip = soInfo.ReceiverInfo.Zip,
                    ReceiveName = soInfo.InvoiceInfo.Header,
                    GiftCardPayAmt = -priceInfo.GiftCardPay,
                    InvoiceNo = null,
                    InvoiceType = soInfo.InvoiceInfo.InvoiceType,
                    MerchantSysNo = soInfo.Merchant.SysNo,
                    CompanyCode = soInfo.CompanyCode,
                    PromotionCustomerSysNo = soInfo.BaseInfo.CustomerSysNo,
                    PromotionCodeSysNo = couponCodeSysNo,
                    IsUseChequesPay = false,//soInfo.BaseInfo.IsUseChequesPay,
                    CashPaid = priceInfo.CashPay,
                };



                priceInfo.Items.ForEach(item =>
                {
                    SOItemInfo soItem = soInfo.Items.Find(i =>
                    {
                        return i.ProductSysNo == item.ProductSysNo && i.ProductType == item.ProductType;
                    });
                    string itemCode = soItem == null ? String.Empty : soItem.ProductID;
                    switch (item.ProductType.Value)
                    {
                        case SOProductType.Coupon:
                            itemCode = String.Format("Promot-{0}", item.ProductSysNo);
                            break;
                        case SOProductType.ExtendWarranty:
                            itemCode = String.Format("{0}E", itemCode);
                            break;
                    }
                    invoiceInfo.InvoiceTransactionInfoList.Add(new BizEntity.Invoice.InvoiceTransactionInfo
                    {
                        ItemCode = itemCode,
                        PrintDescription = item.ProductType.Value.ToEnumDesc(),
                        ItemType = item.ProductType,
                        UnitPrice = item.Price,
                        Quantity = item.Quantity,
                        ExtendPrice = item.Price * item.Quantity,
                        ReferenceSONumber = soSysNo,
                        Weight = soItem.Weight,
                        GainPoint = soItem.GainAveragePoint,
                        PayType = soItem.PayType,
                        PremiumAmt = item.PremiumAmount,
                        ShippingCharge = item.ShipPrice,
                        ExtraAmt = item.PayPrice,
                        CashPaid = item.CashPay,
                        PointPaid = item.PointPayAmount,
                        DiscountAmt = item.PromotionAmount,
                        PrepayAmt = -item.PrepayAmount,
                        Warranty = soItem.Warranty,
                        BriefName = soItem.ProductName,
                        OriginalPrice = soItem.OriginalPrice,
                        PromotionDiscount = item.CouponAmount,
                        MasterProductSysNo = soItem.MasterProductSysNo,
                        UnitCost = soItem.CostPrice,
                        CompanyCode = soInfo.CompanyCode,
                        GiftCardPayAmt = -item.GiftCardPay,
                        UnitCostWithoutTax = soItem.NoTaxCostPrice,
                        ProductSysNo = soItem.ProductSysNo,
                        PriceType = soItem.PriceType,
                        ItemDescription = item.ProductType.Value.ToEnumDesc(),
                    });
                });

                ExternalDomainBroker.CreateInvoice(invoiceInfo);
            }
        }

        #endregion

        protected void PublishMessage()
        {
            EventPublisher.Publish<ECCentral.Service.EventMessage.SO.SOAuditedMessage>(
                new EventMessage.SO.SOAuditedMessage
                {
                    SOSysNo = SOSysNo,
                    AuditedUserName = ServiceContext.Current.UserDisplayName,
                    AuditedUserSysNo = ServiceContext.Current.UserSysNo,
                    CustomerSysNo = CurrentSO.BaseInfo.CustomerSysNo.Value,
                    MasterSOSysNo = CurrentSO.BaseInfo.SplitType == SOSplitType.SubSO ? CurrentSO.BaseInfo.SOSplitMaster.GetValueOrDefault() : SOSysNo,
                    MerchantSysNo = CurrentSO.BaseInfo.Merchant.SysNo.GetValueOrDefault(),
                    OrderAmount = CurrentSO.BaseInfo.SOTotalAmount,
                    OrderTime = CurrentSO.BaseInfo.CreateTime.Value
                });
        }
    }

    /// <summary>
    /// 审核实物卡订单。
    /// 属性 Parameter 说明:
    /// Parameter[0] : bool , 表示是否强制审核订单,默认为false；
    /// Parameter[1] : bool , 表示是否是主管审核,默认为false；
    /// Parameter[2] : bool , 表示是否要审核网上支付,默认为false；
    /// </summary>
    [VersionExport(typeof(SOAction), new string[] { "PhysicalCard", "Audit" })]
    public class PhysicalCardSOAudit : SOAudit
    {
        /// <summary>
        /// 检查订单的折扣是否大于最大折扣比例
        /// </summary>
        /// <returns></returns>
        protected virtual bool PromotionDiscountMoreThanLimit()
        {
            //计算订单的总促销折扣（不包括优惠券折扣）
            decimal promotionDiscountAmount = CurrentSO.Items.Sum<SOItemInfo>(item =>
            {
                return -item.PromotionAmount.Value;//折扣是负数，这里转成正数
            });
            //计算订单的总金额
            decimal totalValue = CurrentSO.Items.Sum<SOItemInfo>(item =>
            {
                return (item.Price.Value * item.Quantity.Value);
            });
            decimal discountRatio = AppSettingHelper.ProductDiscountRatio;

            return (promotionDiscountAmount / totalValue) > discountRatio;
        }

        protected override SOStatus GetAuditStatus()
        {
            SOStatus status = CurrentSO.BaseInfo.Status.Value;
            if (PromotionDiscountMoreThanLimit())
            {
                status = SOStatus.WaitingManagerAudit;
            }
            else
            {
                status = base.GetAuditStatus();
            }
            return status;
        }
    }

    /// <summary>
    /// 电子卡订单审核。
    /// 属性 Parameter 说明:
    /// Parameter[0] : bool , 表示是否强制审核订单,默认为false；
    /// Parameter[1] : bool , 表示是否是主管审核,默认为false；
    /// Parameter[2] : bool , 表示是否要审核网上支付,默认为false；
    /// </summary>
    [VersionExport(typeof(SOAction), new string[] { "ElectronicCard", "Audit" })]
    public class ElectronicCardSOAudit : PhysicalCardSOAudit
    {
        /// <summary>
        /// 电子卡订单完成
        /// </summary>
        protected void ElectronicCardComplete()
        {
            //  1.  激活电子卡
            ExternalDomainBroker.CreateElectronicGiftCard(SOSysNo, CurrentSO.BaseInfo.CustomerSysNo.Value, CurrentSO.Items[0].OriginalPrice.Value, CurrentSO.Items[0].Quantity.Value, CurrentSO.CompanyCode, "");

            //  2.  更新订单的出库时间和订单中商品的出库时间
            DateTime outStockTime = DateTime.Now;
            SODA.UpdateSOOutStockTime(SOSysNo);
            CurrentSO.ShippingInfo.OutTime = outStockTime;
            CurrentSO.Items.ForEach(item =>
            {
                item.IsShippedOut = true;
                item.ShippedOutTime = outStockTime;
            });

            //  3.  发送成功邮件。
            ObjectFactory<SOSendMessageProcessor>.Instance.ActivateElectronicSendMailToCustomer(CurrentSO);
        }

        protected override void SplitPrice()
        {
            base.SplitPrice();
            if (CurrentSO.BaseInfo.Status == SOStatus.OutStock)
            {
                CreateInvoice(CurrentSO);
            }
        }

        #region 订单审核

        protected override SOStatus GetAuditStatus()
        {
            SOStatus status = CurrentSO.BaseInfo.Status.Value;

            if (ValidateSOIncome())
            {
                //支付无误，电子卡直接完成
                status = PromotionDiscountMoreThanLimit() ? SOStatus.WaitingManagerAudit : SOStatus.OutStock;
            }
            return status;
        }

        protected override void SaveAudit(SOStatus nextStatus)
        {
            TransactionOptions options = new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                //更新审核状态到DB
                base.SaveAudit(nextStatus);

                if (CurrentSO.BaseInfo.Status == SOStatus.OutStock)
                {
                    //电子卡订单出库
                    ElectronicCardComplete();
                }
                scope.Complete();
            }
        }

        public override void SendMessage()
        {
            //电子订单不用发送信息，会在电子卡激活的时间发送信息
        }

        #endregion 订单审核

        #region 主管审核

        protected override SOStatus GetManagerAuditStatus()
        {
            SOStatus status = CurrentSO.BaseInfo.Status.Value;
            if (ValidateSOIncome())
            {
                //支付无误，电子卡直接完成
                status = SOStatus.OutStock;
            }
            return status;
        }

        protected override void SaveManagerAudit(SOStatus nextStatus)
        {
            TransactionOptions options = new TransactionOptions
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted
            };
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                //更新审核状态到DB
                base.SaveManagerAudit(nextStatus);

                if (CurrentSO.BaseInfo.Status == SOStatus.OutStock)
                {
                    //电子卡订单出库
                    ElectronicCardComplete();
                }
                scope.Complete();
            }
        }

        #endregion 主管审核
    }

    /// <summary>
    /// 审核订单。
    /// 属性 Parameter 说明:
    /// Parameter[0] : bool , 表示是否强制审核订单,默认为false；
    /// Parameter[1] : bool , 表示是否是主管审核,默认为false；
    /// Parameter[2] : bool , 表示是否要审核网上支付,默认为false；
    /// </summary>
    [VersionExport(typeof(SOAction), new string[] { "Gift", "Audit" })]
    public class GiftSOAudit : SOAudit
    {
        protected override SOStatus GetAuditStatus()
        {
            return SOStatus.WaitingManagerAudit;
        }

        protected override SOStatus GetManagerAuditStatus()
        {
            SOStatus status = CurrentSO.BaseInfo.Status.Value;

            // 创建为 0 的NetPay
            ECCentral.BizEntity.Invoice.NetPayInfo netPayInfo = ExternalDomainBroker.GetSOValidNetPay(SOSysNo);
            //支付方式必须是款到发货，才可创建NetPay
            if (netPayInfo == null && (!CurrentSO.BaseInfo.PayWhenReceived ?? true))
            {
                ExternalDomainBroker.CreatNetPay(SOSysNo, 0, CurrentSO.BaseInfo.PayTypeSysNo.Value, CurrentSO.CompanyCode);
                BizExceptionHelper.Throw("SO_ManagerAudit_GiftSO_CreateNetPay");
            }
            else
            {
                status = base.GetManagerAuditStatus();
            }
            return status;
        }
    }

    /// <summary>
    /// 审核团购订单。
    /// 属性 Parameter 说明:
    /// Parameter[0] : bool , 表示是否强制审核订单,默认为false；
    /// Parameter[1] : bool , 表示是否是主管审核,默认为false；
    /// Parameter[2] : bool , 表示是否要审核网上支付,默认为false；
    /// </summary>
    [VersionExport(typeof(SOAction), new string[] { "GroupBuy", "Audit" })]
    public class GrouBuyAudit : SOAudit
    {
        protected override void AuditPreCheck()
        {
            base.AuditPreCheck();
            //  3.  检查订单否满足审单条件
            if (CurrentSO.BaseInfo.SOType == SOType.GroupBuy)
            {
                //if (!CurrentSO.BaseInfo.SettlementStatus.HasValue)
                //{
                //    BizExceptionHelper.Throw("SO_Audit_GroupBuyNotOver");
                //}
                if (CurrentSO.BaseInfo.SettlementStatus == SettlementStatus.Fail)
                {
                    BizExceptionHelper.Throw("SO_Audit_GroupBuyIsFail");
                }
                else if (CurrentSO.BaseInfo.SettlementStatus == SettlementStatus.PlanFail)
                {
                    string productIDs = string.Empty;
                    int i = 0;
                    CurrentSO.Items.ForEach(item =>
                    {
                        if (item.SettlementStatus == SettlementStatus.PlanFail)
                        {
                            productIDs += i++ > 0 ? string.Format("{0},", item.ProductSysNo) : item.ProductSysNo.ToString();
                        }
                    });
                    BizExceptionHelper.Throw("SO_Audit_GroupBuyProductIsFail", productIDs);
                }
            }
        }
    }

    #endregion 订单审核

    /// <summary>
    /// 取消订单审核
    /// </summary>
    [VersionExport(typeof(SOAction), new string[] { "General", "CancelAudit" })]
    public class SOCancelAudit : SOAction
    {
        private ISODA _soDA;
        protected ISODA SODA
        {
            get
            {
                _soDA = _soDA ?? ObjectFactory<ISODA>.Instance;
                return _soDA;
            }
        }
        /// <summary>
        /// 当前订单编号
        /// </summary>
        protected int SOSysNo
        {
            get
            {
                return CurrentSO.SysNo.Value;
            }
        }
        private SOHolder _holder;
        /// <summary>
        /// 当前订单的锁定操作
        /// </summary>
        private SOHolder Holder
        {
            get
            {
                _holder = _holder ?? ObjectFactory<SOHolder>.Instance;
                _holder.CurrentSO = CurrentSO;
                return _holder;
            }
        }

        public override void Do()
        {
            CancelAudit();
        }

        protected virtual void WriteLog()
        {
            WriteLog(ECCentral.BizEntity.Common.BizLogType.Sale_SO_CancelAudit, "取消审核");
        }

        #region 订单取消审核

        protected virtual void SaveCancelAudit()
        {
            SOStatusChangeInfo statusChangeInfo = new SOStatusChangeInfo
            {
                SOSysNo = SOSysNo,
                ChangeTime = DateTime.Now,
                IsSendMailToCustomer = true,
                OldStatus = CurrentSO.BaseInfo.Status.Value,
                OperatorSysNo = ECCentral.Service.Utility.ServiceContext.Current.UserSysNo,
                OperatorType = ServiceContext.Current.UserSysNo == 0 ? SOOperatorType.System : SOOperatorType.User,
                Status = SOStatus.Origin
            };
            //  2.  保存修改状态
            if (SODA.UpdateSOStatusToOrigin(statusChangeInfo))
            {
                CurrentSO.BaseInfo.Status = SOStatus.Origin;
                CurrentSO.ShippingInfo.FreightUserSysNo = 0;
                CurrentSO.ShippingInfo.AllocatedManSysNo = 0;
            }

            //  3.  写订单修改日志
            WriteLog();
        }

        public void CancelAudit()
        {
            //  1.  业务检查：商家订单不能取消审核；订单状态不是待主管审核/待出库状态，不能取消审核
            if (CurrentSO.ShippingInfo != null && CurrentSO.ShippingInfo.StockType == ECCentral.BizEntity.Invoice.StockType.MET)
            {
                BizExceptionHelper.Throw("SO_CancelAudit_SOIsMETOrder", SOSysNo.ToString());
            }
            SOStatus status = CurrentSO.BaseInfo.Status.Value;
            if (status != SOStatus.WaitingManagerAudit && status != SOStatus.WaitingOutStock)
            {
                BizExceptionHelper.Throw("SO_CancelAudit_SOStatusIsError");
            }

            //  2.  取消审核不管本地是否是锁定都需要去检测仓库是否扫描,这里必须先锁定
            CurrentSO.BaseInfo.HoldReason = "CancelAuditSO";
            CurrentSO.BaseInfo.HoldUser = ECCentral.Service.Utility.ServiceContext.Current.UserSysNo;
            CurrentSO.BaseInfo.HoldTime = DateTime.Now;
            bool isSyn = Holder.Hold(SOHolder.SOHoldReason.CancelAuditOrder, OPCCallBackType.CancelAuditCallBack);

            //  3.  如果是同步锁定订单则执行取消审核操作
            if (isSyn)
            {
                SaveCancelAudit();
            }
            else
            {
                BizExceptionHelper.Throw("SO_CancelAudit_HoldIsAsyn");
            }
        }

        #endregion 订单取消审核

    }
}