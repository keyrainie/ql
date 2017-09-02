using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Inventory.BizProcessor;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Inventory.AppService
{
    /// <summary>
    /// 商品库存
    /// </summary>
    [VersionExport(typeof(ProductInventoryAppService))]
    public class ProductInventoryAppService
    {
        private ProductInventoryProcessor productInventoryProcessor = ObjectFactory<ProductInventoryProcessor>.Instance;


        /// <summary>
        /// 初始化商品库存
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>         
        /// <returns></returns>
        public virtual void InitProductInventoryInfo(int productSysNo)
        {
            string webChannelID = "0"; //Get ChannelID by productSysNo, IM Domain
            productInventoryProcessor.InitProductInventoryInfo(productSysNo, webChannelID);
        }

        #region 商品库存更新

        /// <summary>
        /// 调整商品库存统一接口
        /// </summary>
        /// <param name="productInventoryAdjustInfo">商品库存调整统一格式合约信息</param>
        /// <returns>IsSucceed</returns>
        public virtual bool AdjustProductInventory(InventoryAdjustContractInfo inventoryAdjustContractInfo)
        {
            string adjustResult = ObjectFactory<InventoryAdjustContractProcessor>.Instance.ProcessAdjustContract(inventoryAdjustContractInfo);
            if (!string.IsNullOrEmpty(adjustResult))
            { 
                throw new BizException(adjustResult); 
            }
            return true;
        }

        /// <summary>
        /// 设置商品分仓库预留库存
        /// </summary>
        /// <param name="ProductID">商品编号</param>
        /// <param name="StockSysNo">渠道仓库编号</param>
        /// <param name="ReservedQty">预留库存</param>
        /// <returns></returns>
        public virtual void SetProductReservedQty(int productSysNo, int stockSysNo, int reservedQty)
        {
            productInventoryProcessor.AdjustProductReservedQty(productSysNo, stockSysNo, reservedQty);

        }
        #endregion 商品库存更新

        #region 商品库存查询

        /// <summary>
        /// 取得商品各销售渠道的商品库存
        /// </summary>
        /// <param name="ProductID">商品系统编号</param>
        /// <returns>ProductInventoryEntityList</returns>
        public virtual List<ProductInventoryInfo> GetProductInventoryInfo(int productSysNo)
        {
            return productInventoryProcessor.GetProductInventoryInfo(productSysNo);
        }

        /// <summary>
        /// 获取指定渠道仓库的商品库存
        /// </summary>
        /// <param name="productID">商品系统编号</param>
        /// <param name="stockSysNo">渠道仓库</param>
        /// <returns>ProductInventoryEntity</returns>
        public virtual ProductInventoryInfo GetProductInventoryInfoByStock(int productSysNo, int stockSysNo)
        {
            return productInventoryProcessor.GetProductInventoryInfoByStock(productSysNo, stockSysNo);
        }

        /// <summary>
        /// 获取指定商品的总库存
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public virtual ProductInventoryInfo GetProductTotalInventoryInfo(int productSysNo)
        {
            return productInventoryProcessor.GetProductTotalInventoryInfo(productSysNo);
        }

        /// <summary>
        ///  获取指定商品列表的总库存
        /// </summary>
        /// <param name="productIDList">商品编号列表</param>
        /// <returns>ProductInventoryEntityList</returns>
        public virtual List<ProductInventoryInfo> GetProductTotalInventoryInfoByProductList(List<int> productSysNoList)
        {
            if (productSysNoList == null || productSysNoList.Count == 0)
            {
                return null;
            }

            return productInventoryProcessor.GetProductTotalInventoryInfoByProductList(productSysNoList);

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
            return productInventoryProcessor.GetProductSalesTrendInfoByStock(productSysNo, stockSysNo);
        }

        /// <summary>
        ///  获取指定商品的总销售趋势
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>        
        /// <returns>ProductSalesTrendInfo</returns>
        public virtual ProductSalesTrendInfo GetProductSalesTrendInfoTotal(int productSysNo)
        {
            return productInventoryProcessor.GetProductSalesTrendInfoTotal(productSysNo);
        }

        /// <summary>
        ///  获取指定商品的各渠道仓销售趋势
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>        
        /// <returns>ProductSalesTrendInfo</returns>
        public virtual List<ProductSalesTrendInfo> GetProductSalesTrendInfo(int productSysNo)
        {
            return productInventoryProcessor.GetProductSalesTrendInfo(productSysNo);
        }

        #endregion 销售数量

        /// <summary>
        /// JOb 报警
        /// </summary>
        public virtual void GetProductRingAndSendEmail()
        {
            productInventoryProcessor.GetProductRingAndSendEmail();
        }

        #region 库存成本
        /// <summary>
        /// 批量修改库存成本优先级
        /// </summary>
        /// <param name="list"></param>
        public virtual void UpdateProductCostPriority(List<ProductCostInfo> list)
        {
            productInventoryProcessor.UpdateProductCostPriority(list);
        }
        #endregion

        #region 批次信息
        /// <summary>
        ///  修改商品批次状态
        /// </summary>
        /// <param name="productBatchInfo">商品批次信息</param>
        public virtual void UpdateProductBatchStatus(InventoryBatchDetailsInfo productBatchInfo)
        {
            productInventoryProcessor.UpdateProductBatchStatus(productBatchInfo);
        }
        #endregion
    }
}
