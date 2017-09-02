using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.Service.Inventory.BizProcessor;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.Inventory.AppService
{
    [VersionExport(typeof(IInventoryBizInteract))]
    public class BizInteractAppService : IInventoryBizInteract
    {
        #region 商品库存查询

        /// <summary>
        /// 取得商品各销售渠道的商品库存
        /// </summary>
        /// <param name="ProductID">商品系统编号</param>
        /// <returns>ProductInventoryEntityList</returns>
        public virtual List<ProductInventoryInfo> GetProductInventoryInfo(int productSysNo)
        {
            return ObjectFactory<ProductInventoryAppService>.Instance.GetProductInventoryInfo(productSysNo);
        }

        /// <summary>
        /// 获取指定渠道仓库的商品库存
        /// </summary>
        /// <param name="productID">商品系统编号</param>
        /// <param name="stockSysNo">渠道仓库</param>
        /// <returns>ProductInventoryEntity</returns>
        public virtual ProductInventoryInfo GetProductInventoryInfoByStock(int productSysNo, int stockSysNo)
        {
            return ObjectFactory<ProductInventoryAppService>.Instance.GetProductInventoryInfoByStock(productSysNo, stockSysNo);
        }

        /// <summary>
        /// 根据商品系统编号查询的商品总库存
        /// </summary>
        /// <param name="ProductID">商品编号</param>
        /// <returns>ProductInventoryEntityList</returns>
        public ProductInventoryInfo GetProductTotalInventoryInfo(int productSysNo)
        {
            return ObjectFactory<ProductInventoryAppService>.Instance.GetProductTotalInventoryInfo(productSysNo);
        }


        /// <summary>
        /// 根据商品系统编号列表查询的商品总库存
        /// </summary>
        /// <param name="ProductID">商品编号列表</param>
        /// <returns>ProductInventoryEntityList</returns>
        public virtual List<ProductInventoryInfo> GetProductTotalInventoryInfoByProductList(List<int> productSysNoList)
        {
            return ObjectFactory<ProductInventoryAppService>.Instance.GetProductTotalInventoryInfoByProductList(productSysNoList);
        }

        #endregion 商品库存查询

        #region 商品库存调整

        /// <summary>
        /// 设置商品分仓库预留库存
        /// </summary>
        /// <param name="ProductID">商品编号</param>
        /// <param name="StockSysNo">渠道仓库编号</param>
        /// <param name="ReservedQty">预留库存</param>
        /// <returns></returns>
        public virtual void SetProductReservedQty(int productSysNo, int stockSysNo, int reservedQty)
        {
            ObjectFactory<ProductInventoryAppService>.Instance.SetProductReservedQty(productSysNo, stockSysNo, reservedQty);
        }

        /// <summary>
        /// 调整商品库存
        /// </summary>
        /// <param name="productInventoryAdjustInfo">商品库存调整统一格式合约信息</param>
        /// <returns>IsSucceed</returns>
        public virtual bool AdjustProductInventory(InventoryAdjustContractInfo inventoryAdjustContractInfo)
        {
            return ObjectFactory<ProductInventoryAppService>.Instance.AdjustProductInventory(inventoryAdjustContractInfo);
        }

        /// <summary>
        /// 设置渠道商品指定库存
        /// </summary>
        /// <param name="channelSysNo">渠道编号</param>
        /// <param name="productSysNo">商品编号</param>
        /// <param name="channelSellCount">指定库存数量</param>
        /// <returns>IsSucceed</returns>
        public virtual bool SetChannelProductInventory(int channelSysNo, int productSysNo, int channelSellCount)
        { 
            return true; 
        }

        /// <summary>
        /// 取消渠道商品指定库存
        /// </summary>
        /// <param name="channelSysNo">渠道编号</param>
        /// <param name="productSysNo">商品编号</param>
        /// <param name="channelSellCount">指定库存数量</param>
        /// <returns>IsSucceed</returns>
        public virtual bool AbandonChannelProductInventory(int channelSysNo, int productSysNo, int channelSellCount)
        {
            return true;
        }

        #endregion 商品库存调整

        /// <summary>
        /// 初始化商品库存
        /// </summary>
        /// <param name="ProductID">商品系统编号</param>        
        /// <returns>IsSucceed</returns>
        public virtual bool InitProductInventoryInfo(int productSysNo)
        {
            ObjectFactory<ProductInventoryAppService>.Instance.InitProductInventoryInfo(productSysNo);
            return true;
        }

        #region 基础数据

        /// <summary>
        /// 获取仓库信息
        /// </summary>
        /// <param name="warehouseNo">仓库编号</param>
        /// <returns></returns>
    
        public WarehouseInfo GetWarehouseInfoBySysNo(int warehouseNo)
        {
            return ObjectFactory<StockAppService>.Instance.GetWarehouseInfo(warehouseNo);
        }

        /// <summary>
        /// 获取仓库列表
        /// </summary>
        /// <returns></returns>
        public virtual List<WarehouseInfo> GetWarehouseList(string companyCode)
        {
            return ObjectFactory<StockAppService>.Instance.GetWarehouseList(companyCode);
        }

        ///// <summary>
        ///// 获取渠道仓库列表
        ///// </summary>
        ///// <returns></returns>
        //public virtual List<StockInfo> GetStockList(string webChannelID)
        //{
        //    return ObjectFactory<StockAppService>.Instance.GetStockList(webChannelID);
        //}

        ///// <summary>
        ///// 获取渠道仓库信息
        ///// </summary>
        ///// <returns></returns>
        //public virtual StockInfo GetStockInfo(int stockSysNo)
        //{
        //    return ObjectFactory<StockAppService>.Instance.GetStockInfo(stockSysNo);
        //}

        /// <summary>
        /// 获取本地仓库编码
        /// </summary>
        /// <param name="areaSysNo">地区编号</param>
        /// <param name="companyCode">公司编码</param>
        /// <returns>本地仓库编码</returns>
        public virtual string GetLocalWarehouseNumber(int areaSysNo, string companyCode)
        {
            return ObjectFactory<StockAppService>.Instance.GetLocalWarehouseNumber(areaSysNo,companyCode);
        }

        #endregion 基础数据

        #region 销售数量

        public virtual ProductSalesTrendInfo GetProductSalesTrendInfoByStock(int productSysNo, int stockSysNo)
        {
            return ObjectFactory<ProductInventoryAppService>.Instance.GetProductSalesTrendInfoByStock(productSysNo, stockSysNo);
        }

        public virtual List<ProductSalesTrendInfo> GetProductSalesTrendInfo(int productSysNo)
        {
            return ObjectFactory<ProductInventoryAppService>.Instance.GetProductSalesTrendInfo(productSysNo);
        }

        public virtual ProductSalesTrendInfo GetProductSalesTrendInfoTotal(int productSysNo)
        {
            return ObjectFactory<ProductInventoryAppService>.Instance.GetProductSalesTrendInfoTotal(productSysNo);
        }

        #endregion 销售数量

        #region IInventoryBizInteract Members

        public bool IsAutoShift(int shiftid, out int IsCanAbandon, out int shiftSysNo)
        {
            shiftSysNo = 0;
            IsCanAbandon = 0;
            return false;
            // throw new NotImplementedException();
        }

        public void AbandonShift(int masterSysNo)
        {
            // throw new NotImplementedException();
        }

        /// <summary>
        /// 审核移仓单
        /// </summary>
        /// <param name="ShiftSysNo">ShiftSysNo 移仓单系统编号</param>
        /// <returns>InventoryTransferEntity</returns>
        public virtual ShiftRequestInfo VerifyShiftRequest(int shiftRequestSysNo)
        {
            ShiftRequestInfo entityToUpdate = ObjectFactory<ShiftRequestProcessor>.Instance.GetShiftRequestInfoBySysNo(shiftRequestSysNo);
            entityToUpdate.AuditUser.SysNo = ServiceContext.Current.UserSysNo;
            return ObjectFactory<ShiftRequestAppService>.Instance.VerifyRequest(entityToUpdate);
        }

        public void UpdateCostInWhenCostChange(CostChangeInfo costChangeInfo)
        {
            ObjectFactory<ProductInventoryProcessor>.Instance.UpdateCostInWhenCostChange(costChangeInfo);
        }
        #endregion

        /// <summary>
        /// 获取指定商品的当前库龄
        /// 在MKT Promotion计算库龄毛利率时需要
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns>库龄天数</returns>
        public int GetInStockDaysByProductSysNo(int productSysNo)
        {
            return ObjectFactory<ProductInventoryProcessor>.Instance.GetInStockDaysByProductSysNo(productSysNo);
        }

        public bool EditGoldenTaxNo(string GoldenTaxNo, int stSysNo)
        {
            return ObjectFactory<ShiftRequestProcessor>.Instance.EditGoldenTaxNo(GoldenTaxNo, stSysNo);
        }

        public void AdjustBatchNumberInventory(string xml)
        {
            ObjectFactory<BatchInventoryProcessor>.Instance.AdjustBatchNumberInventory(xml);
        }
    }
}
