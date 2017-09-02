using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.RMA;
using ECCentral.QueryFilter.RMA;
using ECCentral.Service.RMA.AppService;
using ECCentral.Service.RMA.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;
using ECCentral.Service.RMA.Restful.ResponseMsg;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.SO;

namespace ECCentral.Service.RMA.Restful
{
    [ServiceContract]
    public partial class RMAService
    {
        [WebInvoke(UriTemplate = "/Request/Query", Method = "POST")]        
        public QueryResult QueryRequest(RMARequestQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IRMARequestQueryDA>.Instance.QueryRMARequest(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/Request/GetAllReceiveUsers", Method = "GET")]
        public List<UserInfo> GetAllReceiveUsers()
        {
            return ObjectFactory<IRMARequestQueryDA>.Instance.GetAllReceiveUsers();
        }

        [WebInvoke(UriTemplate = "/Request/GetAllConfirmUsers", Method = "GET")]
        public List<UserInfo> GetAllConfirmUsers()
        {
            return ObjectFactory<IRMARequestQueryDA>.Instance.GetAllConfirmUsers();
        }

        [WebInvoke(UriTemplate = "/Request/Load/{sysNo}", Method = "GET")]
        public RequestDetailInfoRsp LoadRequestBySysNo(string sysNo)
        {
             int requestSysNo;
             if (int.TryParse(sysNo, out requestSysNo))
             {
                 string businessModel, deliveryUserName;
                 CustomerInfo customer;
                 SOBaseInfo soBaseInfo;
                 DeliveryInfo deliveryInfo;
                 var request = ObjectFactory<RequestAppService>.Instance.LoadWithRegistersBySysNo(requestSysNo, out customer, out soBaseInfo, out deliveryInfo, out deliveryUserName, out businessModel);
                 RequestDetailInfoRsp result = new RequestDetailInfoRsp
                 {
                     CustomerID = customer.BasicInfo.CustomerID,
                     CustomerName = customer.BasicInfo.CustomerName,                     
                     BusinessModel = businessModel,
                     RequestInfo = request
                 };
                 if (soBaseInfo != null)
                 {
                     result.SOID = soBaseInfo.SOID;
                 }
                 if (deliveryInfo != null)
                 {
                     result.DeliveryStatus = deliveryInfo.Status;
                     result.DeliveryUserName = deliveryUserName;
                 }
                 return result;
             }
             throw new ArgumentException("Invalid sysNo");
        }

        [WebInvoke(UriTemplate = "/Request/Create", Method = "POST")]
        public RMARequestInfo CreateRequest(RMARequestInfo request)
        {
            return ObjectFactory<RequestAppService>.Instance.Create(request);
        }

        [WebInvoke(UriTemplate = "/Request/Update", Method = "PUT")]
        public void UpdateRequest(RMARequestInfo request)
        {
            ObjectFactory<RequestAppService>.Instance.Update(request);
        }

        [WebInvoke(UriTemplate = "/Request/Receive", Method = "PUT")]
        public RMARequestInfo Receive(RMARequestInfo request)
        {
            return ObjectFactory<RequestAppService>.Instance.Receive(request);
        }

        [WebInvoke(UriTemplate = "/Request/CancelReceive", Method = "PUT")]
        public void CancelReceive(RMARequestInfo request)
        {
            ObjectFactory<RequestAppService>.Instance.CancelReceive(request.SysNo.Value);
        }

        [WebInvoke(UriTemplate = "/Request/PrintLabels", Method = "PUT")]
        public void PrintLabels(RMARequestInfo request)
        {
            ObjectFactory<RequestAppService>.Instance.PrintLabels(request.SysNo.Value);
        }

        [WebInvoke(UriTemplate = "/Request/Close", Method = "PUT")]
        public void CloseRequest(RMARequestInfo request)
        {
            ObjectFactory<RequestAppService>.Instance.Close(request);
        }

        [WebInvoke(UriTemplate = "/Request/Abandon", Method = "PUT")]
        public void AbandonRequest(RMARequestInfo request)
        {
            ObjectFactory<RequestAppService>.Instance.Abandon(request.SysNo.Value);
        }

        [WebInvoke(UriTemplate = "/Request/Audit", Method = "PUT")]
        public RMARequestInfo Audit(RMARequestInfo request)
        {
            return ObjectFactory<RequestAppService>.Instance.Adjust(request.SysNo.Value);
        }

        [WebInvoke(UriTemplate = "/Request/Refused", Method = "PUT")]
        public RMARequestInfo Refused(RMARequestInfo request)
        {
            return ObjectFactory<RequestAppService>.Instance.Refused(request.SysNo.Value);
        }
    }
}
