using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.Service.Utility;
using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using System.Transactions;

namespace ECCentral.Service.Inventory.BizProcessor
{
    [VersionExport(typeof(ProductInventoryAdjustProcessor))]
    public abstract class ProductInventoryAdjustProcessor : IProductInventoryAdjustProcessor
    {
        protected IProductInventoryDA _productInventoryDA = ObjectFactory<IProductInventoryDA>.Instance;

        #region 业务属性

        protected InventoryAdjustContractInfo AdjustContractInfo { get; set; }
        protected InventoryAdjustItemInfo CurrentAdjustItemInfo { get; set; }
        protected int AdjustQuantity { get; set; }

        protected ProductInventoryInfo StockInventoryCurrentInfo { get; set; }
        protected ProductInventoryInfo TotalInventoryCurrentInfo { get; set; }
        protected ProductInventoryInfo StockInventoryAdjustInfo { get; set; }
        protected ProductInventoryInfo TotalInventoryAdjustInfo { get; set; }

        protected ProductInventoryInfo StockInventoryAdjustAfterAdjust { get; set; }
        protected ProductInventoryInfo TotalInventoryAdjustAfterAdjust { get; set; }

        protected bool CheckAvailableQtyGreaterThanZero = false;
        public string AdjustResultMsg { get; set; }

        protected List<ProductCostIn> ProductCostInList { get; set; }
        protected List<ProductCostIn> AdjustProductCostInList { get; set; }
        public CostLockType CostLockAction { get; set; }
        public int invenoryType { get; set; }
        #endregion

        #region 抽象方法

        public abstract void SetProductInventoryAdjustInfo();

        #endregion

        #region 库存调整虚方法

        public virtual void AdjustProductInventory(InventoryAdjustContractInfo adjustContractInfo)
        {
            this.AdjustContractInfo = adjustContractInfo;
            ProcessAdjustContractInfo();
        }

        public virtual void ProcessAdjustContractInfo()
        {
            //加载相关单据信息
            LoadAdjustContractReferenceInfo();

            foreach (InventoryAdjustItemInfo adjustItem in this.AdjustContractInfo.AdjustItemList)
            {
                this.CurrentAdjustItemInfo = adjustItem;
                this.AdjustQuantity = adjustItem.AdjustQuantity;
                ProcessAdjustItemInfo();
            }
        }

        public virtual void ProcessAdjustItemInfo()
        {
            //加载库存调整单据相关信息
            LoadAdjustItemReferenceInfo();

            //获取待调商品的当前库存
            LoadCurrentProductInventoryInfo();

            //设置商品的各库存数量的调整值
            SetProductInventoryAdjustInfo();

            //预检调整后的商品库存是否合法  
            PreCheckProductInventoryInfo();

            //调整商品库存
            UpdateProductInventoryInfo();

        }

        public virtual void LoadCurrentProductInventoryInfo()
        {
            //PreCheck Product Info
            PreCheckProductInfo();

            //Init Inventory if NOT EXISTED
            InitProductInventory();
            invenoryType = _productInventoryDA.GetProductInventroyType(this.CurrentAdjustItemInfo.ProductSysNo);
            this.StockInventoryCurrentInfo = _productInventoryDA.GetProductInventoryInfoByStock(this.CurrentAdjustItemInfo.ProductSysNo, this.CurrentAdjustItemInfo.StockSysNo);
            this.TotalInventoryCurrentInfo = _productInventoryDA.GetProductTotalInventoryInfo(this.CurrentAdjustItemInfo.ProductSysNo);
            this.StockInventoryAdjustInfo = new ProductInventoryInfo()
            {
                ProductSysNo = this.CurrentAdjustItemInfo.ProductSysNo,
                StockSysNo = this.CurrentAdjustItemInfo.StockSysNo
            };
            this.TotalInventoryAdjustInfo = new ProductInventoryInfo()
            {
                ProductSysNo = this.CurrentAdjustItemInfo.ProductSysNo
            };

            ProcessOriginAdjustQuantity();
        }

        public virtual void LoadAdjustContractReferenceInfo()
        {
            //Do Nothing. For override;
        }

        public virtual void LoadAdjustItemReferenceInfo()
        {
            //Do Nothing. For override;
        }

        public virtual void ProcessOriginAdjustQuantity()
        {
            //AdjustQty的定义是调整增减量,如果Client传入的不是,则需要变换为库存增减量
            //For override
        }

        public virtual void UpdateProductInventoryInfo()
        {

            if (invenoryType == (int)ProductInventoryType.Company
                || invenoryType == (int)ProductInventoryType.GetShopInventory
                 || invenoryType == (int)ProductInventoryType.TwoDoor)
            {
                return;
            }
            _productInventoryDA.AdjustProductStockInventoryInfo(this.StockInventoryAdjustInfo);
            _productInventoryDA.AdjustProductTotalInventoryInfo(this.TotalInventoryAdjustInfo);
        }

        #region PreCheck
        public virtual void PreCheckProductInfo()
        {
            ProductInfo productInfo = ExternalDomainBroker.GetProductInfoByProductSysNo(this.CurrentAdjustItemInfo.ProductSysNo);
            if (productInfo == null || productInfo.SysNo <= 0)
            {
                throw new BizException(string.Format("欲调库存的商品不存在，商品编号：{0}", this.CurrentAdjustItemInfo.ProductSysNo));
            }
        }

        public virtual void PreCheckProductInventoryInfo()
        {
            SetCheckAvailableQtyGreaterThanZeroFlag();

            this.StockInventoryAdjustAfterAdjust = PreCalculateInventoryAfterAdjust(this.StockInventoryCurrentInfo, this.StockInventoryAdjustInfo);
            this.TotalInventoryAdjustAfterAdjust = PreCalculateInventoryAfterAdjust(this.TotalInventoryCurrentInfo, this.TotalInventoryAdjustInfo);

            bool isNeedCompareAvailableQtyAndAccountQty = true;
            this.PreCheckGeneralRules(this.StockInventoryAdjustAfterAdjust, ref isNeedCompareAvailableQtyAndAccountQty);
            this.PreCheckGeneralRules(this.TotalInventoryAdjustAfterAdjust, ref isNeedCompareAvailableQtyAndAccountQty);

            this.PreCheckSpecialRules();
        }

        public virtual void PreCheckGeneralRules(ProductInventoryInfo inventoryInfo)
        {
            bool isNeedCompareAvailableQtyAndAccountQty = true;
            PreCheckGeneralRules(inventoryInfo, ref isNeedCompareAvailableQtyAndAccountQty);
        }

        private void PreCheckGeneralRules(ProductInventoryInfo inventoryInfo, ref bool isNeedCompareAvailableQtyAndAccountQty)
        {
            string commError = "不能将可用库存调整为负数，也不能使可卖数量为负数，可卖数量=可用库存+虚拟库存+代销库存.";
            #region 检查库存量是否小于0
            if (invenoryType == (int)ProductInventoryType.Company
                || invenoryType == (int)ProductInventoryType.GetShopInventory
                 || invenoryType == (int)ProductInventoryType.TwoDoor)
            {
                return;
            }
            if (inventoryInfo.AccountQty < 0)
            {
                throw new BizException("财务库存不能小于0!");
            }

            if (this.CheckAvailableQtyGreaterThanZero && inventoryInfo.AvailableQty < 0)
            {
                throw new BizException(commError);
            }

            if (inventoryInfo.OrderQty < 0)
            {
                throw new BizException("已订购数量不能小于0!");
            }

            if (inventoryInfo.ConsignQty < 0)
            {
                throw new BizException(commError);
            }

            if (inventoryInfo.AllocatedQty < 0)
            {
                throw new BizException("已分配库存不能小于0！");
            }

            #endregion  检查库存量是否小于0

            #region 检查相关库存量之间的逻辑规则

            if (inventoryInfo.AvailableQty + inventoryInfo.VirtualQty + inventoryInfo.ConsignQty < 0)
            {
                throw new BizException(commError);
            }

            if (inventoryInfo.StockInfo != null && inventoryInfo.StockInfo.SysNo.HasValue)
            {
                //if product is in MKT's Stock, need not run this way
                //var mktStockList = AppSettingManager.GetSetting("Inventory", "MKTVirtualInventory").Split(',').Select(p => int.Parse(p));泰隆修改，将泰隆自己仓库配置起来
                var realStockList = AppSettingManager.GetSetting("Inventory", "RealInventory").Split(',').Select(p => int.Parse(p));
                if (!realStockList.Contains(inventoryInfo.StockInfo.SysNo.Value))
                {
                    isNeedCompareAvailableQtyAndAccountQty = false;
                }
            }

            if (isNeedCompareAvailableQtyAndAccountQty && inventoryInfo.AvailableQty > inventoryInfo.AccountQty)
            {
                throw new BizException("财务库存不能小于可用库存！");
            }

            //@IsDCOrder=0 && Product PromotionType='DC'
            //if (inventoryInfo.OrderQty < inventoryInfo.ReservedQty)
            //{
            //    result.Append("?");
            //}

            #endregion 检查相关库存量之间的逻辑规则
        }

        public virtual void PreCheckSpecialRules()
        {
            //For override;            
        }

        public virtual ProductInventoryInfo PreCalculateInventoryAfterAdjust(ProductInventoryInfo currentInfo, ProductInventoryInfo adjustInfo)
        {
            return new ProductInventoryInfo()
            {
                AllocatedQty = currentInfo.AllocatedQty + adjustInfo.AllocatedQty,
                AccountQty = currentInfo.AccountQty + adjustInfo.AccountQty,
                AvailableQty = currentInfo.AvailableQty + adjustInfo.AvailableQty,
                ConsignQty = currentInfo.ConsignQty + adjustInfo.ConsignQty,
                VirtualQty = currentInfo.VirtualQty + adjustInfo.VirtualQty,
                OrderQty = currentInfo.OrderQty + adjustInfo.OrderQty,
                PurchaseQty = currentInfo.PurchaseQty + adjustInfo.PurchaseQty,
                ReservedQty = currentInfo.ReservedQty + adjustInfo.ReservedQty,
                ShiftQty = currentInfo.ShiftQty + adjustInfo.ShiftQty,
                ShiftInQty = currentInfo.ShiftInQty + adjustInfo.ShiftInQty,
                ShiftOutQty = currentInfo.ShiftOutQty + adjustInfo.ShiftOutQty,
                ChannelQty = currentInfo.ChannelQty + adjustInfo.ChannelQty,
                StockInfo = currentInfo.StockInfo != null ? new StockInfo() { SysNo = currentInfo.StockInfo.SysNo } : null
            };
        }

        #endregion PreCheck


        #region 与库存调整联动的逻辑

        public virtual void InitProductInventory()
        {
            //Check Inventory EXISTS
            //Init Product Inventory If NOT EXISTED
            ObjectFactory<IProductInventoryDA>.Instance.InitProductInventoryInfo(
                    this.CurrentAdjustItemInfo.ProductSysNo, this.CurrentAdjustItemInfo.StockSysNo
                );
        }


        public virtual bool GetUpdateItemUnitCostFlag()
        {
            //TODO: Get OpenSwitch for UpdateUnitCost
            return false;
        }


        public virtual void SetCheckAvailableQtyGreaterThanZeroFlag()
        {
            this.CheckAvailableQtyGreaterThanZero = false;
        }

        #endregion

        #endregion 虚方法

        #region 各库存量调整方法

        protected void AdjustAccountQty(int adjustQty)
        {
            this.StockInventoryAdjustInfo.AccountQty = adjustQty;
            this.TotalInventoryAdjustInfo.AccountQty = adjustQty;
        }

        protected void AdjustAvailableQty(int adjustQty)
        {
            this.StockInventoryAdjustInfo.AvailableQty = adjustQty;
            this.TotalInventoryAdjustInfo.AvailableQty = adjustQty;
        }

        protected void AdjustAllocatedQty(int adjustQty)
        {
            if (adjustQty < 0)
            {
                //AllocatedQty(-,->0),小于0则自动调为0。       
                if (this.StockInventoryCurrentInfo.AllocatedQty + adjustQty < 0)
                {
                    this.StockInventoryAdjustInfo.AllocatedQty = -this.StockInventoryCurrentInfo.AllocatedQty;
                }
                else
                {
                    this.StockInventoryAdjustInfo.AllocatedQty = adjustQty;
                }

                if (this.TotalInventoryCurrentInfo.AllocatedQty + adjustQty < 0)
                {
                    this.TotalInventoryAdjustInfo.AllocatedQty = -this.TotalInventoryCurrentInfo.AllocatedQty;
                }
                else
                {
                    this.TotalInventoryAdjustInfo.AllocatedQty = adjustQty;
                }

            }
            else
            {
                this.StockInventoryAdjustInfo.AllocatedQty = adjustQty;
                this.TotalInventoryAdjustInfo.AllocatedQty = adjustQty;
            }
        }

        protected void AdjustOrderQty(int adjustQty)
        {
            this.StockInventoryAdjustInfo.OrderQty = adjustQty;
            this.TotalInventoryAdjustInfo.OrderQty = adjustQty;
        }

        protected void AdjustConsignQty(int adjustQty)
        {
            this.StockInventoryAdjustInfo.ConsignQty = adjustQty;
            this.TotalInventoryAdjustInfo.ConsignQty = adjustQty;
        }

        protected void AdjustPurchaseQty(int adjustQty)
        {
            this.StockInventoryAdjustInfo.PurchaseQty = adjustQty;
            this.TotalInventoryAdjustInfo.PurchaseQty = adjustQty;
        }

        protected void AdjustReservedQty(int adjustQty)
        {
            this.StockInventoryAdjustInfo.ReservedQty = adjustQty;
            this.TotalInventoryAdjustInfo.ReservedQty = adjustQty;
        }

        protected void AdjustVirtualQty(int adjustQty)
        {
            this.StockInventoryAdjustInfo.VirtualQty = adjustQty;
            this.TotalInventoryAdjustInfo.VirtualQty = adjustQty;
        }

        protected void AdjustChannelQty(int adjustQty)
        {
            this.StockInventoryAdjustInfo.ChannelQty = adjustQty;
            this.TotalInventoryAdjustInfo.ChannelQty = adjustQty;
        }

        #endregion 各库存量调整方法

        #region 先进先出成本相关方法
        /// <summary>
        /// 记入成本逻辑，不同业务需要取不同成本,由子类实现{包括还货单、损益单、转换单}
        /// 需要从AdjustItemList中过滤出AdjustQuantity大于0的记录进行处理
        /// 最终将costin集合赋给ProductCostInList属性
        /// </summary>
        public virtual void UnitCostToCostIn()
        {
            ProductCostIn costIn;
            ProductCostInList = new List<ProductCostIn>();

            foreach (InventoryAdjustItemInfo item in this.AdjustContractInfo.AdjustItemList)
            {
                //特殊库存模式不处理库存成本 by feegod 2013.10.05
                int inventory = _productInventoryDA.GetProductInventroyType(item.ProductSysNo);
                if (inventory == (int)ProductInventoryType.Company
                || inventory == (int)ProductInventoryType.GetShopInventory
                 || inventory == (int)ProductInventoryType.TwoDoor)
                {
                    continue;
                }
                if (item.AdjustQuantity > 0 && item.ProductSysNo == this.CurrentAdjustItemInfo.ProductSysNo)   //库存溢出表示为入库
                {
                    costIn = new ProductCostIn();
                    costIn.BillType = (int)this.AdjustContractInfo.CostType;
                    costIn.BillSysNo = int.Parse(this.AdjustContractInfo.ReferenceSysNo);
                    costIn.Quantity = item.AdjustQuantity;
                    costIn.LeftQuantity = item.AdjustQuantity;
                    costIn.LockQuantity = 0;
                    costIn.ProductSysNo = item.ProductSysNo;
                    costIn.Cost = item.AdjustUnitCost;
                    costIn.WarehouseNumber = item.StockSysNo;
                    ProductCostInList.Add(costIn);
                }
            }

            _productInventoryDA.WriteProductCost(ProductCostInList);
        }
        /// <summary>
        /// 先入先出成本扣减
        /// 需要从AdjustItemList中过滤出AdjustQuantity小于0的记录进行处理
        /// </summary>
        public virtual void UnitCostToCostOut()
        {
            List<ProductCostOut> dbCostOut = new List<ProductCostOut>();
            #region 先入先出逻辑
            //从集合中取数量为负的表示库存减少
            var outlist = this.AdjustContractInfo.AdjustItemList.Where(p => p.AdjustQuantity < 0 && p.ProductSysNo == this.CurrentAdjustItemInfo.ProductSysNo);
            int AdjustQty = 0;
            foreach (var item in outlist)
            {
                //特殊库存模式不处理库存成本 by feegod 2013.10.05
                int inventory = _productInventoryDA.GetProductInventroyType(item.ProductSysNo);
                if (inventory == (int)ProductInventoryType.Company
                || inventory == (int)ProductInventoryType.GetShopInventory
                 || inventory == (int)ProductInventoryType.TwoDoor)
                {
                    continue;
                }
                //获取成本序列开始递减算法
                var costlist = _productInventoryDA.GetCostList(item.ProductSysNo,item.StockSysNo);
                AdjustQty = Math.Abs(item.AdjustQuantity);
                foreach (var cost in costlist)
                {
                    if (cost.LeftQuantity - cost.LockQuantity >= AdjustQty)
                    {
                        ProductCostOut costout = new ProductCostOut
                        {
                            CostInSysNo = cost.SysNo,
                            Quantity = AdjustQty,
                            Cost = cost.Cost,
                            BillSysNo = int.Parse(this.AdjustContractInfo.ReferenceSysNo),
                            BillType = (int)this.AdjustContractInfo.CostType,
                            ProductSysNo = item.ProductSysNo,
                            WarehouseNumber = item.StockSysNo,
                        };
                        dbCostOut.Add(costout);
                        //足额抵扣，跳出成本分配循环
                        break;
                    }
                    //单条记录不足抵扣
                    else
                    {
                        ProductCostOut costout = new ProductCostOut
                        {
                            CostInSysNo = cost.SysNo,
                            Quantity = cost.LeftQuantity - cost.LockQuantity,//调整数=剩余数
                            Cost = cost.Cost,
                            BillSysNo = int.Parse(this.AdjustContractInfo.ReferenceSysNo),
                            BillType = (int)this.AdjustContractInfo.CostType,
                            ProductSysNo = item.ProductSysNo,
                            WarehouseNumber = item.StockSysNo,
                        };
                        dbCostOut.Add(costout);
                        //剩余数量继续抵扣，需要更新当前数量
                        AdjustQty -= (cost.LeftQuantity - cost.LockQuantity);
                    }
                }
            }
            #endregion
            //进行DB操作，dbCostOut包含此单据所有出库分配成本的集合,添加出库成本记录，更新入库序列相应数量，
            #region 处理单个商品，多个成本的情况
            var outBizGroup = dbCostOut.GroupBy(p => new { p.BillType, p.BillSysNo, p.ProductSysNo });
            List<ProductCostOut> outCostBiz = new List<ProductCostOut>();
            foreach (var item in outBizGroup)
            {
                ProductCostOut outbiz = new ProductCostOut
                {
                    BillType = item.Key.BillType,
                    BillSysNo = item.Key.BillSysNo,
                    ProductSysNo = item.Key.ProductSysNo,
                    //多成本分段计算加和再取平均成本，保留三位小数
                    Cost = Math.Round(item.Sum(p => p.Cost * p.Quantity) / item.Sum(p => p.Quantity), 3),
                };
                outCostBiz.Add(outbiz);
            }
            #endregion
            //将分配成本写成本分配表CostOut，同时更新至业务表中

            _productInventoryDA.UpdateProductCost(dbCostOut);
            _productInventoryDA.UpdateCostToBiz(outCostBiz);

        }
        /// <summary>
        /// 成本变化前先写日志，供子类调用
        /// </summary>
        public void WtriteCostLog()
        {
            string msg = SerializationUtility.XmlSerialize(this.AdjustContractInfo);
            _productInventoryDA.WriteCostLog((int)this.AdjustContractInfo.CostType, int.Parse(this.AdjustContractInfo.ReferenceSysNo), msg);
        }

        public virtual void UpdateItemUnitCost()
        {
            using (TransactionScope tran = new TransactionScope())
            {
                WtriteCostLog();
                UnitCostToCostIn();
                UnitCostToCostOut();
                tran.Complete();
            }
        }

        public virtual void UpdateItemUnitCostIn()
        {
            using (TransactionScope tran = new TransactionScope())
            {
                WtriteCostLog();
                UnitCostToCostIn();
                tran.Complete();
            }
        }
        #endregion

    }
}
