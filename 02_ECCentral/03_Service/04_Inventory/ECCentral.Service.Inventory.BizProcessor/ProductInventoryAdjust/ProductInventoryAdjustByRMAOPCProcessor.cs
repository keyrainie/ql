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
    [VersionExport(typeof(ProductInventoryAdjustByRMAOPCProcessor))]
    public class ProductInventoryAdjustByRMAOPCProcessor : ProductInventoryAdjustProcessor
    {
        #region 私有属性

        private bool CurrentProductConsignFlag = false;

        #endregion 私有属性

        #region 抽象方法实现

        public override void SetProductInventoryAdjustInfo()
        {
            switch (AdjustContractInfo.SourceActionName)
            {
                case InventoryAdjustSourceAction.Create:
                    SetInventoryAdjustInfoForCreate();
                    break;
                case InventoryAdjustSourceAction.OutStock:
                    SetInventoryAdjustInfoForOutStock();
                    break;
                case InventoryAdjustSourceAction.InStock:
                    SetInventoryAdjustInfoForInStock();
                    break;
                case InventoryAdjustSourceAction.Reject:
                    SetInventoryAdjustInfoForReject();
                    break;
                default:
                    throw new NotSupportedException("Not Supported Order Action");
            };
        }

        #endregion  抽象方法实现

        #region 虚方法重写

        public override void LoadAdjustItemReferenceInfo()
        {
            CurrentProductConsignFlag = GetCurrentProductConsignFlag();
        }

        #endregion 虚方法重写                

        #region 各Action的库存数量设置方法

        private void SetInventoryAdjustInfoForOutStock()
        {
            //出库
            //区分代销/非代销
            if (CurrentProductConsignFlag == true)
            {
                //代销商品, 减少代销库存/已分配库存, 增加可用库存
                this.AdjustConsignQty(-this.AdjustQuantity);
                this.AdjustAvailableQty(this.AdjustQuantity);
                this.AdjustAllocatedQty(-this.AdjustQuantity);                
            }
            else
            { 
                //非代销商品, 减少财务库存/已分配库存
                this.AdjustAccountQty(-this.AdjustQuantity);
                this.AdjustAllocatedQty(-this.AdjustQuantity);
            }
        }

        private void SetInventoryAdjustInfoForInStock()
        {
            //入库, (?AdjustQty<0), (?与出库逻辑有矛盾? 需要确认)
            //区分代销/非代销
            if (CurrentProductConsignFlag == true)
            {
                //代销, 减少代销库存(AdjustQty<0), 
                this.AdjustConsignQty(this.AdjustQuantity);
            }
            else
            {
                //非代销, 减少财务库存/可用库存(AdjustQty<0)
                this.AdjustAccountQty(this.AdjustQuantity);
                this.AdjustAvailableQty(this.AdjustQuantity);
            }
        }

        private void SetInventoryAdjustInfoForReject()
        {
            //Reject RMA, AdjustQty<0
            if (CurrentProductConsignFlag == true)
            {
                //代销, 减少代销库存(AdjustQty<0), 
                this.AdjustConsignQty(this.AdjustQuantity);
            }
            else
            {
                //非代销, 减少财务库存/可用库存(AdjustQty<0)
                this.AdjustAccountQty(this.AdjustQuantity);
                this.AdjustAvailableQty(this.AdjustQuantity);
            }            
        }

        private void SetInventoryAdjustInfoForCreate()
        {
            //创建RMA, 减少可用库存, 增加已分配库存
            this.AdjustAvailableQty(-this.AdjustQuantity);
            this.AdjustAllocatedQty(this.AdjustQuantity);
        }

        #endregion 各Action的库存数量设置方法

        #region 调整库存相关业务方法

        private bool GetCurrentProductConsignFlag()
        {
            return false;
        }

        #endregion 调整库存相关业务方法
    }
}
