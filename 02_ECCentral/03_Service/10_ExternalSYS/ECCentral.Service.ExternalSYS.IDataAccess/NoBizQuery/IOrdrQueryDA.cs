using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.ExternalSYS;
using System.Data;

namespace ECCentral.Service.ExternalSYS.IDataAccess.NoBizQuery
{
    public interface IOrdrQueryDA
    {
        DataTable Query(OrderQueryFilter filter, out int totalCount);
    }
}
