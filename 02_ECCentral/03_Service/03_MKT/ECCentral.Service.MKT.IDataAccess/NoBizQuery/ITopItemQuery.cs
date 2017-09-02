using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface ITopItemQuery
    {
        DataTable QueryTopItem(QueryFilter.MKT.TopItemFilter filter, out int totalCount);
    }
}
