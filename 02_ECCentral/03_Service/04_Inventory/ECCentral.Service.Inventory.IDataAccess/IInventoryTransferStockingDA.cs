using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Inventory;

namespace ECCentral.Service.Inventory.IDataAccess
{
    public interface IInventoryTransferStockingDA
    {
        /// <summary>
        /// 查询 - 备货中心List
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<ProductCenterItemInfo> QueryInventoryTransferStockingList(InventoryTransferStockingQueryFilter queryFilter, out int totalCount);

    }
}
