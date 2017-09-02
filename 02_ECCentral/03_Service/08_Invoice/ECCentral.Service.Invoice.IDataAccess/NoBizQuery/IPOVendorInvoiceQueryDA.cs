using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.Invoice;

namespace ECCentral.Service.Invoice.IDataAccess.NoBizQuery
{
    public interface IPOVendorInvoiceQueryDA
    {
        DataTable QueryPOVendorInvoice(POVendorInvoiceQueryFilter query, out int totalCountout);

        DataTable QueryTotalAmountByVendor(POVendorInvoiceQueryFilter query, out int totalCount);
    }
}
