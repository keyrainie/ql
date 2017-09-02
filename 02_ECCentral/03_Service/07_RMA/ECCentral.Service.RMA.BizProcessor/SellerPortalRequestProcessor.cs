using System;
using System.Data;
using System.Diagnostics;
using System.Transactions;

using ECCentral.BizEntity.RMA;
using ECCentral.Service.RMA.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using System.Collections.Generic;

namespace ECCentral.Service.RMA.BizProcessor
{
    [VersionExport(typeof(SellerPortalRequestProcessor))]
    public class SellerPortalRequestProcessor
    {
        private const string TableName_RMA_Request_Sequence = "[IPP3].[dbo].[RMA_Request_Sequence]";
        private const string TableName_RMA_Register_Sequence = "[IPP3].[dbo].[RMA_Register_Sequence]";
        private static readonly int AutoRMAUserSysNo = 0;

        static SellerPortalRequestProcessor()
        {
            int.TryParse(AppSettingManager.GetSetting("RMA", "AutoRMAUserSysNo"), out AutoRMAUserSysNo);
        }

        private IRegisterDA registerDA = ObjectFactory<IRegisterDA>.Instance;
        private IRequestDA requestDA = ObjectFactory<IRequestDA>.Instance;
        private IRMAForSellerPortalDA rmaSellerPortalDA = ObjectFactory<IRMAForSellerPortalDA>.Instance;

        public void CreateRequest4AutoRMA(int soSysNo,string inUser)
        {
            //0. Load Data
            SOInfo soInfo = ExternalDomainBroker.GetSOInfo(soSysNo);

            //1. Validation
            ValidateSOInfo(soInfo,soSysNo);
            ValidateIfExistValidRMA(soInfo.SysNo.Value);
            ValidateCanCallRequest(soInfo);

            //2. Create Request
            int requestSysNo = 0;
            string requestState = "F";
            int wareHouseNumber = -1;
            int vendorSysNo = GetVendorSysNoFromInUser(inUser);
            int userSysNo = AutoRMAUserSysNo;
            wareHouseNumber = GetWarehouseNumber(soInfo);
            
            string note = string.Empty;
            string memo = string.Empty;
            SellerPortalAutoRMALog sellerPortalAutoRMALog = GetSellerPortalAutoRMALog(soSysNo);
            if (sellerPortalAutoRMALog == null)
            {
                InsertSellerPortalAutoRMALog(soSysNo, inUser);
                sellerPortalAutoRMALog = GetSellerPortalAutoRMALog(soSysNo);
            }

            try
            {
                if (CreateRequest(soSysNo, userSysNo, note, memo, wareHouseNumber, ref requestSysNo))
                {
                    //设置日志状态
                    requestState = "S";
                }
            }
            catch(Exception ex)
            {
                requestState = "F";
                ExceptionHelper.HandleException(ex);
            }
            finally
            {
                UpdateSellerPortalAutoRMALog(soSysNo, requestState, DateTime.Now, sellerPortalAutoRMALog.RefundStatus, sellerPortalAutoRMALog.RefundTime.Value);
                sellerPortalAutoRMALog = GetSellerPortalAutoRMALog(soSysNo);
            }
            //3. Create Refund
            string refundStatus = "F";
            try
            {
                if (ObjectFactory<SellerPortalRefundProcessor>.Instance.CreateRMAAutoRefund(soSysNo, memo, wareHouseNumber, userSysNo, vendorSysNo))
                {
                    refundStatus = "S";
                }
            }
            catch (Exception ex)
            {
                refundStatus = "F";
                ExceptionHelper.HandleException(ex);
            }
            finally
            {
                UpdateSellerPortalAutoRMALog(soSysNo, sellerPortalAutoRMALog.RequestStatus, sellerPortalAutoRMALog.RequestTime.Value, refundStatus, DateTime.Now);
            }
        }

        #region Validation

        /// <summary>
        /// 验证是否已生成申请单
        /// (如果已有申请单，抛业务异常)
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>        
        private void ValidateIfExistValidRMA(int soSysNo)
        {
            bool result = rmaSellerPortalDA.ExistValidRMA(soSysNo);
            if (result)
            {
                string errorMsg = string.Format(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-ValidateIfExistValidRMA"), soSysNo);
                throw new BizException(errorMsg);
            }
        }

        /// <summary>
        /// 验证是否有订单信息
        /// </summary>
        /// <param name="soSysNo"></param>        
        private void ValidateSOInfo(SOInfo soInfo,int soSysNo)
        {
            if (soInfo == null || soInfo.Items == null || soInfo.Items.Count == 0)
            {
                string errorMessage = string.Format(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-OrderNoExists"), soSysNo);
                throw new BizException(errorMessage);
            }

            if (soInfo.BaseInfo.SOType.HasValue)
            {
                if (!soInfo.BaseInfo.Status.HasValue || soInfo.BaseInfo.Status.Value != SOStatus.OutStock)
                {
                    throw new BizException(ResouceManager.GetMessageString("RMA.Refund", "Refund_SOInfoSatatusNoOutStock"));
                }

                if (soInfo.BaseInfo.SOType == SOType.ElectronicCard)
                {
                    throw new BizException(ResouceManager.GetMessageString("RMA.Refund", "Refund_ElectronicCardNoLogistics"));
                }
                else if (soInfo.BaseInfo.SOType == SOType.PhysicalCard)
                {
                    List<GiftCardInfo> cards = ExternalDomainBroker.GetGiftCardInfoBySOSysNo(soInfo.SysNo.Value, GiftCardType.Standard);
                    if (cards == null || cards.Count == 0)
                    {
                        throw new BizException(ResouceManager.GetMessageString("RMA.Refund", "Refund_CardsException"));
                    }
                }
            }

            foreach (var item in soInfo.Items)
            {
                if (item.ProductType.Value == SOProductType.ExtendWarranty)
                {
                    foreach (var item1 in soInfo.Items)
                    {
                        if (item1.ProductType.Value == SOProductType.Product && item1.ProductSysNo.ToString() == item.MasterProductSysNo)
                        {
                            item.StockSysNo = item1.StockSysNo;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 验证是否可以调用物流拒收接口
        /// </summary>
        /// <param name="sosysno"></param>       
        public void ValidateCanCallRequest(SOInfo soInfo)
        {
            string errorMessage = null;
            if (soInfo.ShippingInfo.StockType == BizEntity.Invoice.StockType.SELF && soInfo.ShippingInfo.ShippingType == BizEntity.Invoice.DeliveryType.SELF)
            {
                errorMessage = string.Format(ResouceManager.GetMessageString("RMA.Request", "RegisterCheck-ValidateCanCallRequest"), soInfo.SysNo);

                throw new BizException(errorMessage);
            }
        }

        #endregion

        #region Create Request

        /// <summary>
        /// Buid [RO+SysNo]
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        private string GenerateID(int requestSysNo)
        {
            return "R0" + requestSysNo.ToString().PadLeft(8, '0');
        }

        /// <summary>
        /// 创建申请单于单件
        /// </summary>
        /// <param name="soNumber"></param>
        /// <param name="userID"></param>
        /// <param name="strNote"></param>
        /// <param name="strMemo"></param>
        /// <param name="warehouseNumber"></param>
        /// <param name="refRequestSysNo"></param>
        /// <returns></returns>        
        private bool CreateRequest(int soNumber, int userSysNo, string strNote, string strMemo, int warehouseNumber, ref int refRequestSysNo)
        {
            bool result = false;

            SOInfo so = ExternalDomainBroker.GetSOInfo(soNumber);

            RMARequestInfo request = new RMARequestInfo();

            int requestSysNo = rmaSellerPortalDA.CreateSequence(TableName_RMA_Request_Sequence);

            string strRequestID = GenerateID(requestSysNo);

            request.SysNo = requestSysNo;
            request.RequestID = strRequestID;
            request.SOSysNo = soNumber;
            request.CustomerSysNo = so.BaseInfo.CustomerSysNo;
            request.Address = so.ReceiverInfo.Address;
            request.Contact = so.ReceiverInfo.Name;
            request.Phone = so.ReceiverInfo.Phone;
            request.ReceiveTime = DateTime.Now;
            request.ReceiveUserSysNo = userSysNo;
            request.Note = strNote;
            request.Memo = strMemo;
            request.Status = RMARequestStatus.Handling;
            request.AreaSysNo = so.ReceiverInfo.AreaSysNo;
            request.CustomerSendTime = DateTime.Now;
            request.IsRejectRMA = true;
            request.ReceiveWarehouse = warehouseNumber.ToString();
            request.IsSubmit = true;
            request.ShipViaCode = "退换货自动处理,快递方式缺省";
            refRequestSysNo = requestSysNo;
            request.CompanyCode = so.CompanyCode;

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;

            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                requestDA.Create(request);

#if DEBUG
                if (Transaction.Current != null)
                {
                    Debug.WriteLine(string.Format("LocalIdentifier:[{0}] Status:[{1}]",
                        Transaction.Current.TransactionInformation.LocalIdentifier,
                        Transaction.Current.TransactionInformation.Status.ToString())
                        );
                }
#endif
                foreach (SOItemInfo soItem in so.Items)
                {
                    if (soItem.ProductType == SOProductType.Coupon)
                    {
                        continue;
                    }

                    for (int i = 0; i < soItem.Quantity; i++)
                    {
                        RMARegisterInfo register = new RMARegisterInfo();
                        register.CompanyCode = so.CompanyCode;

                        int registerSysNo = rmaSellerPortalDA.CreateSequence(TableName_RMA_Register_Sequence);

                        register.SysNo = registerSysNo;

                        if (soItem.ProductType == SOProductType.ExtendWarranty)
                        {
                            register.BasicInfo.ProductSysNo = int.Parse(soItem.MasterProductSysNo);
                            register.BasicInfo.OwnBy = RMAOwnBy.Origin;
                            register.BasicInfo.Location = RMALocation.Origin;
                        }
                        else
                        {
                            register.BasicInfo.ProductSysNo = soItem.ProductSysNo;
                            register.BasicInfo.OwnBy = RMAOwnBy.Customer;
                            register.BasicInfo.Location = RMALocation.Self;
                        }

                        register.RequestType = RMARequestType.Return;
                        register.BasicInfo.CustomerDesc = "物流拒收";

                        register.BasicInfo.NextHandler = RMANextHandler.RMA;
                        register.BasicInfo.Status = RMARequestStatus.Handling;
                        register.BasicInfo.IsWithin7Days = true;

                        register.CheckInfo.IsRecommendRefund = true;

                        //HACK:大小写 Cost
                        register.BasicInfo.Cost = soItem.CostPrice;
                        register.BasicInfo.RMAReason = 0;//质量问题
                        register.BasicInfo.CloseUserSysNo = userSysNo;
                        register.BasicInfo.CloseTime = DateTime.Now;
                        register.BasicInfo.IsHaveInvoice = true;
                        register.BasicInfo.IsFullAccessory = true;
                        register.BasicInfo.IsFullPackage = true;
                        //RMA Warehouse
                        register.BasicInfo.LocationWarehouse = warehouseNumber.ToString();
                        register.BasicInfo.OwnByWarehouse = ((int)RMAOwnBy.Origin).ToString();

                        //使用传过来的warehouseNumber
                        register.BasicInfo.ShippedWarehouse = warehouseNumber.ToString();

                        register.BasicInfo.SOItemType = soItem.ProductType;

                        bool insertRegisterResult = registerDA.Create(register);

                        bool inserRequestItemResult = registerDA.InsertRequestItem(requestSysNo, registerSysNo);

                        if (insertRegisterResult && inserRequestItemResult)
                        {
                            result = true;
                        }
                    }
                }
                scope.Complete();
            }

            return result;
        }

        /// <summary>
        /// 获取仓库编号
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private int GetWarehouseNumber(SOInfo soInfo)
        {
            int wareHouseNumber = -10;
            if (
                (soInfo.ShippingInfo.StockType.Value == BizEntity.Invoice.StockType.MET && soInfo.ShippingInfo.ShippingType == BizEntity.Invoice.DeliveryType.SELF && soInfo.InvoiceInfo.InvoiceType.Value == BizEntity.Invoice.InvoiceType.MET)
                || (soInfo.ShippingInfo.StockType.Value == BizEntity.Invoice.StockType.MET && soInfo.ShippingInfo.ShippingType == BizEntity.Invoice.DeliveryType.SELF && soInfo.InvoiceInfo.InvoiceType.Value == BizEntity.Invoice.InvoiceType.SELF)
                )
            {
                wareHouseNumber = 99;
            }
            else if (
                 (soInfo.ShippingInfo.StockType.Value == BizEntity.Invoice.StockType.MET && soInfo.ShippingInfo.ShippingType == BizEntity.Invoice.DeliveryType.MET && soInfo.InvoiceInfo.InvoiceType.Value == BizEntity.Invoice.InvoiceType.MET)
                || (soInfo.ShippingInfo.StockType.Value == BizEntity.Invoice.StockType.MET && soInfo.ShippingInfo.ShippingType == BizEntity.Invoice.DeliveryType.MET && soInfo.InvoiceInfo.InvoiceType.Value == BizEntity.Invoice.InvoiceType.SELF)
                )
            {
                wareHouseNumber = 90;
            }

            return wareHouseNumber;
        }

        /// <summary>
        /// 获取 VendorSysNo
        /// </summary>
        /// <param name="inUser"></param>
        /// <returns></returns>
        private int GetVendorSysNoFromInUser(string inUser)
        {

            int VendorSysNo = -1;
            if (!string.IsNullOrEmpty(inUser))
            {
                string[] array = inUser.Split('/');
                if (array != null && array.Length == 5)
                {
                    int.TryParse(array[3], out VendorSysNo);
                }
            }
            return VendorSysNo;
        }
        #endregion

        #region AutoRMALog

        /// <summary>
        /// 插入物流拒收申请单日志
        /// </summary>
        ///<param name="soSysNo"></param>
        ///<param name="inUser"></param>
        private void InsertSellerPortalAutoRMALog(int soSysNo, string inUser)
        {
            rmaSellerPortalDA.InsertSellerPortalAutoRMALog(soSysNo, inUser);

        }

        /// <summary>
        /// 更新物流拒收申请单日志状态
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="requestState"></param>
        /// <param name="requestTime"></param>
        /// <param name="refundState"></param>
        /// <param name="refundTime"></param>
        private void UpdateSellerPortalAutoRMALog(int soSysNo, string requestState, DateTime requestTime, string refundState, DateTime refundTime)
        {
            rmaSellerPortalDA.UpdateSellerPortalAutoRMALog(soSysNo, requestState, requestTime, refundState, refundTime);
        }

        /// <summary>
        /// 获取申请单日志
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        private SellerPortalAutoRMALog GetSellerPortalAutoRMALog(int soSysNo)
        {
            return rmaSellerPortalDA.GetSellerPortalAutoRMALog(soSysNo);
        }
        #endregion      
    }
}