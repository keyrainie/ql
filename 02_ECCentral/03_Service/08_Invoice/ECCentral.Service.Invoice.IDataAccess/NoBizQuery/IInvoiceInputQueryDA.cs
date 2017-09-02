using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.Invoice;

namespace ECCentral.Service.Invoice.IDataAccess.NoBizQuery
{
    public interface IInvoiceInputQueryDA
    {
        DataTable QueryInvoiceInput(InvoiceInputQueryFilter query, out int totalCountout);

        int GetPaySettleCompany(int vendorSysNo);
    }
}
