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
    [VersionExport(typeof(ProductInventoryAdjustByConvertRequestProcessor))]
    public class ProductInventoryAdjustByConvertRequestProcessor : ProductInventoryAdjustProcessor
    {
        #region 私有属性

        private ConvertProductType? CurrentProductConvertType = ConvertProductType.Source;

        #endregion 私有属性

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
                default:
                    throw new NotSupportedException("Not Supported Action");
            }
        }

        #endregion  抽象方法实现

        #region 虚方法重写

        public override void LoadAdjustItemReferenceInfo()
        {
            CurrentProductConvertType = GetCurrentItemConvertType();   
        }

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
            if (!CurrentProductConvertType.HasValue)
            {
                throw new BizException("转换单库存调整失败！");
            }   

            if (CurrentProductConvertType == ConvertProductType.Source)
            {
                //源商品, 减少财务库存/已分配库存
                this.AdjustAccountQty(-Math.Abs(this.AdjustQuantity));
                this.AdjustAllocatedQty(-Math.Abs(this.AdjustQuantity));
            }
            else
            { 
                //目标商品, 增加财务库存/已分配库存
                this.AdjustAccountQty(Math.Abs(this.AdjustQuantity));
                this.AdjustAvailableQty(Math.Abs(this.AdjustQuantity));            
            }
            //成本处理
            this.UpdateItemUnitCost();
        }

        private void SetInventoryAdjustInfoForGeneral()
        {            

            //调可用库存/已分配库存, 实际增减根据AdjustQty正负来确定。
            this.AdjustAvailableQty(this.AdjustQuantity);
            this.AdjustAllocatedQty(-this.AdjustQuantity);
        }

        #endregion 各Action的库存数量设置方法


        #region 调整库存相关业务逻辑

        private ConvertProductType? GetCurrentItemConvertType()
        {
            if (this.CurrentAdjustItemInfo.AdjustItemBizFlag.HasValue)
            {
                return (ConvertProductType)this.CurrentAdjustItemInfo.AdjustItemBizFlag;
            }

            return null;            
        }

        #endregion 调整库存相关业务逻辑
    }
}
