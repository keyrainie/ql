using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.MKT;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface IHotSaleCategoryQueryDA
    {
        DataTable Query(HotSaleCategoryQueryFilter filter, out int totalCount);
    }
}
