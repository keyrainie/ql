using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.Inventory;

namespace ECCentral.Service.Inventory.IDataAccess.NoBizQuery
{
    public interface IInventoryAllocatedItemsQueryDA
    {
        /// <summary>
        /// 查询已分配库存关联单据
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        DataTable QueryAllocatedItemOrdersRelated(InventoryAllocatedCardQueryFilter queryFilter, out int totalCount);
    }
}
