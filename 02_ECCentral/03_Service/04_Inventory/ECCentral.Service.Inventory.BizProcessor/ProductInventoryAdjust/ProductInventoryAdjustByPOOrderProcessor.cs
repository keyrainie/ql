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
    [VersionExport(typeof(ProductInventoryAdjustByPOOrderProcessor))]
    public class ProductInventoryAdjustByPOOrderProcessor : ProductInventoryAdjustProcessor
    {
        #region 私有属性

        private bool IsNegativePOFalg = false;

        #endregion

        #region 抽象方法实现

        public override void SetProductInventoryAdjustInfo()
        {
            switch (this.AdjustContractInfo.SourceActionName)
            {
                case InventoryAdjustSourceAction.Audit:
                    SetInventoryAdjustInfoForAudit();
                    break;
                case InventoryAdjustSourceAction.AbandonForPO:
                case InventoryAdjustSourceAction.CancelAudit:
                    SetInventoryAdjustInfoForCancelAudit();
                    break;
                case InventoryAdjustSourceAction.StopInStock:
                    SetInventoryAdjustInfoForStopInStock();
                    break;
                case InventoryAdjustSourceAction.InStock:
                    SetInventoryAdjustInfoForInStock();
                    break;
                default:
                    throw new NotSupportedException("Not Supported Action");
            }
        }

        #endregion 抽象方法实现    

        #region 虚方法重写

        public override void LoadAdjustContractReferenceInfo()
        {
            IsNegativePOFalg = GetNegativePOFlag();            
        }
        public override void LoadCurrentProductInventoryInfo()
        {
            base.LoadCurrentProductInventoryInfo();
            //获取负po的成本库存
            this.ProductCostInList = _productInventoryDA.GetProductCostIn(this.CurrentAdjustItemInfo.ProductSysNo, Convert.ToInt32(this.AdjustContractInfo.ReferenceSysNo),this.CurrentAdjustItemInfo.StockSysNo);
        }
        public override void UpdateProductInventoryInfo()
        {
            base.UpdateProductInventoryInfo();
            //如果需要调整锁定库存
            if (this.CostLockAction != CostLockType.NoUse && this.AdjustProductCostInList!=null)
            {
                _productInventoryDA.LockProductCostInList(this.AdjustProductCostInList);
            }     
        }
        #endregion 虚方法重写       

        #region 各Action的库存数量设置方法

        private void SetInventoryAdjustInfoForInStock()
        {
            bool isConsignProduct = GetProductConsignFlag();
            bool adjustReservedQtyFlag = GetProductConsignFlag();

            //区分正负PO单
            if (IsNegativePOFalg == true)
            {
                //负PO单
                this.CostLockAction = CostLockType.Unlock;
                //区分代销商品
                if (isConsignProduct)
                {
                    //代销商品, 负PO入库, AdjustQuantity<0, 减少代销库存/已分配库存, 增加可用库存?

                    //? 负PO单审核时，已经减少了可用库存, 入库为何再调整可用库存？ 
                    //而且负PO中止入库, 增加可用库存是有道理的; 那么入库就不应该增加可用库存了。

                    this.AdjustConsignQty(this.AdjustQuantity);
                    this.AdjustAllocatedQty(this.AdjustQuantity);
                    this.AdjustAvailableQty(-this.AdjustQuantity);
                }
                else
                {
                    //非代销商品, 负PO入库, AdjustQuantity<0, 减少代销库存/已分配库存
                    this.AdjustAccountQty(this.AdjustQuantity);
                    this.AdjustAllocatedQty(this.AdjustQuantity);
                }
            }
            else
            { 
                //正PO单

                //区分代销商品
                if (isConsignProduct)
                {
                    //代销商品, 正PO入库, AdjustQuantity>0, 增加代销库存, 减少采购库存
                    this.AdjustConsignQty(this.AdjustQuantity);
                    this.AdjustPurchaseQty(-this.AdjustQuantity);
                }
                else
                {
                    //非代销商品, 正PO入库, AdjustQuantity>0, 增加财务库存, 减少采购库存
                    this.AdjustAccountQty(this.AdjustQuantity);
                    this.AdjustPurchaseQty(-this.AdjustQuantity);

                    //区分是否调整预留库存（为限时抢购商品设置）
                    if (adjustReservedQtyFlag == true)
                    {
                        //需要调预留库存,增加预留库存
                        this.AdjustReservedQty(this.AdjustQuantity);
                    }
                    else
                    { 
                        //不需要调预留库存,增加可用库存
                        this.AdjustAvailableQty(this.AdjustQuantity);                        
                    }
                }

            }
            
        }        
        private void SetInventoryAdjustInfoForStopInStock()
        {
            AdjustProductCostInList = new List<ProductCostIn>();
            //区分正负PO单
            if (this.AdjustQuantity > 0)
            {
                //负PO单中止入库, 增加可用库存量, 减少已分配库存量
                this.AdjustAvailableQty(this.AdjustQuantity);
                //调用重写的AdjustAllocatedQty方法。
                this.AdjustAllocatedQty(-this.AdjustQuantity);
                //标识释放锁定库存
                this.CostLockAction = CostLockType.Unlock;
                int temp=this.AdjustQuantity;
                //更新锁定库存
                foreach (var item in this.ProductCostInList)
                {
                    if (item.LockQuantity>=temp)
                    {
                        item.LockQuantity -= temp;
                        AdjustProductCostInList.Add(item);
                        break;
                    }
                    else if (item.LockQuantity > 0)
                    {
                        temp = temp - item.LockQuantity;
                        item.LockQuantity = 0;
                        AdjustProductCostInList.Add(item);
                    }
                }
            }
            else
            {
                //正PO单中止入库, AdjustQty<0, 减少采购库存量
                this.AdjustPurchaseQty(this.AdjustQuantity);
            }
        }

        private void SetInventoryAdjustInfoForAudit()
        {
            AdjustProductCostInList = new List<ProductCostIn>();
            int CanUseQuantity = 0;
            //区分正负PO单
            if (this.AdjustQuantity < 0)
            {
                //负PO单审核, AdjustQty<0, 减少可用库存, 增加已分配库存
                this.AdjustAvailableQty(this.AdjustQuantity);
                this.AdjustAllocatedQty(-this.AdjustQuantity);
                //标识要锁定成本库存
                this.CostLockAction = CostLockType.Lock;
                int temp =Math.Abs(this.AdjustQuantity);
                //锁定库存
                foreach (var item in this.ProductCostInList)
                {
                    CanUseQuantity = item.LeftQuantity - item.LockQuantity;
                    //可用数量大于要锁定数量，直接累加加到锁定数量
                    if (CanUseQuantity >= temp)
                    {
                        item.LockQuantity += temp;
                        AdjustProductCostInList.Add(item);
                        break;
                    }
                    else if (CanUseQuantity > 0) //可用数量不足且大于0，
                    {
                        //调整数量减少相应值，进行一次锁定分配
                        temp = temp - CanUseQuantity;
                        //将可用加到锁定数量上
                        item.LockQuantity += CanUseQuantity;
                        AdjustProductCostInList.Add(item);
                    }                       
                }
            }
            else 
            {
               //正PO单审核, AdjustQty>0, 增加采购库存
                this.AdjustPurchaseQty(this.AdjustQuantity);
            }
        }

        private void SetInventoryAdjustInfoForCancelAudit()
        {
            AdjustProductCostInList = new List<ProductCostIn>();
            //区分正负PO单
            if (this.AdjustQuantity > 0)
            {
                //负PO单取消审核, AdjustQty>0, 增加可用库存, 减少已分配库存
                this.AdjustAvailableQty(this.AdjustQuantity);
                this.AdjustAllocatedQty(-this.AdjustQuantity);
                //标识要释放成本库存
                this.CostLockAction = CostLockType.Unlock;
                int temp = this.AdjustQuantity;
                //更新锁定库存
                foreach (var item in this.ProductCostInList)
                {
                    if (item.LockQuantity >= temp)
                    {
                        item.LockQuantity -= temp;
                        AdjustProductCostInList.Add(item);
                        break;
                    }
                    else if (item.LockQuantity > 0)
                    {
                        temp = temp - item.LockQuantity;
                        item.LockQuantity = 0;
                        AdjustProductCostInList.Add(item);
                    }
                }
            }
            else 
            {
                //正PO单取消审核, AdjustQty<0, 减少采购库存
                this.AdjustPurchaseQty(this.AdjustQuantity);
            }
        }

        #endregion 各Action的库存数量设置方法

        #region 库存调整相关联业务方法

        private bool GetAdjustReservedQtyFlag()
        {
            return false;
        }

        private bool GetProductConsignFlag()
        {
            return false;
        }

        private bool GetNegativePOFlag()
        {
            return false;
        }

        #endregion 库存调整相关联业务方法
    }
}
