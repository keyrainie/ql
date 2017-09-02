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
    [VersionExport(typeof(ProductInventoryAdjustByVirtualRequestProcessor))]
    public class ProductInventoryAdjustByVirtualRequestProcessor : ProductInventoryAdjustProcessor
    {
        #region 抽象方法实现

        public override void SetProductInventoryAdjustInfo()
        {
            switch (this.AdjustContractInfo.SourceActionName)
            {
                case InventoryAdjustSourceAction.Create:
                    SetInventoryAdjustInfoForCreate();
                    break;
                case InventoryAdjustSourceAction.CreateForJob:
                    SetInventoryAdjustInfoForCreateForJob();
                    break;
                case InventoryAdjustSourceAction.Run:
                    SetInventoryAdjustInfoForRun();
                    break;
                case InventoryAdjustSourceAction.Close:
                    SetInventoryAdjustInfoForClose();
                    break;

                default:
                    throw new NotSupportedException("Not Supported Action");
            }
        }

        #endregion  抽象方法实现

        #region 虚方法重写

        public override void ProcessOriginAdjustQuantity()
        {
            //虚库申请单传入的是新的虚库量, 而不是现虚库量的增减量, 需要处理为标准的AdjustQty;
            this.AdjustQuantity = this.CurrentAdjustItemInfo.AdjustQuantity - this.StockInventoryAdjustInfo.VirtualQty;
        }

        public override void PreCheckSpecialRules()
        {
            
            if (InventoryAdjustSourceAction.Create == this.AdjustContractInfo.SourceActionName)
            {
                CheckOverflowRateOfMixedItem();
                CheckOverflowCountOfPureVirtualItem();
            }
        }

        #endregion 虚方法重写
        
        #region 各Action的库存数量设置方法
        

        private void SetInventoryAdjustInfoForCreate()
        {
            SetInventoryAdjustInfoForGeneral();
        }

        private void SetInventoryAdjustInfoForCreateForJob()
        {
            SetInventoryAdjustInfoForGeneral();
        }

        private void SetInventoryAdjustInfoForRun()
        {
            SetInventoryAdjustInfoForGeneral();
        }

        private void SetInventoryAdjustInfoForClose()
        {
            SetInventoryAdjustInfoForGeneral();
        }

        private void SetInventoryAdjustInfoForGeneral()
        {
            //调整虚拟库存
            this.AdjustVirtualQty(this.AdjustQuantity);
        }

        #endregion 各Action的库存数量设置方法

        #region 调整库存相关业务方法

        private void CheckOverflowRateOfMixedItem()
        {   
            //Call Check process from IM Domain         
        }

        private void CheckOverflowCountOfPureVirtualItem()
        {            
            //Call Check process from IM Domain         
        }

        #endregion 调整库存相关业务方法
    }
}
