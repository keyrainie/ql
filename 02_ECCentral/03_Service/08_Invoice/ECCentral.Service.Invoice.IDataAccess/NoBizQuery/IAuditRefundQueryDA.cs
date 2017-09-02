using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Invoice;

namespace ECCentral.Service.Invoice.IDataAccess.NoBizQuery
{
    public interface IAuditRefundQueryDA
    {
        DataTable Query(AuditRefundQueryFilter filter, out int totalCount);
    }
}