using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.MKT;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface ISaleDiscountRuleQueryDA
    {
        DataTable Query(SaleDiscountRuleQueryFilter filter, out int totalCount);
    }
}
