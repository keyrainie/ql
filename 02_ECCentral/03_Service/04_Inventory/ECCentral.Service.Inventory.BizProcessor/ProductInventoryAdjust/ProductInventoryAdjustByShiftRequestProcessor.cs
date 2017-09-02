using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

using ECCentral.BizEntity;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.PO;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Inventory.BizProcessor
{
    [VersionExport(typeof(ProductInventoryAdjustByShiftRequestProcessor))]
    public class ProductInventoryAdjustByShiftRequestProcessor : ProductInventoryAdjustProcessor
    {
        #region 私有属性

        private ShiftRequestInfo CurrentRequestInfo = null;
        private int SourceStockSysNo = 0;
        private int TargetStockSysNo = 0;

        protected ProductInventoryInfo TargetStockInventoryCurrentInfo { get; set; }
        protected ProductInventoryInfo TargetStockInventoryAdjustInfo { get; set; }

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
                case InventoryAdjustSourceAction.AbandonForPO:
                    SetInventoryAdjustInfoForAbandonForPO();
                    break;
                case InventoryAdjustSourceAction.Update:
                    SetInventoryAdjustInfoForUpdate();
                    break;
                case InventoryAdjustSourceAction.OutStock:
                    SetInventoryAdjustInfoForOutStock();
                    break;
                case InventoryAdjustSourceAction.InStock:
                    SetInventoryAdjustInfoForInStock();
                    break;
                default:
                    throw new NotSupportedException("Not Supported Action");
            }
        }


        #endregion  抽象方法实现

        #region 虚方法重写

        public override void LoadAdjustContractReferenceInfo()
        {   
            LoadShiftRequestInfo();
        }

        public override void InitProductInventory()
        {
            //Check Inventory EXISTS
            //Init Product Inventory If NOT EXISTED
            ObjectFactory<IProductInventoryDA>.Instance.InitProductInventoryInfo(
                    this.CurrentAdjustItemInfo.ProductSysNo, SourceStockSysNo
                );

            ObjectFactory<IProductInventoryDA>.Instance.InitProductInventoryInfo(
                    this.CurrentAdjustItemInfo.ProductSysNo, TargetStockSysNo
                );
        }

        public override void LoadCurrentProductInventoryInfo()
        {
            //Init Inventory if NOT EXISTED
            InitProductInventory();

            this.StockInventoryCurrentInfo = _productInventoryDA.GetProductInventoryInfoByStock(this.CurrentAdjustItemInfo.ProductSysNo, SourceStockSysNo);
            this.TargetStockInventoryCurrentInfo = _productInventoryDA.GetProductInventoryInfoByStock(this.CurrentAdjustItemInfo.ProductSysNo, TargetStockSysNo);
            this.TotalInventoryCurrentInfo = _productInventoryDA.GetProductTotalInventoryInfo(this.CurrentAdjustItemInfo.ProductSysNo);
                  
            this.StockInventoryAdjustInfo = new ProductInventoryInfo() {
                ProductSysNo = this.CurrentAdjustItemInfo.ProductSysNo,
                StockSysNo = SourceStockSysNo    
            };

            this.TotalInventoryAdjustInfo = new ProductInventoryInfo() {
                ProductSysNo = this.CurrentAdjustItemInfo.ProductSysNo
            };
            this.TargetStockInventoryAdjustInfo = new ProductInventoryInfo() {
                ProductSysNo = this.CurrentAdjustItemInfo.ProductSysNo,
                StockSysNo = TargetStockSysNo    
            };

            ProcessOriginAdjustQuantity();

        }

        public override void UpdateProductInventoryInfo()
        {
            _productInventoryDA.AdjustProductStockInventoryInfo(this.StockInventoryAdjustInfo);
            _productInventoryDA.AdjustProductStockInventoryInfo(this.TargetStockInventoryAdjustInfo);
            _productInventoryDA.AdjustProductTotalInventoryInfo(this.TotalInventoryAdjustInfo);

             UpdateItemUnitCost();
        }

        public override void SetCheckAvailableQtyGreaterThanZeroFlag()
        {
            ///  AvailableQty在两种情况下可以为负：1，非代销，有特殊权限，2 代销
            ///  2012.11.27 代收代付也可以调整为负
            bool hasSpecialRights = this.CurrentRequestInfo.HasSpecialRightforCreate;
            bool isConsign = (this.CurrentRequestInfo.ConsignFlag == RequestConsignFlag.Consign || this.CurrentRequestInfo.ConsignFlag == RequestConsignFlag.GatherPay);
            
            this.CheckAvailableQtyGreaterThanZero = !isConsign && !hasSpecialRights;
        }

        public override void PreCheckProductInventoryInfo()
        {
            SetCheckAvailableQtyGreaterThanZeroFlag();

            ProductInventoryInfo stockInventoryAfterAdjust = PreCalculateInventoryAfterAdjust(this.StockInventoryCurrentInfo, this.StockInventoryAdjustInfo);
            ProductInventoryInfo targetStockInventoryAfterAdjust = PreCalculateInventoryAfterAdjust(this.TargetStockInventoryCurrentInfo, this.TargetStockInventoryAdjustInfo);
            ProductInventoryInfo totalInventoryAfterAdjust = PreCalculateInventoryAfterAdjust(this.TotalInventoryCurrentInfo, this.TotalInventoryAdjustInfo);

            this.PreCheckGeneralRules(stockInventoryAfterAdjust);
            this.PreCheckGeneralRules(targetStockInventoryAfterAdjust);
            this.PreCheckGeneralRules(totalInventoryAfterAdjust);

            this.PreCheckSpecialRules();
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

        private void SetInventoryAdjustInfoForAbandonForPO()
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
            //移仓单出库

            //增加总仓的移仓库存/移出仓的移出库存/移入仓的移入库存
            this.StockInventoryAdjustInfo.ShiftOutQty = this.AdjustQuantity;
            this.TotalInventoryAdjustInfo.ShiftQty = this.AdjustQuantity;
            this.TargetStockInventoryAdjustInfo.ShiftInQty = this.AdjustQuantity;
            
            //区分代销/非代销
            if (this.CurrentRequestInfo.ConsignFlag == RequestConsignFlag.Consign || this.CurrentRequestInfo.ConsignFlag == RequestConsignFlag.GatherPay)
            {
                //代销
                //总仓: 恢复可用库存, 减少已分配库存/代销库存
                this.TotalInventoryAdjustInfo.AvailableQty = this.AdjustQuantity;
                this.TotalInventoryAdjustInfo.AllocatedQty = -this.AdjustQuantity;                
                this.TotalInventoryAdjustInfo.ConsignQty = -this.AdjustQuantity;

                //移出仓: 恢复可用库存, 减少已分配库存/代销库存
                this.StockInventoryAdjustInfo.AvailableQty = this.AdjustQuantity;
                this.StockInventoryAdjustInfo.AllocatedQty = -this.AdjustQuantity;                
                this.StockInventoryAdjustInfo.ConsignQty = -this.AdjustQuantity;                
            }
            else
            { 
                //非代销                
                //移出仓:减少移出仓的财务库存/已分配库存
                this.StockInventoryAdjustInfo.AccountQty = -this.AdjustQuantity;
                this.StockInventoryAdjustInfo.AllocatedQty = -this.AdjustQuantity;
            }
            //成本处理
            this.UpdateItemUnitCost();
        }

        private void SetInventoryAdjustInfoForInStock()
        {
            //移仓单入库      
            //减少总仓的移仓库存/移出仓的移出库存/移入仓的移入库存
            this.TotalInventoryAdjustInfo.ShiftQty = -this.AdjustQuantity;
            this.StockInventoryAdjustInfo.ShiftOutQty = -this.AdjustQuantity;
            this.TargetStockInventoryAdjustInfo.ShiftInQty = -this.AdjustQuantity;

            //区分代销/非代销
            if (this.CurrentRequestInfo.ConsignFlag == RequestConsignFlag.Consign || this.CurrentRequestInfo.ConsignFlag == RequestConsignFlag.GatherPay)
            {
                //代销, 增加总仓和移入仓的代销库存
                this.TotalInventoryAdjustInfo.ConsignQty = this.AdjustQuantity;
                this.TargetStockInventoryAdjustInfo.ConsignQty = this.AdjustQuantity;
            }
            else
            { 
                //非代销, 增加目移入仓的财务库存/可用库存
                this.TargetStockInventoryAdjustInfo.AccountQty = this.AdjustQuantity;
                this.TargetStockInventoryAdjustInfo.AvailableQty = this.AdjustQuantity;
            }
            //成本处理
            this.UpdateItemUnitCost();
        }

        private void SetInventoryAdjustInfoForGeneral()
        {
            //移出仓: 调整移出仓可用库存/已分配库存, 实际增减有AdjustQty正负决定
            this.StockInventoryAdjustInfo.AvailableQty = this.AdjustQuantity;
            this.StockInventoryAdjustInfo.AllocatedQty = -this.AdjustQuantity;

            //区分代销/非代销
            if (this.CurrentRequestInfo.ConsignFlag == RequestConsignFlag.Consign || this.CurrentRequestInfo.ConsignFlag == RequestConsignFlag.GatherPay)
            {
                //代销单，调整总仓可用库存/已分配库存, 实际增减有AdjustQty正负决定 (减可用+占用)
                this.TotalInventoryAdjustInfo.AvailableQty = this.AdjustQuantity;
                //已分配库存最小调为0。                           
                this.TotalInventoryAdjustInfo.AllocatedQty = -this.AdjustQuantity;
            }            
        }

        #endregion 各Action的库存数量设置方法

        #region 调整库存相关业务方法

        private void LoadShiftRequestInfo()
        {
            int requestSysNo = int.Parse(this.AdjustContractInfo.ReferenceSysNo);
            CurrentRequestInfo = ObjectFactory<IShiftRequestDA>.Instance.GetShiftRequestInfoBySysNo(requestSysNo);
            if (CurrentRequestInfo == null)
            {
                throw new BizException(string.Format("移仓单{0}不存在.", requestSysNo));
            }

            this.TargetStockSysNo = (int)CurrentRequestInfo.TargetStock.SysNo;
            this.SourceStockSysNo = (int)CurrentRequestInfo.SourceStock.SysNo;          

        }

        private void AdjustShiftQty(int adjustQty)
        {
            this.StockInventoryAdjustInfo.ShiftQty = adjustQty;
            this.TotalInventoryAdjustInfo.ShiftQty = adjustQty;
        }

        private void AdjustShiftInQty(int adjustQty)
        {
            this.StockInventoryAdjustInfo.ShiftInQty = adjustQty;
            this.TotalInventoryAdjustInfo.ShiftInQty = adjustQty;
        }

        private void AdjustShiftOutQty(int adjustQty)
        {
            this.StockInventoryAdjustInfo.ShiftOutQty = adjustQty;
            this.TotalInventoryAdjustInfo.ShiftOutQty = adjustQty;
        }

        #endregion 调整库存相关业务方法

        #region Ref.SourceCode

        private void PreCheckShiftItemInfoForInventoryAdjust()
        {
            ////所有与库存调整的逻辑都移至库存统一调整接口

            ////Check for Create

            ////代销商品的移仓单需要更改总仓
            //bool isConsign = shiftItemList[0].ShiftProduct.ProductConsignFlag == VendorConsignFlag.Consign;

            //if (isConsign)
            //{
            //    inventoryAdjustEntity.IsConsign = 1;
            //    inventoryAdjustEntity.AdjustReasonType = InventoryAdjustReasonTypeEntity.InventoryDoc_Transfer_Consign;
            //}

            //bool checkAvailableQtyGreaterThanZero = CanAvailableQtyGreaterThanZero(newEntity.HasSepcialRightforCreate, isConsign);

            //var errorMsg = string.Empty;

            //var itemsInventoryQty = new List<BranchInventoryEntity>();

            //#region 如果有特殊移仓单创建权限，则提取对应的移仓商品的库存信息

            //if (newEntity.HasSepcialRightforCreate)
            //{
            //    itemsInventoryQty = BranchInventoryBP.GetInventory(new BranchInventoryQueryCriteriaEntity
            //    {
            //        ItemSysNumberList = itemsSysNumbers,
            //        WarehouseSysNumber = newEntity.StockSysNumberA,
            //        PageInfo = null
            //    }).ResultList;
            //}

            //#endregion


            //#region 如果有特殊移仓单制单的权限，则检查库存是否足够

            //if (!isConsign)
            //{
            //    //对于非代销商品，如果有权限，只要移仓数量小于AccountQty就可以移仓
            //    if (newEntity.HasSepcialRightforCreate)
            //    {
            //        var itemInventoryQty = itemsInventoryQty.Find(itemEntity => transferItemEntity.ItemSysNumber == itemEntity.ItemSysNumber);

            //        if (itemInventoryQty == null)
            //        {
            //            msg += string.Format(WarningMessage.InventoryTransfer_AccountQtyNotEnoughValue, item.ItemCode);
            //        }
            //        else
            //        {
            //            if (item.InventoryTransferQty > itemInventoryQty.AccountQty || item.InventoryTransferQty > (itemInventoryQty.AvailableQty + itemInventoryQty.VirtualQty + itemInventoryQty.ConsignQty))
            //            {
            //                msg += string.Format(WarningMessage.InventoryTransfer_AccountQtyNotEnoughValue, item.ItemCode);
            //            }
            //        }
            //    }
            //}

            //#region 限时抢购
            //if (inventoryAdjustEntity.IsConsign.Value == 1)
            //{
            //    int checkResultLimitedQtyORIsReservedQty = AdjustDAL.CheckBuyLimitAndIsLimitedQtyORIsReservedQty(item.ItemSysNumber);
            //    if (checkResultLimitedQtyORIsReservedQty > 0)
            //    {
            //        throw new BizException("商品编号为" + item.ItemSysNumber + "存在有限量或预留库存,且状态是就绪或运行的限时抢购记录。请先作废限时抢购记录");
            //    }

            //    int checkResultIsNotLimitedQtyANDIsNotReservedQty = AdjustDAL.CheckBuyLimitAndIsNotLimitedQtyANDIsNotReservedQty(item.ItemSysNumber, item.InventoryTransferQty, newEntity.StockSysNumberA, newEntity.CompanyCode);
            //    if (checkResultIsNotLimitedQtyANDIsNotReservedQty > 0)
            //    {
            //        throw new BizException("商品编号为" + item.ItemSysNumber + "移仓数量不能大于“代销-被占用-被订购”数量");
            //    }
            //}
            //#endregion


            //if (msg != string.Empty)
            //{
            //    errorMsg += msg;
            //    continue;
            //}

            //#endregion

            ////Check for Update

            //var errorMsg = string.Empty;

            //#region 如果有特殊移仓单制单权限，则提取对应移仓商品的库存信息

            //var itemsInventoryQty = new List<BranchInventoryEntity>();

            //if (newEntity.HasSepcialRightforCreate)
            //{
            //    itemsInventoryQty = BranchInventoryBP.GetInventory(new BranchInventoryQueryCriteriaEntity
            //    {
            //        ItemSysNumberList = itemsSysNumbers,
            //        WarehouseSysNumber = originalEntity.StockSysNumberA,
            //        PageInfo = null
            //    }).ResultList;
            //}

            //#endregion


            //#region 如果有特殊移仓单制单的权限，则检查对应的库存是否足够

            //if (!isConsign)
            //{
            //    if (newEntity.HasSepcialRightforCreate)
            //    {
            //        var transferQty = item.InventoryTransferQty;

            //        if (originalItems != null)
            //        {
            //            var oldItem = originalItems.Find(itemEntity => entity.ItemSysNumber == itemEntity.ItemSysNumber);

            //            if (oldItem != null)
            //            {
            //                transferQty = item.InventoryTransferQty - oldItem.InventoryTransferQty;
            //            }
            //        }

            //        if (transferQty > 0)
            //        {
            //            var itemInventoryQty = itemsInventoryQty.Find(itemEntity => entity.ItemSysNumber == itemEntity.ItemSysNumber);

            //            if (itemInventoryQty == null)
            //            {
            //                msg += string.Format(WarningMessage.InventoryTransfer_AccountQtyNotEnoughValue, item.ItemCode);
            //            }
            //            else
            //            {
            //                if ((itemInventoryQty.AccountQty - transferQty) < 0 || (itemInventoryQty.AvailableQty + itemInventoryQty.VirtualQty + itemInventoryQty.ConsignQty - transferQty) < 0)
            //                {
            //                    msg += string.Format(WarningMessage.InventoryTransfer_AccountQtyNotEnoughValue, item.ItemCode);
            //                }
            //            }
            //        }
            //    }
            //}

            //if (msg != string.Empty)
            //{
            //    errorMsg += msg;
            //    continue;
            //}

            //#endregion


            //#region 限时抢购 2010-9-25
            //if (inventoryAdjustEntity.IsConsign.Value == 1)
            //{
            //    int checkResultLimitedQtyORIsReservedQty = AdjustDAL.CheckBuyLimitAndIsLimitedQtyORIsReservedQty(item.ItemSysNumber);
            //    if (checkResultLimitedQtyORIsReservedQty > 0)
            //    {
            //        throw new BizException("商品编号为" + item.ItemSysNumber + "存在有限量或预留库存,且状态是就绪或运行的限时抢购记录。请先作废限时抢购记录");
            //    }

            //    int checkResultIsNotLimitedQtyANDIsNotReservedQty = AdjustDAL.CheckBuyLimitAndIsNotLimitedQtyANDIsNotReservedQty(item.ItemSysNumber, item.InventoryTransferQty, newEntity.StockSysNumberA, newEntity.CompanyCode);
            //    if (checkResultIsNotLimitedQtyANDIsNotReservedQty > 0)
            //    {
            //        throw new BizException("商品编号为" + item.ItemSysNumber + "移仓数量不能大于“代销-被占用-被订购”数量");
            //    }
            //}
            //#endregion

            ////Check For Abandon 
            //inventoryAdjustEntity.IsConsign = originalEntity.IsConsign;

            //bool isCosnign = originalEntity.IsConsign == 1;
            //if (isCosnign)
            //{
            //    inventoryAdjustEntity.AdjustReasonType = InventoryAdjustReasonTypeEntity.InventoryDoc_Transfer_Consign;
            //}

            //bool checkAvailableQtyGreaterThanZero = CanAvailableQtyGreaterThanZero(newEntity.HasSepcialRightforCreate, isCosnign);

        }        

        #endregion Ref.SourceCode
    }
}
