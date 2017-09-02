using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.Inventory.Restful.ResponseMsg
{
    public class InventoryTransferStockingQueryRsp
    {
        public int TotalCount { get; set; }

        public List<ProductCenterItemInfo> ResultList { get; set; }
    }
}
