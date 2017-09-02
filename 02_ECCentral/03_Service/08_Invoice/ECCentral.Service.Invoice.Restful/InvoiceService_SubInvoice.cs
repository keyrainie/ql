using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Invoice.AppService;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.Restful
{
    public partial class InvoiceService
    {
        [WebInvoke(UriTemplate = "/SubInvoice/Load/{soSysNo}", Method = "GET")]
        public List<SubInvoiceInfo> LoadSubInvoiceList(string soSysNo)
        {
            return ObjectFactory<SubInvoiceAppService>.Instance.LoadSubInvoiceList(int.Parse(soSysNo));
        }
    }
}