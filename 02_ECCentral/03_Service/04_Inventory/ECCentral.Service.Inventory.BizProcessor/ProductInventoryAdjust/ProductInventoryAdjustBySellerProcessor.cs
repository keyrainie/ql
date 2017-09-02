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
    [VersionExport(typeof(ProductInventoryAdjustBySellerProcessor))]
    public class ProductInventoryAdjustBySellerProcessor : ProductInventoryAdjustProcessor
    {
        #region 抽象方法实现

        public override void SetProductInventoryAdjustInfo()
        {
            switch (this.AdjustContractInfo.SourceActionName)
            {
                case InventoryAdjustSourceAction.Update:
                    SetInventoryAdjustInfoForUpdate();
                    break;
                default:
                    throw new NotSupportedException("Not Supported Action");
            }
        }


        #endregion  抽象方法实现

        #region 虚方法重写

        public override void ProcessOriginAdjustQuantity()
        {
            //Seller库存调整单传入的是新的可用库存量, 而不是现可用库存量的增减量, 需要处理为标准的AdjustQty;
            this.AdjustQuantity = this.CurrentAdjustItemInfo.AdjustQuantity - this.StockInventoryCurrentInfo.AvailableQty;
        }

        #endregion 虚方法重写

        #region 各Action的库存数量设置方法

        private void SetInventoryAdjustInfoForUpdate()
        {
            //调整可用库存, 实际增减根据AdjustQty的正负确定
            this.AdjustAvailableQty(this.AdjustQuantity);
        }

        #endregion 各Action的库存数量设置方法

        #region 调整库存相关业务方法

        #endregion 调整库存相关业务方法
    }
}
