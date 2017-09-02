using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Invoice;

namespace ECCentral.Service.Invoice.IDataAccess.NoBizQuery
{
    public interface ISAPQueryDA
    {
        DataTable QueryVendor(SAPVendorQueryFilter request, out int totalCount);

        DataTable QueryCompany(SAPCompanyQueryFilter request, out int totalCount);

        DataTable QueryIPPUser(SAPIPPUserQueryFilter request, out int totalCount);
    }
}