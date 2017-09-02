using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using ECCentral.BizEntity;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.SO;
using ECCentral.Service.RMA.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.RMA.BizProcessor
{
    [VersionExport(typeof(RequestProcessor))]
    public class RequestProcessor
    {
        private IRequestDA requestDA = ObjectFactory<IRequestDA>.Instance;
        private IRegisterDA registerDA = ObjectFactory<IRegisterDA>.Instance;
        private ICustomerContactDA customerContactDA = ObjectFactory<ICustomerContactDA>.Instance;
        private RegisterProcessor registerProcessor = ObjectFactory<RegisterProcessor>.Instance;
        private string ozzShip = AppSettingManager.GetSetting("RMA", "PushShipTypeName"); // 上门取件物流  "OZZO奥硕物流";

        /// <summary>
        /// 创建申请单
        /// </summary>
        /// <param name="request">申请单信息</param>
        /// <returns>申请单信息</returns>
        public virtual RMARequestInfo Create(RMARequestInfo request)
        {
            request.VerifyForCreate();

            if (request.ShipViaCode.Trim() == ozzShip)
            {
                //奥硕上门地点判断
                //ValidOZZOFetchToHome(request);
            }

            var refundItems = ObjectFactory<IRefundDA>.Instance.GetMasterByOrderSysNo(request.SOSysNo.Value);
            if (refundItems != null)
            {
                refundItems.ForEach(p =>
                {
                    if (p.Status == RMARefundStatus.Refunded && p.RefundReason.HasValue && p.RefundReason == 2)
                    {
                        throw new BizException(ResouceManager.GetMessageString("RMA.Request", "ExsitsNotCreateRequest"));
                    }
                });
            }

            SOInfo so = ExternalDomainBroker.GetSOInfo(request.SOSysNo.Value);

            if (so.BaseInfo.Status.Value != SOStatus.OutStock && so.BaseInfo.Status.Value != SOStatus.Shipping && so.BaseInfo.Status != SOStatus.Complete && so.BaseInfo.Status != SOStatus.Reported)
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.Request", "OutStockNotCreateRequest"));
            }

            if (so.BaseInfo.SOType == SOType.ElectronicCard || so.BaseInfo.SOType == SOType.PhysicalCard)
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.Request", "GiftCardNoRMA"));
            }
            if (so.BaseInfo.PayTypeSysNo == 203)
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.Request", "RedeemGiftCertificatesNoRMA"));
            }


            if (so.BaseInfo.PayTypeSysNo.HasValue)
            {
                int t;

                if (int.TryParse(AppSettingManager.GetSetting(SOConst.DomainName, "GiftVoucher_PayTypeSysNo"), out t))
                {
                    if (so.BaseInfo.PayTypeSysNo.Value == t)
                    {
                        throw new BizException(ResouceManager.GetMessageString("RMA.Request", "GiftCertificatesNoRMA"));
                    }
                }
            }


            CustomerContactInfo customerContactInfoEntity = GetCustomerContactInfo(request);

            BizEntity.Invoice.DeliveryType shipType = BizEntity.Invoice.DeliveryType.SELF;
            InvoiceType? invoceType = InvoiceType.SELF;
            StockType? stockType = StockType.SELF;
            int merchantSysNo = 1;

            if (so != null)
            {
                shipType = so.ShippingInfo.ShippingType;
                invoceType = so.InvoiceInfo.InvoiceType;
                stockType = so.ShippingInfo.StockType;
                merchantSysNo = so.BaseInfo.Merchant.MerchantID ?? 0;
            }

            using (TransactionScope tran = new TransactionScope())
            {
                request.SysNo = CreateSysNo();
                request.RequestID = GenerateId(request.SysNo.Value);
                //创建申请单初始状态为【待审核】 add by norton 2012.11.21
                request.Status = RMARequestStatus.WaitingAudit;
                request.IsSubmit = true;
                request.ShippingType = shipType;
                request.InvoiceType = invoceType;
                request.StockType = stockType;
                request.MerchantSysNo = merchantSysNo;
                requestDA.Create(request);

                customerContactInfoEntity.RMARequestSysNo = request.SysNo;

                customerContactInfoEntity.ReceiveCellPhone = so.ReceiverInfo.MobilePhone;
                customerContactInfoEntity.ReceiveZip = so.ReceiverInfo.Zip;

                ObjectFactory<CustomerContactProcessor>.Instance.Create(customerContactInfoEntity);

                request.Registers.ForEach(register =>
                {
                    register.SysNo = registerDA.CreateSysNo();
                    register.BasicInfo.Status = RMARequestStatus.Origin;
                    register.BasicInfo.OwnBy = RMAOwnBy.Origin;
                    register.BasicInfo.Location = RMALocation.Origin;
                    register.BasicInfo.IsWithin7Days = false;
                    register.CheckInfo.IsRecommendRefund = false;
                    register.RevertInfo.NewProductStatus = RMANewProductStatus.Origin;
                    register.BasicInfo.NextHandler = RMANextHandler.RMA;
                    register.CompanyCode = request.CompanyCode;

                    SOItemInfo item = so.Items.FirstOrDefault(p => p.ProductSysNo == register.BasicInfo.ProductSysNo.Value);
                    register.BasicInfo.ShippedWarehouse = item.StockSysNo.ToString();

                    register.VerifyCreate();

                    registerDA.Create(register);

                    registerDA.InsertRequestItem(request.SysNo.Value, register.SysNo.Value);

                    //创建成功后，发送邮件
                    //if (request.VerifyForSendCreateEmail())
                    //{
                    //    var customer = ExternalDomainBroker.GetCustomerInfo(request.CustomerSysNo.Value);
                    //    SendCreateEmail(customer.BasicInfo.Email, request.RequestID);
                    //}
                });

                tran.Complete();
            }

            ExternalDomainBroker.CreateOperationLog("RMA_Request_Create", BizLogType.RMA_Request_Create, request.SysNo.Value, request.CompanyCode);

            return request;
        }

        /// <summary>
        /// 验证奥硕是否支持上门取件
        /// </summary>
        /// <param name="areaSysNo">申请地区编号</param>
        //void ValidOZZOFetchToHome(RMARequestInfo request)
        //{
        //    //根据行政区划判断是否可以上门取件，如不支持进行提示，且不能保存。
        //    int? areaSysNo = request.AreaSysNo;
        //    bool isFetch = true;

        //    var ozzoShipTypeAreasSysNo = new int[] { 21, 65, 370 };

        //    //获取不支持的地区的列表
        //    var unFetchAreaList = ObjectFactory<IBizInteract.ICommonBizInteract>.Instance.QueryShipAreaUnByAreaSysNo(ozzoShipTypeAreasSysNo, areaSysNo.Value);
        //    // QueryModelProxy.CommonDataQueryProvider.QueryShipAreaUnByAreaSysNo(ozzoShipTypeAreasSysNo, areaSysNo.Value);
        //    if (unFetchAreaList.Count > 0)
        //    {
        //        isFetch = false;
        //        //判断逻辑为：如果有一种配送点可支持上门取件，那判断将终止
        //        foreach (int shipTypeAreaSysNo in ozzoShipTypeAreasSysNo)
        //        {
        //            //可配送的终止条件为配送点没有在不支持地区中配置
        //            if (unFetchAreaList.Count(p => p.ShipTypeSysNo == shipTypeAreaSysNo) == 0)
        //            {
        //                isFetch = true;
        //                break;
        //            }
        //        }
        //    }

        //    //不支持抛出异常
        //    if (!isFetch)
        //    {
        //        throw new BizException("本地区不支持上门取件服务！");
        //    }

        //}


        /// <summary>
        /// 更新申请单（包括单件信息）
        /// </summary>
        /// <param name="request">申请单信息</param>
        public virtual void Update(RMARequestInfo request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            if (!request.SysNo.HasValue)
            {
                throw new ArgumentNullException("request.SysNo");
            }
            request.VerifyUpdate(LoadBySysNo(request.SysNo.Value));

            if (request.ShipViaCode.Trim() == ozzShip)
            {
                //ValidOZZOFetchToHome(request);

                #region [时间限制]

                //if (!request.ETakeDate.HasValue)
                //{
                //    request.ETakeDate = request.CustomerSendTime;
                //}
                //else
                //{
                //    request.CustomerSendTime = request.ETakeDate;
                //}
                //DateTime minReceiveDate = DateTime.Now;
                //DateTime tmpDate = DateTime.Now;
                ////周一四点后
                //if (request.CreateDate.Value.DayOfWeek == DayOfWeek.Monday && request.CreateDate.Value.Hour >= 16)
                //{

                //    tmpDate = request.CreateDate.Value.AddDays(2);
                //    minReceiveDate = new DateTime(tmpDate.Year, tmpDate.Month, tmpDate.Day, 0, 0, 0);
                //    if (request.CustomerSendTime.Value < minReceiveDate)
                //    {

                //        throw new BizException("周一下午四点之后至周二下午四点前申请，预约取货时间最早可选周三取件！");
                //    }

                //}
                ////周二四点前
                //else if (request.CreateDate.Value.DayOfWeek == DayOfWeek.Tuesday && request.CreateDate.Value.Hour < 16)
                //{

                //    tmpDate = request.CreateDate.Value.AddHours(25 - request.CreateDate.Value.Hour);
                //    minReceiveDate = new DateTime(tmpDate.Year, tmpDate.Month, tmpDate.Day, 0, 0, 0);
                //    if (request.CustomerSendTime.Value < minReceiveDate)
                //    {
                //        throw new BizException("周一下午四点之后至周二下午四点前申请，预约取货时间最早可选周三取件！");
                //    }
                //}
                ////周二四点后
                //else if (request.CreateDate.Value.DayOfWeek == DayOfWeek.Tuesday && request.CreateDate.Value.Hour >= 16)
                //{

                //    tmpDate = request.CreateDate.Value.AddDays(2);
                //    minReceiveDate = new DateTime(tmpDate.Year, tmpDate.Month, tmpDate.Day, 0, 0, 0);
                //    if (request.CustomerSendTime.Value < minReceiveDate)
                //    {
                //        throw new BizException("周二下午四点后至周三下午四点前申请，预约取货时间最早可选周四取件！");
                //    }

                //}
                ////周三四点前
                //else if (request.CreateDate.Value.DayOfWeek == DayOfWeek.Wednesday && request.CreateDate.Value.Hour < 16)
                //{

                //    tmpDate = request.CreateDate.Value.AddHours(25 - request.CreateDate.Value.Hour);
                //    minReceiveDate = new DateTime(tmpDate.Year, tmpDate.Month, tmpDate.Day, 0, 0, 0);
                //    if (request.CustomerSendTime.Value < minReceiveDate)
                //    {
                //        throw new BizException("周二下午四点后至周三下午四点前申请，预约取货时间最早可选周四取件！");
                //    }
                //}
                ////周三四点后
                //else if (request.CreateDate.Value.DayOfWeek == DayOfWeek.Wednesday && request.CreateDate.Value.Hour >= 16)
                //{

                //    tmpDate = request.CreateDate.Value.AddDays(2);
                //    minReceiveDate = new DateTime(tmpDate.Year, tmpDate.Month, tmpDate.Day, 0, 0, 0);
                //    if (request.CustomerSendTime.Value < minReceiveDate)
                //    {
                //        throw new BizException("周三下午四点后至周四下午四点前申请，预约取货时间最早可选周五取件！");

                //    }

                //}
                ////周四四点前Thursday
                //else if (request.CreateDate.Value.DayOfWeek == DayOfWeek.Thursday && request.CreateDate.Value.Hour < 16)
                //{

                //    tmpDate = request.CreateDate.Value.AddHours(25 - request.CreateDate.Value.Hour);
                //    minReceiveDate = new DateTime(tmpDate.Year, tmpDate.Month, tmpDate.Day, 0, 0, 0);
                //    if (request.CustomerSendTime.Value < minReceiveDate)
                //    {
                //        throw new BizException("周三下午四点后至周四下午四点前申请，预约取货时间最早可选周五取件！");
                //    }
                //}
                ////周四四点后
                //else if (request.CreateDate.Value.DayOfWeek == DayOfWeek.Thursday && request.CreateDate.Value.Hour >= 16)
                //{

                //    tmpDate = request.CreateDate.Value.AddDays(2);
                //    minReceiveDate = new DateTime(tmpDate.Year, tmpDate.Month, tmpDate.Day, 0, 0, 0);
                //    if (request.CustomerSendTime.Value < minReceiveDate)
                //    {
                //        throw new BizException("周四下午四点后至周五下午四点前申请，预约取货时间最早可选周六取件！");
                //    }

                //}
                ////周五四点前
                //else if (request.CreateDate.Value.DayOfWeek == DayOfWeek.Friday && request.CreateDate.Value.Hour < 16)
                //{

                //    tmpDate = request.CreateDate.Value.AddHours(25 - request.CreateDate.Value.Hour);
                //    minReceiveDate = new DateTime(tmpDate.Year, tmpDate.Month, tmpDate.Day, 0, 0, 0);
                //    if (request.CustomerSendTime.Value < minReceiveDate)
                //    {
                //        throw new BizException("周四下午四点后至周五下午四点前申请，预约取货时间最早可选周六取件！");
                //    }
                //}
                ////周五四点后
                //else if (request.CreateDate.Value.DayOfWeek == DayOfWeek.Friday && request.CreateDate.Value.Hour >= 16)
                //{

                //    tmpDate = request.CreateDate.Value.AddDays(4);
                //    minReceiveDate = new DateTime(tmpDate.Year, tmpDate.Month, tmpDate.Day, 0, 0, 0);
                //    if (request.CustomerSendTime.Value < minReceiveDate)
                //    {

                //        throw new BizException("周五下午四点之后至周一下午四点前申请，预约取货时间最早可选下周二取件！");
                //    }
                //}
                ////周六全天
                //else if (request.CreateDate.Value.DayOfWeek == DayOfWeek.Saturday)
                //{

                //    tmpDate = request.CreateDate.Value.AddDays(3);
                //    minReceiveDate = new DateTime(tmpDate.Year, tmpDate.Month, tmpDate.Day, 0, 0, 0);
                //    if (request.CustomerSendTime.Value < minReceiveDate)
                //    {
                //        throw new BizException("周五下午四点之后至周一下午四点前申请，预约取货时间最早可选下周二取件！");
                //    }
                //}
                ////周日全天
                //else if (request.CreateDate.Value.DayOfWeek == DayOfWeek.Sunday)
                //{
                //    tmpDate = request.CreateDate.Value.AddDays(2);
                //    minReceiveDate = new DateTime(tmpDate.Year, tmpDate.Month, tmpDate.Day, 0, 0, 0);
                //    if (request.CustomerSendTime.Value < minReceiveDate)
                //    {
                //        throw new BizException("周五下午四点之后至周一下午四点前申请，预约取货时间最早可选下周二取件！");
                //    }
                //}
                ////周一四点前
                //else if (request.CreateDate.Value.DayOfWeek == DayOfWeek.Monday && request.CreateDate.Value.Hour < 16)
                //{

                //    tmpDate = request.CreateDate.Value.AddHours(25 - request.CreateDate.Value.Hour);
                //    minReceiveDate = new DateTime(tmpDate.Year, tmpDate.Month, tmpDate.Day, 0, 0, 0);
                //    if (request.CustomerSendTime.Value < minReceiveDate)
                //    {
                //        throw new BizException("周五下午四点之后至周一下午四点前申请，预约取货时间最早可选下周二取件！");
                //    }
                //}

                #endregion
            }

            if (request.CreateDate.Value.Year > request.CustomerSendTime.Value.Year
               || (request.CreateDate.Value.Year == request.CustomerSendTime.Value.Year && (request.CreateDate.Value.DayOfYear - request.CustomerSendTime.Value.DayOfYear) > 7))
            {
                throw new BizException(ResouceManager.GetMessageString("RMA.Request", "OrderTimeNotPreviousWeek"));
            }


            request.Registers = registerProcessor.LoadByRequestSysNo(request.SysNo.Value);

            CustomerContactInfo customerContactInfo = GetCustomerContactInfo(request);

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                customerContactDA.UpdateByRequestSysNo(customerContactInfo);

                bool? isWithin7Days = null;
                if (request.CustomerSendTime.HasValue)
                {
                    isWithin7Days = IsWithin7Days(request.SOSysNo.Value, request.CustomerSendTime.Value);
                }
                request.Registers.ForEach(reg =>
                {
                    reg.BasicInfo.IsWithin7Days = isWithin7Days;
                });

                UpdateWithRegisters(request);

                scope.Complete();
            }

            ExternalDomainBroker.CreateOperationLog("RMA_Request_Update", BizLogType.RMA_Request_Update, request.SysNo.Value, request.CompanyCode);
        }

        /// <summary>
        /// 更新申请单主信息（不包括单件）
        /// </summary>
        /// <param name="request">申请单信息</param>
        public virtual void UpdateWithoutRegisters(RMARequestInfo request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            if (!request.SysNo.HasValue)
            {
                throw new ArgumentNullException("request.SysNo");
            }

            request.VerifyUpdate(LoadBySysNo(request.SysNo.Value));

            var customerContactInfo = GetCustomerContactInfo(request);

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                ObjectFactory<ICustomerContactDA>.Instance.UpdateByRequestSysNo(customerContactInfo);

                requestDA.Update(request);

                scope.Complete();
            }
        }

        /// <summary>
        /// 作废申请单
        /// </summary>
        /// <param name="sysNo">申请单编号</param>
        /// <returns>是否成功</returns>
        public virtual bool Abandon(int sysNo)
        {
            RMARequestInfo request = LoadBySysNo(sysNo);

            request.VerifyAbandon(request);

            request.Status = RMARequestStatus.Abandon;
            request.Registers = registerProcessor.LoadByRequestSysNo(sysNo);
            request.Registers.ForEach(reg => { reg.BasicInfo.Status = request.Status; });

            bool result;

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                result = UpdateWithRegisters(request);

                scope.Complete();
            }

            ExternalDomainBroker.CreateOperationLog("RMA_Request_Abandon", BizLogType.RMA_Request_Abandon, request.SysNo.Value, request.CompanyCode);

            return result;
        }

        /// <summary>
        /// 接收申请单
        /// </summary>
        /// <param name="request">申请单信息</param>
        /// <returns>申请单信息</returns>
        public virtual RMARequestInfo Receive(RMARequestInfo request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request is required!");
            }
            if (!request.SysNo.HasValue)
            {
                throw new ArgumentNullException("request.SysNo is required!");
            }

            RMARequestInfo original = LoadWithRegistersBySysNo(request.SysNo.Value);

            request.VerifyReceive(original);

            original.ReceiveWarehouse = request.ReceiveWarehouse;
            original.ReceiveTime = DateTime.Now;
            original.ReceiveUserSysNo = ServiceContext.Current.UserSysNo;
            //获取UI上需要的接收人名称
            original.ReceiveUserName = ExternalDomainBroker.GetUserInfoBySysNo(original.ReceiveUserSysNo.Value);
            original.IsViaPostOffice = request.IsViaPostOffice;
            original.PostageToPoint = request.PostageToPoint;
            original.Status = RMARequestStatus.Handling;
            original.CustomerSendTime = request.CustomerSendTime;
            original.ShipViaCode = request.ShipViaCode;
            original.IsRejectRMA = request.IsRejectRMA;
            original.TrackingNumber = request.TrackingNumber;
            original.Phone = request.Phone;
            original.Contact = request.Contact;
            original.Address = request.Address;
            original.ETakeDate = request.ETakeDate;
            original.AreaSysNo = request.AreaSysNo;
            original.Memo = request.Memo;
            original.Note = request.Note;
            original.IsReceiveMsg = request.IsReceiveMsg;

            request.ReturnPoint = request.PostageToPoint;

            // data format:
            // { RegisterSysNo: [ HaveInvoice, IsFullAccessory, IsFullPackage ] }
            Dictionary<int, bool?[]> dic = new Dictionary<int, bool?[]>();
            request.Registers.ForEach(reg =>
            {
                dic.Add(
                    reg.BasicInfo.SysNo.Value,
                    new bool?[] { reg.BasicInfo.IsHaveInvoice, reg.BasicInfo.IsFullAccessory, reg.BasicInfo.IsFullPackage }
                );
            });

            Dictionary<int, decimal> costs = GetSOItemsCost(request.SysNo.Value);
            bool isWithin7Days = IsWithin7Days(request.SOSysNo.Value, request.CustomerSendTime.Value);
            original.Registers.ForEach(reg =>
            {
                reg.BasicInfo.OwnBy = RMAOwnBy.Customer;
                reg.BasicInfo.Location = RMALocation.Self;
                reg.BasicInfo.LocationWarehouse = request.ReceiveWarehouse;
                reg.BasicInfo.OwnByWarehouse = ((int)RMAOwnBy.Origin).ToString();
                reg.BasicInfo.Cost = costs[reg.BasicInfo.ProductSysNo.Value];
                reg.BasicInfo.IsWithin7Days = isWithin7Days;
                reg.BasicInfo.Status = request.Status;
                reg.BasicInfo.IsHaveInvoice = dic[reg.BasicInfo.SysNo.Value][0];
                reg.BasicInfo.IsFullAccessory = dic[reg.BasicInfo.SysNo.Value][1];
                reg.BasicInfo.IsFullPackage = dic[reg.BasicInfo.SysNo.Value][2];
            });

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                UpdateWithRegisters(original);

                UpdateInventory(original.Registers, true);

                scope.Complete();
            }

            var customer = ExternalDomainBroker.GetCustomerInfo(request.CustomerSysNo.Value);

            //SendEmail(customer.BasicInfo.Email, request.RequestID);

            ExternalDomainBroker.CreateOperationLog("RMA_Request_Recieve", BizLogType.RMA_Request_Receive, request.SysNo.Value, original.CompanyCode);

            return original;
        }

        /// <summary>
        /// 取消接收申请单
        /// </summary>
        /// <param name="sysNo">系统编号</param>
        /// <returns>是否成功</returns>
        public virtual bool CancelReceive(int sysNo)
        {
            RMARequestInfo request = LoadWithRegistersBySysNo(sysNo);
            request.VerifyCancelReceive();

            using (TransactionScope tran = new TransactionScope())
            {
                //[Jay]:OZZO奥硕物流上门取返修件或返还50点积分 2010-01-28
                //VoidTransferPostage2Point(request);

                request.Status = RMARequestStatus.Origin;
                request.ReceiveWarehouse = null;
                request.ReceiveTime = null;
                request.ReceiveUserSysNo = null;
                request.IsViaPostOffice = null;
                request.PostageToPoint = null;
                request.ReturnPoint = null;
                request.Registers = registerProcessor.LoadByRequestSysNo(request.SysNo.Value);

                // 先更新 RMA Inventory
                // 更新后再重置 LocationWarehouse
                UpdateInventory(request.Registers, false);

                request.Registers.ForEach(reg =>
                {
                    reg.BasicInfo.OwnBy = RMAOwnBy.Origin;
                    reg.BasicInfo.Location = RMALocation.Origin;
                    reg.BasicInfo.Cost = null;
                    reg.BasicInfo.Status = request.Status;
                    reg.BasicInfo.LocationWarehouse = ((int)RMALocation.Origin).ToString();
                    reg.BasicInfo.RefundStatus = null;
                    reg.RevertInfo.RevertStatus = null;
                    reg.BasicInfo.OutBoundStatus = null;
                    reg.BasicInfo.ReturnStatus = null;
                });

                requestDA.Update(request);

                request.Registers.ForEach(reg =>
                {
                    registerProcessor.Update(reg);
                });

                tran.Complete();
            }

            ExternalDomainBroker.CreateOperationLog("RMA_Request_CancelRecieve", BizLogType.RMA_Request_CancelReceive, request.SysNo.Value, request.CompanyCode);
            return true;
        }

        /// <summary>
        /// 关闭申请单
        /// </summary>
        /// <param name="sysNo">系统编号</param>
        /// <returns>是否成功</returns>
        public virtual bool Close(int sysNo)
        {
            RMARequestInfo request = LoadBySysNo(sysNo);
            request.Status = RMARequestStatus.Complete;
            request.Registers = registerProcessor.LoadByRequestSysNo(request.SysNo.Value);

            request.Registers.ForEach(register =>
            {
                register.BasicInfo.CloseTime = DateTime.Now;
                register.BasicInfo.CloseUserSysNo = ServiceContext.Current.UserSysNo;
            });

            bool result;
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                result = UpdateWithRegisters(request);

                scope.Complete();
            }

            ExternalDomainBroker.CreateOperationLog("RMA_Request_Close", BizLogType.RMA_Request_Close, request.SysNo.Value, request.CompanyCode);

            return result;
        }

        /// <summary>
        /// 更新申请单Lable打印信息
        /// </summary>
        /// <param name="sysNo">系统编号</param>
        /// <returns>是否成功</returns>
        public virtual bool PrintLables(int sysNo)
        {
            bool result = requestDA.PrintLabels(sysNo);

            RMARequestInfo request = LoadBySysNo(sysNo);

            ExternalDomainBroker.CreateOperationLog("RMA_Request_PrintLabels", BizLogType.RMA_Request_PrintLabels, sysNo, request.CompanyCode);

            return result;
        }

        /// <summary>
        /// 获取申请单的业务模式
        /// </summary>
        /// <param name="request">申请单信息</param>
        /// <returns>业务模式描述信息，比如：(泰隆优选销售，泰隆优选仓储，泰隆优选配送，商家：泰隆优选网)</returns>
        public virtual string GetBusinessModel(RMARequestInfo request)
        {
            string businessType = string.Empty;
            if (!string.IsNullOrEmpty(GetBusinessTypeString(request.InvoiceType)))
            {
                //{0}销售
                businessType += string.Format(ResouceManager.GetMessageString("RMA.Request", "InvoiceTypeFormat"), GetBusinessTypeString(request.InvoiceType));
            }
            if (!string.IsNullOrEmpty(GetBusinessTypeString(request.StockType)))
            {
                //{0}仓储
                businessType += string.Format(ResouceManager.GetMessageString("RMA.Request", "StockTypeFormat"), GetBusinessTypeString(request.StockType));
            }

            if (!string.IsNullOrEmpty(GetBusinessTypeString(request.ShippingType)))
            {
                //{0}配送
                businessType += string.Format(ResouceManager.GetMessageString("RMA.Request", "ShippingTypeFormat"), GetBusinessTypeString(request.ShippingType));
            }

            string vendorName = string.Empty;

            ///为符合IPP逻辑而改
            ///ipp中 NEG订单时MerchantSysNo=0, 不用加载商家信息
            ///改之前代码：if (request.MerchantSysNo.HasValue)
            ///改之后代码如下：
            if (request.MerchantSysNo.HasValue && request.MerchantSysNo != 0)
            {
                //获取Merchant的信息
                var merchantInfo = ExternalDomainBroker.GetVendorFinanceInfoByVendorSysNo(request.MerchantSysNo.Value);
                if (!string.IsNullOrEmpty(merchantInfo.VendorBasicInfo.VendorBriefName))
                {
                    //商家:{0}
                    vendorName = string.Format(ResouceManager.GetMessageString("RMA.Request", "SellerTypeFormat"), merchantInfo.VendorBasicInfo.VendorBriefName);
                }
            }
            if (request.InvoiceType.HasValue && request.StockType.HasValue && request.ShippingType.HasValue)
            {
                if (request.InvoiceType == InvoiceType.SELF && request.StockType == BizEntity.Invoice.StockType.SELF && request.ShippingType == BizEntity.Invoice.DeliveryType.SELF)
                {
                    //商家:{0}
                    vendorName = string.Format(ResouceManager.GetMessageString("RMA.Request", "SellerTypeFormat"), ResouceManager.GetMessageString("RMA.Request", "SellerType_Self"));
                }
            }
            return businessType + vendorName;
        }

        /// <summary>
        /// 判断申请单是否存在
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <returns>是/否</returns>
        public virtual bool IsRMARequestExists(int soSysNo)
        {
            return requestDA.IsRMARequestExists(soSysNo);
        }

        /// <summary>
        /// 根据系统编号获取申请单以及对应的单件详细信息
        /// </summary>
        /// <param name="sysNo">系统编号</param>
        /// <returns>申请单详细信息</returns>
        public virtual RMARequestInfo LoadWithRegistersBySysNo(int sysNo)
        {
            RMARequestInfo result = ObjectFactory<IRequestDA>.Instance.LoadWithRegistersBySysNo(sysNo);
            if (result == null)
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "RequestNotExists");
                msg = string.Format(msg, sysNo);
                throw new BizException(msg);
            }
            return result;
        }

        /// <summary>
        /// 根据系统编号获取申请单信息（不包括单件信息）
        /// </summary>
        /// <param name="sysNo">系统编号</param>
        /// <returns>单件信息</returns>
        public virtual RMARequestInfo LoadBySysNo(int sysNo)
        {
            var result = ObjectFactory<IRequestDA>.Instance.LoadBySysNo(sysNo);
            if (result == null)
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "RequestNotExists");
                msg = string.Format(msg, sysNo);
                throw new BizException(msg);
            }
            return result;
        }

        /// <summary>
        /// 根据单件系统编号获取申请单信息
        /// </summary>
        /// <param name="registerSysNo">单件系统编号</param>
        /// <returns>申请单信息</returns>
        public virtual RMARequestInfo LoadByRegisterSysNo(int registerSysNo)
        {
            return requestDA.LoadByRegisterSysNo(registerSysNo);
        }

        /// <summary>
        /// 根据SO系统编号获取申请单列表信息
        /// </summary>
        /// <param name="soSysNo">SO系统编号</param>
        /// <returns>申请单列表</returns>
        public virtual List<RMARequestInfo> LoadRequestBySOSysNo(int soSysNo)
        {
            var requests = requestDA.LoadRequestBySOSysNo(soSysNo);
            if (requests != null)
            {

            }
            return null;
        }

        public virtual RMARequestInfo Adjust(int sysNo)
        {
            string ServiceCode = requestDA.CreateServiceCode();
            if (string.IsNullOrEmpty(ServiceCode) || ServiceCode.Length != 6)
            {
                throw new ArgumentNullException(ResouceManager.GetMessageString("RMA.Request", "ServiceCodeError"));
            }
            else
            {

                RMARequestInfo request = LoadBySysNo(sysNo);
                request.VerifyAuditPassed(request);
                request.ServiceCode = ServiceCode;
                request.Status = RMARequestStatus.Origin;
                request.Registers = registerProcessor.LoadByRequestSysNo(request.SysNo.Value);
                request.AuditTime = DateTime.Now;
                request.AuditUserSysNo = ServiceContext.Current.UserSysNo;
                bool result = UpdateWithRegisters(request);

                if (request.IsReceiveMsg == true && !string.IsNullOrEmpty(request.Phone)
                    && request.ShippingType == BizEntity.Invoice.DeliveryType.SELF && request.StockType == StockType.SELF && request.InvoiceType == InvoiceType.SELF)
                {
                    string message = string.Format(ResouceManager.GetMessageString("RMA.Request", "SMSAuditRMARequest"), request.RequestID, request.ServiceCode);
                    ExternalDomainBroker.SendSMS(request.Phone, message);
                }

                if (result)
                {
                    ExternalDomainBroker.CreateOperationLog("RMA_Request_Audit", BizLogType.RMA_Request_Audit, request.SysNo.Value, request.CompanyCode);
                }
                return request;
            }
        }

        public virtual RMARequestInfo Refused(int sysNo)
        {
            RMARequestInfo request = LoadBySysNo(sysNo);
            request.VerifyAuditRefuesed(request);

            request.Status = RMARequestStatus.AuditRefuesed;
            request.Registers = registerProcessor.LoadByRequestSysNo(request.SysNo.Value);

            bool result = UpdateWithRegisters(request);

            if (result)
            {
                ExternalDomainBroker.CreateOperationLog("RMA_Request_Refused", BizLogType.RMA_Request_Refused, request.SysNo.Value, request.CompanyCode);
            }
            return request;
        }



        #region Private Methods

        private void SendEmail(string email, string requestid)
        {
            if (StringUtility.IsNullOrEmpty(email) || StringUtility.IsNullOrEmpty(requestid))
            {
                return;
            }

            KeyValueVariables vars = new KeyValueVariables();
            vars.Add("RequestID", requestid);

            EmailHelper.SendEmailByTemplate(email, "RMARequest_Receive", vars);
        }

        private void SendCreateEmail(string email, string requestId)
        {
            if (StringUtility.IsNullOrEmpty(email) || StringUtility.IsNullOrEmpty(requestId))
                return;
            KeyValueVariables vars = new KeyValueVariables();
            vars.Add("RequestID", requestId);

            EmailHelper.SendEmailByTemplate(email, "RMARequest_Create", vars);
        }

        private string GetBusinessTypeString(InvoiceType? model)
        {
            string result = string.Empty;
            if (model == InvoiceType.MET)
            {
                result = ResouceManager.GetMessageString("RMA.Request", "BusinessType_MET");
            }
            else if (model == InvoiceType.SELF)
            {
                result = ResouceManager.GetMessageString("RMA.Request", "BusinessType_Self");
            }
            return result;
        }

        private string GetBusinessTypeString(BizEntity.Invoice.DeliveryType? model)
        {
            string result = string.Empty;
            if (model == BizEntity.Invoice.DeliveryType.MET)
            {
                result = ResouceManager.GetMessageString("RMA.Request", "BusinessType_MET");
            }
            else if (model == BizEntity.Invoice.DeliveryType.SELF)
            {
                result = ResouceManager.GetMessageString("RMA.Request", "BusinessType_Self");
            }
            return result;
        }

        private string GetBusinessTypeString(BizEntity.Invoice.StockType? model)
        {
            string result = string.Empty;
            if (model == BizEntity.Invoice.StockType.MET)
            {
                result = ResouceManager.GetMessageString("RMA.Request", "BusinessType_MET");
            }
            else if (model == BizEntity.Invoice.StockType.SELF)
            {
                result = ResouceManager.GetMessageString("RMA.Request", "BusinessType_Self");
            }
            return result;
        }

        /// <summary>
        /// 获取商品的成本价
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        private Dictionary<int, decimal> GetSOItemsCost(int requestSysNo)
        {
            Dictionary<int, decimal> result = new Dictionary<int, decimal>();
            RMARequestInfo request = LoadWithRegistersBySysNo(requestSysNo);
            if (request != null)
            {
                var soItems = ExternalDomainBroker.GetSOItemList(request.SOSysNo.Value);
                request.Registers.ForEach(p =>
                {
                    var item = soItems.FirstOrDefault(q => q.ProductSysNo == p.BasicInfo.ProductSysNo);
                    if (item != null && !result.Keys.Contains(p.BasicInfo.ProductSysNo.Value))
                    {
                        result.Add(p.BasicInfo.ProductSysNo.Value, item.CostPrice ?? 0);
                    }
                });
            }
            return result;
        }

        private int CreateSysNo()
        {
            return requestDA.CreateSysNo();
        }

        private string GenerateId(int sysNo)
        {
            return string.Format("R0{0}", sysNo.ToString().PadLeft(8, '0'));
        }

        /// <summary>
        /// 更新Request和Register信息
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private bool UpdateWithRegisters(RMARequestInfo request)
        {
            requestDA.Update(request);

            if (request.Registers != null)
            {
                if (request.Status == RMARequestStatus.AuditRefuesed)
                {
                    request.Registers.ForEach(
                                           reg =>
                                           {
                                               reg.BasicInfo.Status = RMARequestStatus.Abandon;
                                               registerProcessor.Update(reg);
                                           }
                                       );
                }
                else
                {
                    request.Registers.ForEach(
                        reg =>
                        {
                            reg.BasicInfo.Status = request.Status;
                            registerProcessor.Update(reg);
                        }
                    );
                }
            }

            return true;
        }

        private bool IsWithin7Days(int soSysNo, DateTime customerSendTime)
        {
            //SOInfo so = ExternalDomainBroker.GetSOInfo(soSysNo);
            //if (so == null)
            //{
            //    throw new BizException(ResouceManager.GetMessageString("RMA.Request", "SONotExsits"));
            //}
            return customerSendTime.AddDays(-7) < DateTime.Now;
        }

        private int TrimIntNull(Object obj)
        {
            if (obj is System.DBNull || obj == null)
            {
                return -999999;
            }
            else
            {
                return Int32.Parse(obj.ToString());
            }
        }

        private bool UpdateInventory(List<RMARegisterInfo> registers, bool isRecv)
        {
            registers.ForEach(r =>
            {
                if (r.BasicInfo.SOItemType != SOProductType.ExtendWarranty)
                {
                    var InventoryMemo = string.Empty;
                    var tempInventoryMemo = requestDA.GetInventoryMemo(TrimIntNull(r.BasicInfo.LocationWarehouse),
                        r.BasicInfo.ProductSysNo,
                        r.CompanyCode);

                    registerDA.UpdateInventory(
                        Convert.ToInt32(r.BasicInfo.LocationWarehouse), r.BasicInfo.ProductSysNo.Value, isRecv, r.CompanyCode
                    );

                    RMAInventoryLog rmaInventoryLogEntity = new RMAInventoryLog();

                    rmaInventoryLogEntity.WarehouseSysNo = TrimIntNull(r.BasicInfo.LocationWarehouse);
                    rmaInventoryLogEntity.ProductSysNo = TrimIntNull(r.BasicInfo.ProductSysNo);
                    rmaInventoryLogEntity.SysNo = TrimIntNull(r.BasicInfo.SysNo);

                    rmaInventoryLogEntity.RMAOnVendorQty = 0;
                    rmaInventoryLogEntity.ShiftQty = 0;
                    rmaInventoryLogEntity.OwnbyNeweggQty = 0;
                    rmaInventoryLogEntity.OperationTime = DateTime.Now;

                    if (isRecv)
                    {
                        rmaInventoryLogEntity.OperationType = ResouceManager.GetMessageString("RMA.Request", "RMARequestMsgOne");
                        rmaInventoryLogEntity.RMAStockQty = 1;
                        rmaInventoryLogEntity.OwnbyCustomerQty = 1;
                        InventoryMemo = ResouceManager.GetMessageString("RMA.Request", "RMARequestMsgTwo") + tempInventoryMemo;
                    }
                    else
                    {
                        rmaInventoryLogEntity.OperationType = ResouceManager.GetMessageString("RMA.Request", "RMARequestMsgThree");
                        rmaInventoryLogEntity.RMAStockQty = -1;
                        rmaInventoryLogEntity.OwnbyCustomerQty = -1;
                        InventoryMemo = ResouceManager.GetMessageString("RMA.Request", "RMARequestMsgFour") + tempInventoryMemo;
                    }

                    rmaInventoryLogEntity.Memo = InventoryMemo;
                    requestDA.InsertRMAInventoryLog(rmaInventoryLogEntity);
                }
            });
            return true;
        }

        private CustomerContactInfo GetCustomerContactInfo(RMARequestInfo request)
        {
            if (request == null)
            {
                return null;
            }
            CustomerContactInfo customerContactInfo = new CustomerContactInfo();
            customerContactInfo.ReceiveAddress = request.Address;
            customerContactInfo.ReceiveAreaSysNo = request.AreaSysNo;
            customerContactInfo.ReceiveCellPhone = request.Phone;
            customerContactInfo.ReceiveContact = request.Contact;
            customerContactInfo.ReceiveName = request.Contact;
            customerContactInfo.ReceivePhone = request.Phone;
            customerContactInfo.RMARequestSysNo = request.SysNo;

            return customerContactInfo;
        }

        private void VoidTransferPostage2Point(RMARequestInfo request)
        {
            var customerRank = ExternalDomainBroker.GetCustomerRank(request.CustomerSysNo.Value);

            TriStatus? stat = null;

            DateTime oldMaxReceiveTime;

            if (!string.IsNullOrEmpty(AppSettingManager.GetSetting("RMA", "OldMaxReceiveTime")))
            {
                oldMaxReceiveTime = DateTime.Parse(AppSettingManager.GetSetting("RMA", "OldMaxReceiveTime"));
            }
            else
            {
                oldMaxReceiveTime = DateTime.Parse("2010-2-23 10:00:00");
            }

            if (request.ReceiveTime < oldMaxReceiveTime && (int)customerRank >= (int)CustomerRank.Golden)
            {
                request.PostageToPoint = 50;
                if (!(request.ShipViaCode.ToLower().Contains("ozzo") ||
                      request.ShipViaCode.ToLower().Contains("奥硕")))
                {
                    int point = 0 - request.PostageToPoint.Value;
                    AdjustPointRequest adjustInfo = new AdjustPointRequest();
                    adjustInfo.CustomerSysNo = request.CustomerSysNo.Value;
                    adjustInfo.Point = point;
                    adjustInfo.PointType = (int)AdjustPointType.RMAPostageManuToPoints;
                    adjustInfo.Source = "RMA";
                    adjustInfo.Memo = ResouceManager.GetMessageString("RMA.Request", "AdjustInfoMemo");
                    adjustInfo.OperationType = AdjustPointOperationType.Abandon;
                    adjustInfo.SOSysNo = request.SysNo;
                    ExternalDomainBroker.AdjustPoint(adjustInfo);
                }
            }
            else if (request.ShipViaCode == RMAConst.ShipVia_PostOffice
                && request.PostageToPoint.HasValue
                && request.PostageToPoint.Value > 0)
            {
                try
                {
                    int re = ExternalDomainBroker.GetCustomerPointAddRequestStatus(request.SysNo.Value);
                    stat = (TriStatus)re;
                }
                catch (BizException e)
                {
                    //邮资转积分有可能关闭，此时将没有调整记录
                    if (string.Compare(e.Message, "Cannot find any matched AdjustPointRequest") == 0)
                    {
                        return;
                    }
                    else
                    {
                        throw e;
                    }
                }
                if (stat != null && stat.Value == TriStatus.Origin)
                {
                    ExternalDomainBroker.AbandonAdjustPointRequest(request.SysNo.Value);
                }
            }
        }

        #endregion
    }
}
