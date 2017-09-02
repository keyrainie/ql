using System;
using System.Collections.Generic;
using System.ServiceModel.Web;

using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.RMA;
using ECCentral.QueryFilter.RMA;
using ECCentral.Service.RMA.AppService;
using ECCentral.Service.RMA.IDataAccess.NoBizQuery;
using ECCentral.Service.RMA.Restful.ResponseMsg;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using ECCentral.BizEntity.Invoice;
using System.Data;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.RMA.Restful
{
    public partial class RMAService
    {
        [WebInvoke(UriTemplate = "/Register/Query", Method = "POST")]
        public QueryResult QueryRegister(RegisterQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IRegisterQueryDA>.Instance.QueryRegister(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/Register/Load/{sysNo}", Method = "GET")]
        public RegisterDetailInfoRsp LoadRegisterBySysNo(string sysNo)
        {
            int no = 0;
            if (int.TryParse(sysNo, out no))
            {
                RegisterDetailInfoRsp result = new RegisterDetailInfoRsp();
                CustomerInfo customerInfo;
                RMARequestInfo requestInfo;
                ProcessType processType;
                string businessModel;
                InvoiceType? invoiceType;
                List<ProductInventoryInfo> inventoryInfo;
                ProductInventoryType inventoryType;
                int? refundSysNo;
                result.Register = ObjectFactory<RegisterAppService>.Instance.LoadForEditBySysNo(
                    int.Parse(sysNo),
                    out businessModel,
                    out processType,
                    out invoiceType,
                    out customerInfo,
                    out requestInfo,
                    out inventoryInfo,
                    out refundSysNo,
                    out inventoryType);
                result.ReceiveTime = requestInfo.ReceiveTime;
                result.SOSysNo = requestInfo.SOSysNo;
                result.ProcessType = processType;
                result.InvoiceType = invoiceType;
                result.BusinessModel = businessModel;
                result.RequestSysNo = requestInfo.SysNo;
                result.CustomerID = customerInfo.BasicInfo.CustomerID;
                result.CustomerName = customerInfo.BasicInfo.CustomerName;
                result.CustomerRank = customerInfo.Rank;
                result.ProductInventoryInfo = inventoryInfo;
                result.RefundSysNo = refundSysNo;
                result.InventoryType = inventoryType;
                return result;
            }
            throw new ArgumentException("Invalid sysNo");
        }

        [WebInvoke(UriTemplate = "/Register/LoadSecondHandProducts/{productID}", Method = "GET")]
        public List<RegisterSecondHandRsp> LoadSecondHandProducts(string ProductID)
        {
            List<ProductInfo> productList = ObjectFactory<RegisterAppService>.Instance.GetSecondProductInfoList(ProductID);
            List<RegisterSecondHandRsp> tList = new List<RegisterSecondHandRsp>();
            productList.ForEach(x =>
            {
                tList.Add(new RegisterSecondHandRsp()
                {
                    SysNo = x.SysNo,
                    ProductID = x.ProductID
                });
            });
            return tList;
        }

        [WebInvoke(UriTemplate = "/Register/LoadRegisterDunLog/{registerSysNo}", Method = "GET")]
        public List<RegisterDunLog> LoadRegisterDunLog(string registerSysNo)
        {
            int sysNo;
            if (int.TryParse(registerSysNo, out sysNo))
            {
                return ObjectFactory<IRegisterQueryDA>.Instance.QueryRegisterDunLog(sysNo);
            }
            throw new ArgumentException("Invalid registerSysNo");
        }

        [WebInvoke(UriTemplate = "/Register/UpdateBasicInfo", Method = "PUT")]
        public RMARegisterInfo UpdateRegisterBasicInfo(RMARegisterInfo register)
        {
            return ObjectFactory<RegisterAppService>.Instance.UpdateBasicInfo(register);
        }

        [WebInvoke(UriTemplate = "/Register/UpdateCheckInfo", Method = "PUT")]
        public RMARegisterInfo UpdateRegisterCheckInfo(RMARegisterInfo register)
        {
            return ObjectFactory<RegisterAppService>.Instance.UpdateCheckInfo(register);
        }

        [WebInvoke(UriTemplate = "/Register/UpdateResponseInfo", Method = "PUT")]
        public RMARegisterInfo UpdateRegisterResponseInfo(RMARegisterInfo register)
        {
            return ObjectFactory<RegisterAppService>.Instance.UpdateResponseInfo(register);
        }

        [WebInvoke(UriTemplate = "/Register/SetWaitingReturn", Method = "PUT")]
        public RMARegisterInfo SetWaitingReturn(int sysNo)
        {
            return ObjectFactory<RegisterAppService>.Instance.SetWaitingReturn(sysNo);
        }

        [WebInvoke(UriTemplate = "/Register/CancelWaitingReturn", Method = "PUT")]
        public void CancelWaitingReturn(int sysNo)
        {
            ObjectFactory<RegisterAppService>.Instance.CancelWaitingReturn(sysNo);
        }

        [WebInvoke(UriTemplate = "/Register/SetWaitingOutbound", Method = "PUT")]
        public RMARegisterInfo SetWaitingOutbound(int sysNo)
        {
            return ObjectFactory<RegisterAppService>.Instance.SetWaitingOutbound(sysNo);
        }

        [WebInvoke(UriTemplate = "/Register/CancelWaitingOutbound", Method = "PUT")]
        public void CancelWaitingOutbound(int sysNo)
        {
            ObjectFactory<RegisterAppService>.Instance.CancelWaitingOutbound(sysNo);
        }

        [WebInvoke(UriTemplate = "/Register/SetWaitingRefund", Method = "PUT")]
        public RMARegisterInfo SetWaitingRefund(int sysNo)
        {
            return ObjectFactory<RegisterAppService>.Instance.SetWaitingRefund(sysNo);
        }

        [WebInvoke(UriTemplate = "/Register/CancelWaitingRefund", Method = "PUT")]
        public void CancelWaitingRefund(int sysNo)
        {
            ObjectFactory<RegisterAppService>.Instance.CancelWaitingRefund(sysNo);
        }

        [WebInvoke(UriTemplate = "/Register/SetWaitingRevert", Method = "PUT")]
        public RMARegisterInfo SetWaitingRevert(RMARegisterInfo register)
        {
            return ObjectFactory<RegisterAppService>.Instance.SetWaitingRevert(register);
        }

        [WebInvoke(UriTemplate = "/Register/CancelWaitingRevert", Method = "PUT")]
        public void CancelWaitingRevert(RMARegisterInfo register)
        {
            ObjectFactory<RegisterAppService>.Instance.CancelWaitingRevert(register);
        }

        [WebInvoke(UriTemplate = "/Register/ApproveRevertAudit", Method = "PUT")]
        public RMARegisterInfo ApproveRevertAudit(RMARegisterInfo register)
        {
            return ObjectFactory<RegisterAppService>.Instance.ApproveRevertAudit(register);
        }

        [WebInvoke(UriTemplate = "/Register/RejectRevertAudit", Method = "PUT")]
        public RMARegisterInfo RejectRevertAudit(RMARegisterInfo register)
        {
            return ObjectFactory<RegisterAppService>.Instance.RejectRevertAudit(register);
        }

        [WebInvoke(UriTemplate = "/Register/Close", Method = "PUT")]
        public RMARegisterInfo CloseRegister(int sysNo)
        {
            return ObjectFactory<RegisterAppService>.Instance.Close(sysNo);
        }

        [WebInvoke(UriTemplate = "/Register/CloseCase", Method = "PUT")]
        public RMARegisterInfo CloseRegisterCase(int sysNo)
        {
            return ObjectFactory<RegisterAppService>.Instance.CloseCase(sysNo);
        }

        [WebInvoke(UriTemplate = "/Register/ReOpen", Method = "PUT")]
        public RMARegisterInfo ReOpenRegister(int sysNo)
        {
            return ObjectFactory<RegisterAppService>.Instance.ReOpen(sysNo);
        }

        [WebInvoke(UriTemplate = "/Register/Abandon", Method = "PUT")]
        public RMARegisterInfo AbandonRegister(int sysNo)
        {
            return ObjectFactory<RegisterAppService>.Instance.SetAbandon(sysNo);
        }

        [WebInvoke(UriTemplate = "/Register/LoadRegisterMemo/{RegisterSysNo}", Method = "GET")]
        public RegisterMemoRsp LoadRegisterMemo(string RegisterSysNo)
        {
            int registerSysNo;
            if (int.TryParse(RegisterSysNo, out registerSysNo))
            {
                string memo = string.Empty;
                string productID = string.Empty;
                string productName = string.Empty;
                string vendorName = string.Empty;
                ObjectFactory<RegisterAppService>.Instance.LoadRegisterMemo(registerSysNo, ref memo, ref productID, ref productName, ref vendorName);
                RegisterMemoRsp result = new RegisterMemoRsp
                {
                    RegisterSysNo = registerSysNo,
                    Memo = memo,
                    ProductID = productID,
                    ProductName = productName,
                    VendorName = vendorName
                };
                return result;
            }
            throw new ArgumentException("Invalid registerSysNo");

        }

        [WebInvoke(UriTemplate = "/Register/SyncERP", Method = "PUT")]
        public RMARegisterInfo SyncERPRegister(int sysNo)
        {
            return ObjectFactory<RegisterAppService>.Instance.SyncERP(sysNo);
        }

    }
}
