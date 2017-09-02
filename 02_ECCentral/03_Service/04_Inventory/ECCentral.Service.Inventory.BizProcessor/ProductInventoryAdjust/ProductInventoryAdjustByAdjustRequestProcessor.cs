using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

using ECCentral.BizEntity;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.Inventory.BizProcessor
{
    [VersionExport(typeof(ProductInventoryAdjustByAdjustRequestProcessor))]
    public class ProductInventoryAdjustByAdjustRequestProcessor : ProductInventoryAdjustProcessor
    {
        #region 私有属性
        
        private AdjustRequestInfo CurrentRequestInfo = null;

        #endregion  私有属性

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
                //影响成本
                case InventoryAdjustSourceAction.OutStock:
                    SetInventoryAdjustInfoForOutStock();
                    break;
                default:
                    throw new NotSupportedException("Not Supported Action");
            }
        }

        #endregion 抽象方法实现

        #region 虚方法重写

        public override void LoadAdjustContractReferenceInfo()
        {
            int requestSysNo = int.Parse(this.AdjustContractInfo.ReferenceSysNo);
            CurrentRequestInfo = ObjectFactory<IAdjustRequestDA>.Instance.GetAdjustRequestInfoBySysNo(requestSysNo);
        }

        public override void SetCheckAvailableQtyGreaterThanZeroFlag()
        {
            //非代销商品检查AvailableQty>0, 代销忽略
            this.CheckAvailableQtyGreaterThanZero = CurrentRequestInfo.ConsignFlag == RequestConsignFlag.NotConsign;
        }

        public override void PreCheckSpecialRules()
        {
            //创建损益单时特殊检查
            if (this.AdjustContractInfo.SourceActionName == InventoryAdjustSourceAction.Create)
            {
                if (CurrentRequestInfo.ConsignFlag == RequestConsignFlag.Consign || CurrentRequestInfo.ConsignFlag == RequestConsignFlag.GatherPay)
                {
                    List<CountdownInfo> countDownList = ExternalDomainBroker.GetReadyOrRunningCountDownByProductSysNo(this.CurrentAdjustItemInfo.ProductSysNo);
                    if (ExternalDomainBroker.CheckBuyLimitAndIsLimitedQtyORIsReservedQty(countDownList))
                    {
                        throw new BizException("商品编号为" + this.CurrentAdjustItemInfo.ProductSysNo + "存在有限量或预留库存,且状态是就绪或运行的限时抢购记录。请先作废限时抢购记录");
                    }
                    if (ExternalDomainBroker.CheckBuyLimitAndIsNotLimitedQtyANDIsNotReservedQty(countDownList))
                    {
                        int consignQty = this.StockInventoryAdjustAfterAdjust.ConsignQty;
                        int allocatedQty = this.StockInventoryAdjustAfterAdjust.AllocatedQty;
                        int orderQty = this.StockInventoryAdjustAfterAdjust.OrderQty;

                        if (consignQty - allocatedQty - orderQty < this.CurrentAdjustItemInfo.AdjustQuantity)
                        {
                            throw new BizException("商品编号为" + this.CurrentAdjustItemInfo.ProductSysNo + "损单数量不能大于“代销-被占用-被订购”数量");
                        }
                    }

                }    
            }
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
            if (this.AdjustQuantity < 0)
            {
                //损单出库
                if (CurrentRequestInfo.ConsignFlag == RequestConsignFlag.Consign || CurrentRequestInfo.ConsignFlag == RequestConsignFlag.GatherPay)
                {
                    //代销商品, 恢复可用库存, 减少已分配库存/代销库存                   
                    this.AdjustAvailableQty(-this.AdjustQuantity);
                    this.AdjustAllocatedQty(this.AdjustQuantity);
                    this.AdjustConsignQty(this.AdjustQuantity);
                }
                else
                {
                    //非代销商品, 减少财务库存/已分配库存
                    this.AdjustAccountQty(this.AdjustQuantity);
                    this.AdjustAllocatedQty(this.AdjustQuantity);
                }
            }
            else
            {
                //溢单出库
                if (CurrentRequestInfo.ConsignFlag == RequestConsignFlag.Consign ||CurrentRequestInfo.ConsignFlag == RequestConsignFlag.GatherPay)
                {
                    //代销商品, 增加代销库存 (损/溢单都增加代销库存?)
                    this.AdjustConsignQty(this.AdjustQuantity);                    
                }
                else
                { 
                   //非代销商品, 增加财务库存/可用库存
                    this.AdjustAccountQty(this.AdjustQuantity);
                    this.AdjustAvailableQty(this.AdjustQuantity);
                }
            }

            //成本处理
            this.UpdateItemUnitCost();
        }       

        private void SetInventoryAdjustInfoForGeneral()
        {
            //调整可用库存/已分配库存,如果AdjustQty>0，则是增加可用库存/减少已分配库存。
            //如果AdjustQty<0, 则是减少可用库存/增加已分配库存
            this.AdjustAvailableQty(this.AdjustQuantity);
            this.AdjustAllocatedQty(-this.AdjustQuantity);
        }

        #endregion 各Action的库存数量设置方法

        #region 调整库存相关业务逻辑
  
        #endregion 调整库存相关业务逻辑
    }
}
