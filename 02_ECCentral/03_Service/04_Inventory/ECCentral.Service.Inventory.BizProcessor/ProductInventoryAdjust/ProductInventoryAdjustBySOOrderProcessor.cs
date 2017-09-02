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
    [VersionExport(typeof(ProductInventoryAdjustBySOOrderProcessor))]
    public class ProductInventoryAdjustBySOOrderProcessor : ProductInventoryAdjustProcessor
    {
        #region 抽象方法实现

        public override void SetProductInventoryAdjustInfo()
        {
            switch (AdjustContractInfo.SourceActionName)
            {
                case InventoryAdjustSourceAction.Create:
                    SetInventoryAdjustInfoForOrderCreate();
                    break;
                case InventoryAdjustSourceAction.Abandon:
                    if (AdjustContractInfo.IsOutStockAbandon)
                    {
                        SetInventoryAdjustInfoForOutStockAbandon();
                    }
                    else
                    {
                        SetInventoryAdjustInfoForOrderVoid(false);
                    }
                    break;
                case InventoryAdjustSourceAction.Abandon_RecoverStock:
                    if (AdjustContractInfo.IsOutStockAbandon)
                    {
                        SetInventoryAdjustInfoForOutStockAbandon();
                    }
                    else
                    {
                        SetInventoryAdjustInfoForOrderVoid(true);
                    }
                    break;
                case InventoryAdjustSourceAction.OutStock:
                    SetInventoryAdjustInfoForOrderShipOut();
                    break;
                case InventoryAdjustSourceAction.WHUpdate:
                    SetInventoryAdjustInfoForOrderWHUpdate();
                    break;
                case InventoryAdjustSourceAction.Pending:
                    SetInventoryAdjustInfoForOrderPending();
                    break;
                default:
                    throw new NotSupportedException("Not Supported Order Action");
            };
        }

        #endregion 抽象方法实现


        #region 虚方法重写  

        public override void LoadAdjustContractReferenceInfo()
        {
          
        }     

        #endregion 虚方法重写

        #region 各Action的库存数量设置方法

        private InventoryAdjustContractInfo BuildAdjustContractInfoForVoidSO(string orderSysNo)
        {
            //根据orderSysNo获取原订单的商品，构造调库ContractInfo
            InventoryAdjustContractInfo contractInfoForVoidSO = new InventoryAdjustContractInfo();
            
            return contractInfoForVoidSO;
        }

        private void SetInventoryAdjustInfoForOrderCreate()
        {
            //销售订单创建

            //减可销售库存
            this.AdjustAvailableQty(-this.AdjustQuantity);

            //加已订购库存
            this.AdjustOrderQty(+this.AdjustQuantity);


        }
   
        private void SetInventoryAdjustInfoForOrderVoid(bool stockRecoverFlag)
        {
            //销售订单作废

            //返还可销售库存
            this.AdjustAvailableQty(+this.AdjustQuantity);

            //减少已订购库存
            this.AdjustOrderQty(-this.AdjustQuantity);
        }

        private void SetInventoryAdjustInfoForOutStockAbandon()
        {
            //申报失败作废、清关作废

            //返还可销售库存
            this.AdjustAvailableQty(+this.AdjustQuantity);

            //减少已订购库存
            this.AdjustOrderQty(-this.AdjustQuantity);

        }


        private void SetInventoryAdjustInfoForOrderShipOut()
        {      
            //KJT 自贸仓 扣减代销库存  返还可销售库存
            if (this.StockInventoryCurrentInfo != null
                && this.StockInventoryCurrentInfo.StockInfo != null
                && this.StockInventoryCurrentInfo.StockInfo.MerchantSysNo == 1
                && this.StockInventoryCurrentInfo.StockInfo.StockType == BizEntity.Common.TradeType.FTA)
            {
                //扣减代销库存
                this.AdjustConsignQty(-this.AdjustQuantity);
                //返还可销售库存
                this.AdjustAvailableQty(+this.AdjustQuantity);
            }

            //减少已订购库存
            this.AdjustOrderQty(-this.AdjustQuantity);

            //成本处理
            this.UpdateItemUnitCost();
        }

        private void SetInventoryAdjustInfoForOrderWHUpdate()
        {
            //仓库更新销售订单, 增加已订购库存, 减少可用库存
            this.AdjustOrderQty(this.AdjustQuantity);
            this.AdjustAvailableQty(-this.AdjustQuantity);
        }

        private void SetInventoryAdjustInfoForOrderPending()
        {
            //销售订单Pending, 增加已订购库存, 减少可用库存
            this.AdjustOrderQty(this.AdjustQuantity);
            this.AdjustAvailableQty(-this.AdjustQuantity);
        }


        #endregion 各Action的库存数量设置方法

        #region 库存调整相关联业务方法

        private bool GetOrderShipByMerchantFlag()
        {
            int soSysNo = Convert.ToInt32(this.AdjustContractInfo.ReferenceSysNo);
            var soInfo = ExternalDomainBroker.GetSOInfo(soSysNo);
            if (soInfo == null || soInfo.BaseInfo == null)
            {
                //todo:需要抛出业务异常
                //throw new BusinessException("找不到指定的订单信息")
                return false;
            }
            return soInfo.ShippingInfo.ShippingType == BizEntity.Invoice.DeliveryType.MET;
        }

        private bool GetAdjustReservedQtyFlag()
        {
            return false;
        }

        private void UpdateSOItemUnitCostForOutStock()
        { 
        
        }
        #endregion
    }
}
