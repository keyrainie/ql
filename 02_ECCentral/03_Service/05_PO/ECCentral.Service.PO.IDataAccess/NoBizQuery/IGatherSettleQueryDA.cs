using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.PO;

namespace ECCentral.Service.PO.IDataAccess.NoBizQuery
{
    public interface IGatherSettleQueryDA
    {
        DataTable Query(GatherSettleQueryFilter queryFilter, out int totalCount);

        DataTable QuerySettleList(SettleQueryFilter queryFilter, out int totalCount);
    }
}
