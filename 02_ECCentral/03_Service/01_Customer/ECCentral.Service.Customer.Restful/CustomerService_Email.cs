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
        [WebInvoke(UriTemplate = "/Email/Query", Method = "POST")]
        public QueryResult QueryEmail(EmailQueryFilter query)
        {
            int totalCount;
            return new QueryResult()
            {
                Data = ObjectFactory<IEmailQueryDA>.Instance.Query(query, out totalCount),
                TotalCount = totalCount
            };
        }
        [WebInvoke(UriTemplate = "/Email/MailDBLoad/{SysNo}", Method = "GET")]
        public string MailDBLoad(string SysNo)
        {
            return ObjectFactory<CustomerEmailAppService>.Instance.GetEmailContent(int.Parse(SysNo), "MailDB");
        }

        [WebInvoke(UriTemplate = "/Email/IPP3Load/{SysNo}", Method = "GET")]
        public string IPP3Load(string SysNo)
        {
            return ObjectFactory<CustomerEmailAppService>.Instance.GetEmailContent(int.Parse(SysNo), "IPP3");
        }
    }
}
