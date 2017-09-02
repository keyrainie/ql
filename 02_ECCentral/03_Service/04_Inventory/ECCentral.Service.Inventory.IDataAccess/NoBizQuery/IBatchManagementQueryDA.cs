using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Inventory;

namespace ECCentral.Service.Inventory.IDataAccess.NoBizQuery
{
    public interface IBatchManagementQueryDA
    {
        DataTable QueryAdventProductsList(AdventProductsQueryFilter queryFilter, out int totalCount);
    }
}
