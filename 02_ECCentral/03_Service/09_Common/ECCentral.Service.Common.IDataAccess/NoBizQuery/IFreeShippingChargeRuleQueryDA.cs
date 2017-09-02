using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Service.Common.IDataAccess
{
    public interface IFreeShippingChargeRuleQueryDA
    {
        DataTable Query(FreeShippingChargeRuleQueryFilter filter, out int totalCount);
    }
}
