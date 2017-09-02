using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.Customer;

namespace ECCentral.Service.Customer.IDataAccess.NoBizQuery
{
    public interface ICSQueryDA
    {
        DataTable Query(CSQueryFilter filter, out int totalCount);
    }
}
