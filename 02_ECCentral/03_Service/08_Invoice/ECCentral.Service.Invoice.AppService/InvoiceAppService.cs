using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.Invoice.BizProcessor;

namespace ECCentral.Service.Invoice.AppService
{
    [VersionExport(typeof(InvoiceAppService))]
    public class InvoiceAppService
    {

        public void UpdateSOInvoice(int soSysNo, string invoiceNo, string warehouseNo, string companyCode)
        {
            ObjectFactory<InvoiceProcessor>.Instance.UpdateSOInvoice(soSysNo, invoiceNo, warehouseNo, companyCode);
        }
    }
}
