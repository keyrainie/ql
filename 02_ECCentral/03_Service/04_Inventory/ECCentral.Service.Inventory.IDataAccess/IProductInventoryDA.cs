using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.Inventory.IDataAccess
{
    public interface IProductInventoryDA
    {
        #region 商品库存调整统一接口相关方法

        /// <summary>
        /// 初始化商品库存记录
        /// </summary>
        /// <param name="inventoryInfo"></param>
        /// <returns></returns>
        void InitProductInventoryInfo(int productSysNo, int stockSysNo);

        /// <summary>
        /// 更新商品渠道仓库各项库存数量(复写更新)
        /// </summary>
        /// <param name="inventoryInfo"></param>
        /// <returns></returns>
        void UpdateProductStockInventoryInfo(ProductInventoryInfo inventoryInfo);

        /// <summary>
        /// 更新商品渠道总仓各项库存数量(复写更新)
        /// </summary>
        /// <param name="inventoryInfo"></param>
        /// <returns></returns>
        void UpdateProductTotalInventoryInfo(ProductInventoryInfo inventoryInfo);

        /// <summary>
        /// 更新商品渠道仓库各项库存数量(增量更新)
        /// </summary>
        /// <param name="inventoryInfo"></param>
        /// <returns></returns>
        void AdjustProductStockInventoryInfo(ProductInventoryInfo inventoryInfo);

        /// <summary>
        /// 更新商品渠道仓库各项库存数量(增量更新)
        /// </summary>
        /// <param name="inventoryInfo"></param>
        /// <returns></returns>
        void AdjustProductTotalInventoryInfo(ProductInventoryInfo inventoryInfo);

        #endregion 商品库存调整统一接口相关方法

        #region 商品库存查询

        /// <summary>
        /// 取得商品各销售渠道的商品库存
        /// </summary>
        /// <param name="ProductID">商品系统编号</param>
        /// <returns>ProductInventoryEntityList</returns>
        List<ProductInventoryInfo> GetProductInventoryInfo(int productSysNo);

        /// <summary>
        /// 获取指定渠道仓库的商品库存
        /// </summary>
        /// <param name="productID">商品系统编号</param>
        /// <param name="stockSysNo">渠道仓库</param>
        /// <returns>ProductInventoryEntity</returns>
        ProductInventoryInfo GetProductInventoryInfoByStock(int productSysNo, int stockSysNo);

        /// <summary>
        /// 获取指定商品的总库存
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        ProductInventoryInfo GetProductTotalInventoryInfo(int productSysNo);

        /// <summary>
        ///  获取指定商品列表的总库存
        /// </summary>
        /// <param name="productIDList">商品编号列表</param>
        /// <returns>ProductInventoryEntityList</returns>
        List<ProductInventoryInfo> GetProductTotalInventoryInfoByProductList(List<int> productSysNoList);

        #endregion 商品库存查询

        #region 销售数量
        /// <summary>
        ///  获取指定商品的渠道仓销售趋势
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>
        /// <param name="stockSysNo">渠道仓编号</param>
        /// <returns>ProductSalesTrendInfo</returns>
        ProductSalesTrendInfo GetProductSalesTrendInfoByStock(int productSysNo, int stockSysNo);

        /// <summary>
        ///  获取指定商品的总销售趋势
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>        
        /// <returns>ProductSalesTrendInfo</returns>
        ProductSalesTrendInfo GetProductSalesTrendInfoTotal(int productSysNo);

        /// <summary>
        ///  获取指定商品的各渠道仓销售趋势
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>        
        /// <returns>ProductSalesTrendInfo</returns>
        List<ProductSalesTrendInfo> GetProductSalesTrendInfo(int productSysNo);

        #endregion 销售数量

        #region 库存成本
        void UpdateProductCostPriority(List<ProductCostInfo> list);
        #endregion

        /// <summary>
        /// 获取商品当前库龄
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        int GetInStockDaysByProductSysNo(int productSysNo);

        void WriteCostLog(int billType,int billSysNo,string msg);

        List<ProductCostIn> GetCostList(int productSysNo);

        //给负采购用，用成本获取成本序列
        List<ProductCostIn> GetCostList(int productSysNo, decimal cost);

        //取同仓库成本
        List<ProductCostIn> GetCostList(int productSysNo, int stockSysNo);

        void UpdateProductCost( List<ProductCostOut> list);

        void UpdateProductCostForCostChange(List<ProductCostOut> list);

        void UpdateCostToBiz(List<ProductCostOut> list);

        List<ProductCostIn> GetProductCostIn(int productSysNo, int poSysNo, int stockSysNo);

        void LockProductCostInList(List<ProductCostIn> list);

        void WriteProductCost(List<ProductCostIn> ProductCostInList);

        List<InventoryBatchDetailsInfo> GetProdcutBatchesInfo(string type, int number, int productSysNo);

        List<ProductRingDetailInfo> GetProductRingDetails();
        int GetProductInventroyType(int productSysNo);
    }
}
