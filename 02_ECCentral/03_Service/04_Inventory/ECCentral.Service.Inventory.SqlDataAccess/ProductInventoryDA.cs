using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Inventory;

using ECCentral.Service.Inventory.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Inventory.SqlDataAccess
{
    [VersionExport(typeof(IProductInventoryDA))]
    public class ProductInventoryDA : IProductInventoryDA
    {
        #region 商品库存调整统一接口相关方法

        /// <summary>
        /// 初始化商品库存记录
        /// </summary>
        /// <param name="inventoryInfo"></param>
        /// <returns></returns>
        public virtual void InitProductInventoryInfo(int productSysNo, int stockSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_InitProductInventoryInfo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@StockSysNo", stockSysNo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新商品渠道仓库各项库存数量(复写更新)
        /// </summary>
        /// <param name="inventoryInfo"></param>
        /// <returns></returns>
        public virtual void UpdateProductStockInventoryInfo(ProductInventoryInfo inventoryInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_UpdateProductInventoryByStock");
            cmd.SetParameterValue<ProductInventoryInfo>(inventoryInfo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新商品渠道总仓各项库存数量(复写更新)
        /// </summary>
        /// <param name="inventoryInfo"></param>
        /// <returns></returns>
        public virtual void UpdateProductTotalInventoryInfo(ProductInventoryInfo inventoryInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_UpdateProductTotalInventory");
            cmd.SetParameterValue<ProductInventoryInfo>(inventoryInfo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新商品渠道仓库各项库存数量(增量更新)
        /// </summary>
        /// <param name="inventoryInfo"></param>
        /// <returns></returns>
        public virtual void AdjustProductStockInventoryInfo(ProductInventoryInfo inventoryInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_AdjustProductInventoryByStock");
            cmd.SetParameterValue<ProductInventoryInfo>(inventoryInfo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 更新商品渠道仓库各项库存数量(增量更新)
        /// </summary>
        /// <param name="inventoryInfo"></param>
        /// <returns></returns>
        public virtual void AdjustProductTotalInventoryInfo(ProductInventoryInfo inventoryInfo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_AdjustProductTotalInventory");
            cmd.SetParameterValue<ProductInventoryInfo>(inventoryInfo);
            cmd.ExecuteNonQuery();
        }

        #endregion 商品库存调整统一接口相关方法

        #region 商品库存查询

        /// <summary>
        /// 取得商品各销售渠道的商品库存
        /// </summary>
        /// <param name="ProductID">商品系统编号</param>
        /// <returns>ProductInventoryEntityList</returns>
        public virtual List<ProductInventoryInfo> GetProductInventoryInfo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_GetProductInventoryInfo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);

            using (IDataReader reader = cmd.ExecuteDataReader())
            {
                return DataMapper.GetEntityList<ProductInventoryInfo, List<ProductInventoryInfo>>(reader);
            }
        }

        /// <summary>
        /// 获取指定渠道仓库的商品库存
        /// </summary>
        /// <param name="productID">商品系统编号</param>
        /// <param name="stockSysNo">渠道仓库</param>
        /// <returns>ProductInventoryEntity</returns>
        public virtual ProductInventoryInfo GetProductInventoryInfoByStock(int productSysNo, int stockSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_GetProductInventoryInfoByStock");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@StockSysNo", stockSysNo);
            return cmd.ExecuteEntity<ProductInventoryInfo>();
        }

        /// <summary>
        /// 获取指定商品的总库存
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public virtual ProductInventoryInfo GetProductTotalInventoryInfo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_GetProductTotalInventoryInfoByProduct");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteEntity<ProductInventoryInfo>();
        }

        /// <summary>
        ///  获取指定商品列表的总库存
        /// </summary>
        /// <param name="productIDList">商品编号列表</param>
        /// <returns>ProductInventoryEntityList</returns>
        public virtual List<ProductInventoryInfo> GetProductTotalInventoryInfoByProductList(List<int> productSysNoList)
        {
            CustomDataCommand cmd = DataCommandManager.CreateCustomDataCommandFromConfig("Inventory_GetProductTotalInventoryInfo");

            using (var sqlBuilder = new DynamicQuerySqlBuilder(cmd, "a.ProductSysNo DESC"))
            {
                sqlBuilder.ConditionConstructor.AddInCondition(QueryConditionRelationType.AND,
                    "a.ProductSysNo", DbType.Int32, productSysNoList);

                cmd.CommandText = sqlBuilder.BuildQuerySql();

                using (IDataReader reader = cmd.ExecuteDataReader())
                {
                    return DataMapper.GetEntityList<ProductInventoryInfo, List<ProductInventoryInfo>>(reader);
                }
            }

        }
        #endregion 商品库存查询

        #region 销售数量
        /// <summary>
        ///  获取指定商品的渠道仓销售趋势
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>
        /// <param name="stockSysNo">渠道仓编号</param>
        /// <returns>ProductSalesTrendInfo</returns>
        public virtual ProductSalesTrendInfo GetProductSalesTrendInfoByStock(int productSysNo, int stockSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_GetProductSalesTrendInfoByStock");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@StockSysNo", stockSysNo);
            return cmd.ExecuteEntity<ProductSalesTrendInfo>();
        }

        /// <summary>
        ///  获取指定商品的总销售趋势
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>        
        /// <returns>ProductSalesTrendInfo</returns>
        public virtual ProductSalesTrendInfo GetProductSalesTrendInfoTotal(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_GetProductSalesTrendInfoTotalByProduct");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteEntity<ProductSalesTrendInfo>();
        }

        /// <summary>
        ///  获取指定商品的各渠道仓销售趋势
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>        
        /// <returns>ProductSalesTrendInfo</returns>
        public virtual List<ProductSalesTrendInfo> GetProductSalesTrendInfo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_GetProductSalesTrendInfo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            using (IDataReader reader = cmd.ExecuteDataReader())
            {
                return DataMapper.GetEntityList<ProductSalesTrendInfo, List<ProductSalesTrendInfo>>(reader);
            }
        }
        #endregion 销售数量

        #region 库存成本
        public virtual void UpdateProductCostPriority(List<ProductCostInfo> list)
        {
            foreach (var item in list)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_UpdateProductCostPriority");
                cmd.SetParameterValue("@SysNo", item.SysNo);
                cmd.SetParameterValue("@Priority", item.Priority);
                cmd.SetParameterValue("@EditUser", ServiceContext.Current.UserSysNo);
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region 其他
        /// <summary>
        /// 获取商品当前库龄
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public int GetInStockDaysByProductSysNo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_GetInStockDaysByProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteScalar<int>();
        }

        public void WriteCostLog(int billType, int billSysNo, string msg)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_WriteCostLog");
            cmd.SetParameterValue("@BillType", billType);
            cmd.SetParameterValue("@BillSysNo", billSysNo);
            cmd.SetParameterValue("@Message", msg);
            cmd.ExecuteNonQuery();
        }

        public List<ProductCostIn> GetCostList(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_GetCostListByProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteEntityList<ProductCostIn>();
        }

        public List<ProductCostIn> GetCostList(int productSysNo, decimal cost)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_GetCostListByProductCost");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@Cost", cost);
            return cmd.ExecuteEntityList<ProductCostIn>();
        }

        public List<ProductCostIn> GetCostList(int productSysNo, int stockSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_GetCostListByProductStock");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@WarehouseNumber", stockSysNo);
            return cmd.ExecuteEntityList<ProductCostIn>();
        }

        public void UpdateProductCost(List<ProductCostOut> list)
        {
            foreach (var item in list)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_UpdateProductCost");
                cmd.SetParameterValue("@ProductSysNo", item.ProductSysNo);
                cmd.SetParameterValue("@BillType", item.BillType);
                cmd.SetParameterValue("@BillSysNo", item.BillSysNo);
                cmd.SetParameterValue("@Quantity", item.Quantity);
                cmd.SetParameterValue("@Cost", item.Cost);
                cmd.SetParameterValue("@CostInSysNo", item.CostInSysNo);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateProductCostForCostChange(List<ProductCostOut> list)
        {
            foreach (var item in list)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_UpdateProductCostForCostChange");
                cmd.SetParameterValue("@ProductSysNo", item.ProductSysNo);
                cmd.SetParameterValue("@BillType", item.BillType);
                cmd.SetParameterValue("@BillSysNo", item.BillSysNo);
                cmd.SetParameterValue("@Quantity", item.Quantity);
                cmd.SetParameterValue("@Cost", item.Cost);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateCostToBiz(List<ProductCostOut> list)
        {
            foreach (var item in list)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_UpdateCostToBiz");
                cmd.SetParameterValue("@ProductSysNo", item.ProductSysNo);
                cmd.SetParameterValue("@BillType", item.BillType);
                cmd.SetParameterValue("@BillSysNo", item.BillSysNo);
                cmd.SetParameterValue("@UnitCost", item.Cost);
                cmd.ExecuteNonQuery();
            }
        }
        public List<ProductCostIn> GetProductCostIn(int productSysNo, int poSysNo, int stockSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_GetProductCostIn");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            cmd.SetParameterValue("@PoSysNo", poSysNo);
            cmd.SetParameterValue("@WarehouseNumber", stockSysNo);
            return cmd.ExecuteEntityList<ProductCostIn>();
        }
        public void LockProductCostInList(List<ProductCostIn> list)
        {
            foreach (var item in list)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_LockProductCostInList");
                cmd.SetParameterValue("@SysNo", item.SysNo);
                cmd.SetParameterValue("@LockQuantity", item.LockQuantity);
                cmd.ExecuteNonQuery();
            }
        }
        public void WriteProductCost(List<ProductCostIn> list)
        {
            foreach (var item in list)
            {
                DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_WriteProductCost");
                cmd.SetParameterValue("@ProductSysNo", item.ProductSysNo);
                cmd.SetParameterValue("@BillType", item.BillType);
                cmd.SetParameterValue("@BillSysNo", item.BillSysNo);
                cmd.SetParameterValue("@Quantity", item.Quantity);
                cmd.SetParameterValue("@Cost", item.Cost);
                cmd.SetParameterValue("@WarehouseNumber", item.WarehouseNumber);
                cmd.ExecuteNonQuery();
            }
        }

        public List<InventoryBatchDetailsInfo> GetProdcutBatchesInfo(string type, int number, int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryBatchesByBillSysNoAndProductSysNo");
            cmd.SetParameterValue("@Type", type);
            cmd.SetParameterValue("@Number", number);
            cmd.SetParameterValue("@ProductSysNo", productSysNo);

            return cmd.ExecuteEntityList<InventoryBatchDetailsInfo>();
        }

        /// <summary>
        /// 获取库存报警信息
        /// </summary>
        /// <returns></returns>
        public List<ProductRingDetailInfo> GetProductRingDetails()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("JobGetProductRingDetails");
            return cmd.ExecuteEntityList<ProductRingDetailInfo>();
        }

        public int GetProductInventroyType(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Inventory_GetProductInventroyType");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);
            return cmd.ExecuteScalar<int>();
        }
        #endregion

    }
}
