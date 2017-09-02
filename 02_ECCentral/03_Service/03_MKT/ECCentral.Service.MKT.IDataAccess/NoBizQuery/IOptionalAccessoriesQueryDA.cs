using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.MKT;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface IOptionalAccessoriesQueryDA
    {
        DataTable Query(OptionalAccessoriesQueryFilter filter, out int totalCount);
    }
}
