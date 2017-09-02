using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.Customer;
using ECCentral.Service.Utility;
using ECCentral.Service.Customer.IDataAccess.NoBizQuery;
using ECCentral.Service.Customer.Restful.RequestMsg;
using ECCentral.Service.Customer.AppService;

namespace ECCentral.Service.Customer.Restful
{
    public partial class CustomerService
    {
        [WebInvoke(UriTemplate = "/SMS/Query", Method = "POST")]
        public QueryResult QuerySMS(SMSQueryFilter query)
        {
            int totalCount;
            return new QueryResult()
            {
                Data = ObjectFactory<ISMSQueryDA>.Instance.Query(query, out totalCount),
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/SMS/SendByCellphone", Method = "POST")]
        public List<string> SendByCellphone(SendSMSReq request)
        {
            return ObjectFactory<SendSMSAndEmailService>.Instance.SendByCellphone(request.Numbers, request.Message);
        }
        [WebInvoke(UriTemplate = "/SMS/SendBySOSysNo", Method = "POST")]
        public List<string> SendBySOSysNo(SendSMSReq request)
        {
            return ObjectFactory<SendSMSAndEmailService>.Instance.SendBySOSysNo(request.Numbers, request.Message);
        }

        [WebInvoke(UriTemplate = "/SMS/SendEmail", Method = "POST")]
        public List<string> SendEmail(SendEmailReq request)
        {
            return ObjectFactory<SendSMSAndEmailService>.Instance.SendEmail(request.EmailList, request.Title, request.Content,request.CompanyCode);
        }
    }
}
