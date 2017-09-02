using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.IDataAccess
{
    public interface IPOVendorInvoiceDA
    {

        POVendorInvoiceInfo Create(POVendorInvoiceInfo input);

        void Update(POVendorInvoiceInfo entity);

        POVendorInvoiceInfo GetPOVendorInvoiceBySysNo(int sysNo);

        void UpdateStatus(int sysNo, InvoiceStatus invoiceStatus);

        void Audit(int sysNo, InvoiceStatus invoiceStatus);
    }
}
