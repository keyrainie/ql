using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.Inventory;

namespace ECCentral.Service.Inventory.IDataAccess.NoBizQuery
{
    /// <summary>
    /// 货卡查询
    /// </summary>
    public interface IInventoryCardItemsQueryDA
    {
        /// <summary>
        /// 查询货卡关联单据
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryCardItemOrdersRelated(InventoryItemCardQueryFilter queryFilter, out int totalCount);

    }
}
