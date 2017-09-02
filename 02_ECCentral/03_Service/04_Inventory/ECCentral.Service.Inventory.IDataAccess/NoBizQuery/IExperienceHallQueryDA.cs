using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Inventory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Inventory.IDataAccess.NoBizQuery
{
    /// <summary>
    /// 体验厅查询
    /// </summary>
    public interface IExperienceHallQueryDA
    {
        DataTable QueryExperienceHallInventory(ExperienceHallInventoryInfoQueryFilter filter, out int totalCount);

        DataTable QueryExperienceHallAllocateOrder(QueryFilter.Inventory.ExperienceHallAllocateOrderQueryFilter queryFilter, out int totalCount);
    }
}
