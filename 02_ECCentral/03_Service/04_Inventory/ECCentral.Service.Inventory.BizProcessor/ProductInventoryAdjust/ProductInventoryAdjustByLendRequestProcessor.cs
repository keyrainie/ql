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
    [VersionExport(typeof(ProductInventoryAdjustByLendRequestProcessor))]
    public class ProductInventoryAdjustByLendRequestProcessor : ProductInventoryAdjustProcessor
    {
        #region 抽象方法实现

        public override void SetProductInventoryAdjustInfo()
        {
            switch (this.AdjustContractInfo.SourceActionName)
            {
                case InventoryAdjustSourceAction.Create:
                    SetInventoryAdjustInfoForCreate();
                    break;
                case InventoryAdjustSourceAction.Abandon:
                    SetInventoryAdjustInfoForAbandon();
                    break;
                case InventoryAdjustSourceAction.CancelAbandon:
                    SetInventoryAdjustInfoForCancelAbandon();
                    break;
                case InventoryAdjustSourceAction.Update:
                    SetInventoryAdjustInfoForUpdate();
                    break;
                case InventoryAdjustSourceAction.OutStock:
                    SetInventoryAdjustInfoForOutStock();
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

        public override void SetCheckAvailableQtyGreaterThanZeroFlag()
        {
            this.CheckAvailableQtyGreaterThanZero = true;
        }

        #endregion 虚方法重写       

        #region 各Action的库存数量设置方法

        private void SetInventoryAdjustInfoForCreate()
        {
            SetInventoryAdjustInfoForGeneral();
        }

        private void SetInventoryAdjustInfoForAbandon()
        {
            SetInventoryAdjustInfoForGeneral();
        }

        private void SetInventoryAdjustInfoForCancelAbandon()
        {
            SetInventoryAdjustInfoForGeneral();
        }

        private void SetInventoryAdjustInfoForUpdate()
        {
            SetInventoryAdjustInfoForGeneral();
        }

        private void SetInventoryAdjustInfoForOutStock()
        {
            //借货出库, 减少财务库存/已分配库存
            this.AdjustAccountQty(this.AdjustQuantity);            
            this.AdjustAllocatedQty(this.AdjustQuantity);
            //成本处理
            this.UpdateItemUnitCost();
        }

        private void SetInventoryAdjustInfoForReturn()
        {
            //归还商品, 增加财务库存/可用库存
            this.AdjustAccountQty(this.AdjustQuantity);
            this.AdjustAvailableQty(this.AdjustQuantity);
            //不用更新成本，只有库存记录
            this.UpdateItemUnitCostIn();
        }

        private void SetInventoryAdjustInfoForGeneral()
        {
            //调整商品可用库存/已分配库存, 实际增减根据AdjustQty正负来决定
            this.AdjustAvailableQty(this.AdjustQuantity);
            this.AdjustAllocatedQty(-this.AdjustQuantity);
        }

        #endregion 各Action的库存数量设置方法

        #region 调整库存相关业务方法

        #endregion 调整库存相关业务方法
    }
}
