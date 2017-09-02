using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

using ECCentral.BizEntity;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Inventory.BizProcessor
{
    [VersionExport(typeof(ProductInventoryAdjustByChannelProcessor))]
    public class ProductInventoryAdjustByChannelProcessor : ProductInventoryAdjustProcessor
    {
        #region 抽象方法实现

        public override void SetProductInventoryAdjustInfo()
        {
            switch (this.AdjustContractInfo.SourceActionName)
            {
                case InventoryAdjustSourceAction.Allocate:
                    SetInventoryAdjustInfoForAllocate();
                    break;
                case InventoryAdjustSourceAction.Return:
                    SetInventoryAdjustInfoForReturn();
                    break;
                default:
                    throw new NotSupportedException("Not Supported Action");
            }
        }
        
        #endregion  抽象方法实现

        #region 虚方法重写
        #endregion 虚方法重写

        #region 调整库存相关业务方法

        #endregion 调整库存相关业务方法
        
        #region 各Action的库存数量设置方法

        private void SetInventoryAdjustInfoForAllocate()
        {
            //分配渠道库存, 减少可用库存, 增加渠道库存
            this.AdjustAvailableQty(-this.AdjustQuantity);
            this.AdjustChannelQty(this.AdjustQuantity);
        }

        private void SetInventoryAdjustInfoForReturn()
        {
            //收回渠道库存, 增加可用库存, 减少渠道库存
            this.AdjustAvailableQty(this.AdjustQuantity);
            this.AdjustChannelQty(-this.AdjustQuantity);
        }

        #endregion 各Action的库存数量设置方法
    }
}
