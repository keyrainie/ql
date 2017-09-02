using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.Customer;

namespace ECCentral.Service.Customer.IDataAccess.NoBizQuery
{
    public interface IRefundRequestQueryDA
    {
        DataTable Query(RefundRequestQueryFilter filter, out int totalCount);
    }
}
