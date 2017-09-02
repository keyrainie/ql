using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using ECCentral.BizEntity;
using ECCentral.BizEntity.RMA;
using ECCentral.BizEntity.SO;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.RMA.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.RMA.BizProcessor
{
    internal static class RequestCheck
    {
        #region Verify update

        public static bool VerifyUpdate(this RMARequestInfo request, RMARequestInfo requestInDb)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            if (requestInDb == null)
            {
                throw new ArgumentNullException("requestInDb");
            }

            // verify required fields
            CommonCheck.VerifyNotNull("SysNo", request.SysNo);
            CommonCheck.VerifyNotNull("RequestID", request.RequestID);
            CommonCheck.VerifyNotNull("SOSysNo", request.SOSysNo);
            CommonCheck.VerifyNotNull("CustomerSysNo", request.CustomerSysNo);            
            CommonCheck.VerifyNotNull("Address", request.Address);
            CommonCheck.VerifyNotNull("Contact", request.Contact);
            CommonCheck.VerifyNotNull("Phone", request.Phone);
            // verify request fields' max length
            request.VerifyFieldsLength();

            // 如果是”邮局普包“就必须传入”邮资转积分“数            
            if (request.ShipViaCode == RMAConst.ShipVia_PostOffice)
            {
                if (request.PostageToPoint.HasValue && request.PostageToPoint.Value > 50)
                {
                    string msg = ResouceManager.GetMessageString("RMA.Request", "PostageToPointShouldLessThan50");
                    throw new BizException(msg);
                }
            }
            if (requestInDb.Status.Value == RMARequestStatus.Complete
                || requestInDb.Status.Value == RMARequestStatus.Abandon)
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "CannotEditWhenHandlingOrClosedOrAbandon");
                throw new BizException(msg);
            }
            if (!request.Status.HasValue)
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "ObjectIsRequired");
                msg = string.Format(msg, "申请单处理状态");
                throw new BizException(msg);
            }
            if (request.Status != requestInDb.Status)
            {
                switch (request.Status.Value)
                {
                    case RMARequestStatus.Abandon:
                        request.VerifyAbandon(requestInDb);
                        break;
                    case RMARequestStatus.Complete:
                        break;
                    case RMARequestStatus.Handling:
                        request.VerifyReceive(requestInDb);
                        break;
                    case RMARequestStatus.Origin:
                        requestInDb.VerifyCancelReceive();
                        break;
                }
            }
            return true;
        }

        public static bool VerifyAbandon(this RMARequestInfo request, RMARequestInfo requestInDb)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request is required!");
            }
            if (!request.SysNo.HasValue)
            {
                throw new ArgumentNullException("request.SysNo is required!");
            }
            if (requestInDb != null
                && requestInDb.Status.HasValue
                && (requestInDb.Status.Value != RMARequestStatus.WaitingAudit && requestInDb.Status.Value != RMARequestStatus.Origin))
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "CannotAbandonOriginRequest");
                throw new BizException(msg);
            }
            return true;
        }

        public static bool VerifyCancelReceive(this RMARequestInfo requestInDb)
        {
            if (requestInDb == null)
            {
                throw new ArgumentNullException("request");
            }
            if (!requestInDb.SysNo.HasValue)
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "ObjectIsRequired");
                msg = string.Format(msg, "系统编号");
                throw new BizException(msg);
            }
            if (requestInDb.Status.Value != RMARequestStatus.Handling)
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "CannotCancelReceiveWithoutHandlingRequest");
                throw new BizException(msg);
            }

            /*
             * 不允许 Cancel Receive 的状态包括
             *   1. 存在非作废状态的送修单      或者
             *   2. 存在非作废状态的发货单      或者
             *   3. 存在非作废状态的发款单      或者
             *   4. 存在非作废状态的退货入库单
             */

            DataSet ds = ObjectFactory<IRequestDA>.Instance.LoadForCheckCancelReceive(requestInDb.SysNo.Value);
            if (ds != null && ds.Tables != null &&
                ds.Tables[0] != null && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "CannotCancelReceive");
                throw new BizException(msg);
            }
            return true;
        }

        public static bool VerifyReceive(this RMARequestInfo request, RMARequestInfo requestInDb)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            // verify required fields
            CommonCheck.VerifyNotNull("申请单ID", request.SysNo);
            CommonCheck.VerifyNotNull("仓库", request.ReceiveWarehouse);
            CommonCheck.VerifyNotNull("接收日期", request.CustomerSendTime);
            CommonCheck.VerifyNotNull("订单号", request.SOSysNo);          
            //if (!(request.ShipViaCode.ToLower().Contains("ozzo") ||
            //      request.ShipViaCode.ToLower().Contains("奥硕")))
            if (!request.ShipViaCode.ToLower().Contains(AppSettingManager.GetSetting("RMA", "PushShipTypeName")))
            {
                CommonCheck.VerifyNotNull("邮包编号", request.TrackingNumber);
            }

            // verify request fields' max length
            request.VerifyFieldsLength();

            if (requestInDb.Status.HasValue && requestInDb.Status.Value != RMARequestStatus.Origin)
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "CannotReceiveWithoutOriginRequest");
                throw new BizException(msg);
            }
            if (request.Registers == null || request.Registers.Count < 1)
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "ObjectIsRequired");
                msg = string.Format(msg, "订单列表");
                throw new BizException(msg);
            }
            request.Registers.ForEach(
                reg =>
                {
                    if (!reg.BasicInfo.IsHaveInvoice.HasValue ||
                        !reg.BasicInfo.IsFullAccessory.HasValue ||
                        !reg.BasicInfo.IsFullPackage.HasValue)
                    {
                        string msg = ResouceManager.GetMessageString("RMA.Request", "LostRegisterReceiveInformations");
                        throw new BizException(msg);
                    }
                }
            );
            if (request.ShipViaCode == RMAConst.ShipVia_PostOffice)
            {
                if (request.PostageToPoint.HasValue && request.PostageToPoint.Value > 50)
                {
                    string msg = ResouceManager.GetMessageString("RMA.Request", "PostageToPointShouldLessThan50");
                    throw new BizException(msg);
                }
            }
            return true;
        }

        public static bool VerifyAuditPassed(this RMARequestInfo request, RMARequestInfo requestInDb)
        {
            if (request == null)
            {
                throw new ArgumentNullException("Request is required!");
            }
            if (!request.SysNo.HasValue)
            {
                throw new ArgumentNullException("request.SysNo is required!");
            }
            if (requestInDb != null
                && requestInDb.Status.HasValue
                && requestInDb.Status.Value != RMARequestStatus.WaitingAudit)
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "LostRegisterReceiveInformations");
                throw new BizException(msg);               
            }
            return true;
        }

        public static bool VerifyAuditRefuesed(this RMARequestInfo request, RMARequestInfo requestInDb)
        {
            if (request == null)
            {
                throw new ArgumentNullException("Request is required!");
            }
            if (!request.SysNo.HasValue)
            {
                throw new ArgumentNullException("request.SysNo is required!");
            }
            if (requestInDb != null
                && requestInDb.Status.HasValue
                && requestInDb.Status.Value != RMARequestStatus.WaitingAudit)
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "LostRegisterReceiveInformations");
                throw new BizException(msg);
            }
            return true;
        }

        #endregion

        #region Verify create

        public static bool VerifyForCreate(this RMARequestInfo request)
        {
            // verify required fields
            CommonCheck.VerifyNotNull("销售单号", request.SOSysNo);
            CommonCheck.VerifyNotNull("顾客编号", request.CustomerSysNo);
            CommonCheck.VerifyNotNull("联系电话", request.Phone);
            CommonCheck.VerifyNotNull("联系人", request.Contact);
            CommonCheck.VerifyNotNull("送货方式", request.ShipViaCode);
            CommonCheck.VerifyNotNull("收货地址区域编号", request.AreaSysNo);
            CommonCheck.VerifyNotNull("收货地址", request.Address);
            // verify all fields' length
            request.VerifyFieldsLength();

            VerifyGiftCardRMA(request);

            // tracking# is required withou OZZO ship via
            if ((!request.ShipViaCode.ToLower().Contains("ozzo") &&
                    !request.ShipViaCode.ToLower().Contains("奥硕"))
                && StringUtility.IsNullOrEmpty(request.TrackingNumber))
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "ObjectIsRequired");
                msg = string.Format(msg, "邮包编号");
                throw new BizException(msg);
            }

            return request.VerifyDuplicate();
        }

        private static bool VerifyDuplicate(this RMARequestInfo request)
        {
            if (!request.NeedVerifyDuplicate || request.Registers == null || request.Registers.Count < 1)
            {
                return true;
            }
            Dictionary<int, int> master = new Dictionary<int, int>();
            Dictionary<int, int> exwarranty = new Dictionary<int, int>();
            request.Registers.ForEach(r =>
            {
                if (r.BasicInfo.SOItemType.Value == SOProductType.ExtendWarranty)
                {
                    if (exwarranty.ContainsKey(r.BasicInfo.ProductSysNo.Value))
                    {
                        exwarranty[r.BasicInfo.ProductSysNo.Value]++;
                    }
                    else
                    {
                        exwarranty.Add(r.BasicInfo.ProductSysNo.Value, 1);
                    }
                }
                else
                {
                    if (master.ContainsKey(r.BasicInfo.ProductSysNo.Value))
                    {
                        master[r.BasicInfo.ProductSysNo.Value]++;
                    }
                    else
                    {
                        master.Add(r.BasicInfo.ProductSysNo.Value, 1);
                    }
                }
            });

            // verify master so items
            foreach (KeyValuePair<int, int> pair in master)
            {
                int? soitemQty = GetSoItemQty(pair.Key, SOProductType.Product, request.SOSysNo.Value);
                if (soitemQty.HasValue)
                {
                    int? regQty = ObjectFactory<IRegisterDA>.Instance.GetRegisterQty(
                        pair.Key, (int)SOProductType.Product, request.SOSysNo.Value);
                    if (soitemQty < regQty + pair.Value)
                    {
                        string msg = ResouceManager.GetMessageString("RMA.Request", "DuplicateRegister");
                        throw new BizException(msg);
                    }
                }
            }
            // verify extend warranty so items
            foreach (KeyValuePair<int, int> pair in exwarranty)
            {
                int? soitemQty = GetSoItemQty(pair.Key, SOProductType.ExtendWarranty, request.SOSysNo.Value);
                if (soitemQty.HasValue)
                {
                    int? regQty = ObjectFactory<IRegisterDA>.Instance.GetRegisterQty(
                        pair.Key, (int)SOProductType.ExtendWarranty, request.SOSysNo.Value);
                    if (soitemQty < regQty + pair.Value)
                    {
                        string msg = ResouceManager.GetMessageString("RMA.Request", "DuplicateRegister");
                        throw new BizException(msg);
                    }
                }
            }
            return true;
        }

        private static int? GetSoItemQty(int productSysNo, SOProductType soItemType, int soSysNo)
        {
            var items = ObjectFactory<ISOBizInteract>.Instance.GetSOItemList(soSysNo);
            SOItemInfo item = null;
            if (soItemType == SOProductType.ExtendWarranty)
            {
                item = items.FirstOrDefault(p => p.MasterProductSysNo == productSysNo.ToString()
                    && p.ProductType == SOProductType.ExtendWarranty);
            }
            else
            {
                item = items.FirstOrDefault(p => p.ProductSysNo == productSysNo && p.ProductType == soItemType);
            }
            if (item != null)
            {
                return item.Quantity.Value;
            }
            return default(int?);
        }

        private static bool VerifyGiftCardRMA(RMARequestInfo request)
        {
            var so = ObjectFactory<ISOBizInteract>.Instance.GetSOBaseInfo(request.SOSysNo.Value);
            var items = ObjectFactory<ISOBizInteract>.Instance.GetSOItemList(request.SOSysNo.Value);

            if (so == null || items == null || items.Count == 0)
            {
                string msg = ResouceManager.GetMessageString("RMA.Request", "ExceptionOrder");
                throw new BizException(msg);
            }

            if (so.SOType.HasValue)
            {
                if (so.SOType == SOType.ElectronicCard || so.SOType == SOType.PhysicalCard)
                {
                    string msg = ResouceManager.GetMessageString("RMA.Request", "GiftCardCannotRMA");
                    throw new BizException(msg);
                }
            }
            return true;
        }

        //验证申请单创建成功后是否发送短信(商家订单（对应2，3，4，5，6模式），创建RMA申请单不发送邮件)
        public static bool VerifyForSendCreateEmail(this RMARequestInfo request)
        {
            if (request != null
                  && (
                    (request.StockType == StockType.MET && request.ShippingType == BizEntity.Invoice.DeliveryType.SELF && request.InvoiceType == InvoiceType.SELF)//2
                    ||
                    (request.StockType == StockType.MET && request.ShippingType == BizEntity.Invoice.DeliveryType.SELF && request.InvoiceType == InvoiceType.MET)//5
                    ||
                    (request.StockType == StockType.SELF && request.ShippingType == BizEntity.Invoice.DeliveryType.SELF && request.InvoiceType == InvoiceType.MET)//4
                    ||
                    (request.StockType == StockType.MET && request.ShippingType == BizEntity.Invoice.DeliveryType.MET && request.InvoiceType == InvoiceType.SELF)//3
                    ||
                    (request.StockType == StockType.MET && request.ShippingType == BizEntity.Invoice.DeliveryType.MET && request.InvoiceType == InvoiceType.MET)//6
                    ))
            {
                return false;
            }
            return true;
        }

        #endregion

        #region verify

        public static bool VerifyFieldsLength(this RMARequestInfo request)
        {
            if (request != null)
            {
                CommonCheck.VerifyLength("RequestID", request.RequestID, 20);
                CommonCheck.VerifyLength("Address", request.Address, 200);
                CommonCheck.VerifyLength("Contact", request.Contact, 50);
                CommonCheck.VerifyLength("Phone", request.Phone, 50);
                CommonCheck.VerifyLength("Note", request.Note, 200);
                CommonCheck.VerifyLength("Memo", request.Memo, 200);
                CommonCheck.VerifyLength("ShipViaCode", request.ShipViaCode, 50);
                CommonCheck.VerifyLength("TrackingNumber", request.TrackingNumber, 50);
                CommonCheck.VerifyLength("ReceiveWarehouse", request.ReceiveWarehouse, 4);
            }
            return true;
        }
        
        #endregion
    }
}
