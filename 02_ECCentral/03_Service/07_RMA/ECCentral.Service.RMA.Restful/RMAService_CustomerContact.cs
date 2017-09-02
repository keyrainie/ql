using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.RMA.AppService;
using ECCentral.Service.Utility;

namespace ECCentral.Service.RMA.Restful
{
    public partial class RMAService
    {
        [WebInvoke(UriTemplate = "/CustomerContact/Load/{sysNo}", Method = "GET")]
        public CustomerContactInfo LoadByRequestSysNo(string sysNo)
        {
            return ObjectFactory<CustomerContactAppService>.Instance.LoadByRequestSysNo(int.Parse(sysNo));
        }

        [WebInvoke(UriTemplate = "/CustomerContact/LoadOrigin/{sysNo}", Method = "GET")]
        public CustomerContactInfo LoadOriginByRequestSysNo(string sysNo)
        {
            return ObjectFactory<CustomerContactAppService>.Instance.LoadOriginByRequestSysNo(int.Parse(sysNo));
        }

        [WebInvoke(UriTemplate = "/CustomerContact/Create", Method = "POST")]
        public CustomerContactInfo CreateCustomerContact(CustomerContactInfo customerContact)
        {
            return ObjectFactory<CustomerContactAppService>.Instance.Create(customerContact);
        }

        [WebInvoke(UriTemplate = "/CustomerContact/Update", Method = "PUT")]
        public CustomerContactInfo UpdateCustomerContact(CustomerContactInfo customerContact)
        {
            return ObjectFactory<CustomerContactAppService>.Instance.Create(customerContact);
        }
    }
}
