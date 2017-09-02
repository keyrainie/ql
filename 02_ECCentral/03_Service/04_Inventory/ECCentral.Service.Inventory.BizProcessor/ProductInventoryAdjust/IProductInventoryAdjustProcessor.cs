using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.Inventory.BizProcessor
{
    public interface IProductInventoryAdjustProcessor
    {
        string AdjustResultMsg { get; set; }
        void AdjustProductInventory(InventoryAdjustContractInfo adjustContractInfo);
    }
}
