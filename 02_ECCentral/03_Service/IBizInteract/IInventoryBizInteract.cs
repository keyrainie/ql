using System.Collections.Generic;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.PO;

namespace ECCentral.Service.IBizInteract
{
    public interface IInventoryBizInteract
    {

        #region 商品库存查询

        /// <summary>
        /// 取得商品各销售渠道的商品库存
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>
        /// <returns>ProductInventoryEntityList</returns>
        List<ProductInventoryInfo> GetProductInventoryInfo(int productSysNo);  

        /// <summary>
        /// 获取指定渠道仓库的商品库存
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>
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
        /// 获取指定销售渠道的商品总库存
        /// </summary>
        /// <param name="productIDList">商品编号</param>
        /// <returns>ProductInventoryEntityList</returns>
        List<ProductInventoryInfo> GetProductTotalInventoryInfoByProductList(List<int> productIDList);

        #endregion 商品库存查询

        #region 库存更新

        /// <summary>
        /// 设置商品分仓库预留库存
        /// </summary>
        /// <param name="ProductID">商品编号</param>
        /// <param name="StockSysNo">渠道仓库编号</param>
        /// <param name="ReservedQty">预留库存</param>
        /// <returns></returns>
        void SetProductReservedQty(int productSysNo, int stockSysNo, int reservedQty);


        /// <summary>
        /// 调整商品库存
        /// </summary>
        /// <param name="productInventoryAdjustInfo">商品库存调整统一格式合约信息</param>
        /// <returns>IsSucceed</returns>
        bool AdjustProductInventory(InventoryAdjustContractInfo inventoryAdjustContractInfo);


        /// <summary>
        /// 创建商品时初始化商品库存
        /// </summary>
        /// <param name="ProductID">商品系统编号</param>        
        /// <returns>IsSucceed</returns>
        bool InitProductInventoryInfo(int productSysNo);

        /// <summary>
        /// 设置渠道商品指定库存
        /// </summary>
        /// <param name="channelSysNo">渠道编号</param>
        /// <param name="productSysNo">商品编号</param>
        /// <param name="channelSellCount">指定库存数量</param>
        /// <returns>IsSucceed</returns>
        bool SetChannelProductInventory(int channelSysNo, int productSysNo, int channelSellCount);

        /// <summary>
        /// 取消渠道商品指定库存
        /// </summary>
        /// <param name="channelSysNo">渠道编号</param>
        /// <param name="productSysNo">商品编号</param>
        /// <param name="channelSellCount">指定库存数量</param>
        /// <returns>IsSucceed</returns>
        bool AbandonChannelProductInventory(int channelSysNo, int productSysNo, int channelSellCount);

        #endregion 库存更新

        #region 仓库基础信息

        /// <summary>
        /// 根据仓库编号获取仓库信息 Ray.L.Xing  泰隆优选定制化 ：泰隆优选不存在多渠道
        /// </summary>
        /// <param name="warehouseNo">泰隆优选仓库编号</param>
        /// <returns></returns>
        WarehouseInfo GetWarehouseInfoBySysNo(int warehouseNo);

        /// <summary>
        /// 获取仓库列表   Ray.L.Xing  泰隆优选定制化 ：泰隆优选不存在多渠道
        /// </summary>
        /// <returns></returns>
        List<WarehouseInfo> GetWarehouseList(string companyCode);

        ///// <summary>
        ///// 获取渠道仓库列表
        ///// </summary>
        ///// <returns></returns>
        //List<StockInfo> GetStockList(string webChannelID);

        ///// <summary>
        ///// 获取渠道仓库信息
        ///// </summary>
        ///// <returns></returns>
        //StockInfo GetStockInfo(int sysNo);

        /// <summary>
        /// 获取地区仓库编码
        /// </summary>
        /// <param name="areaSysNo">地区编码</param>
        /// <param name="companyCode">公司编码</param>
        /// <returns>地区仓库编码</returns>
        string GetLocalWarehouseNumber(int areaSysNo, string companyCode);

        #endregion 仓库基础信息

        #region 销售数量

        ProductSalesTrendInfo GetProductSalesTrendInfoByStock(int productSysNo, int stockSysNo);

        ProductSalesTrendInfo GetProductSalesTrendInfoTotal(int productSysNo);

        List<ProductSalesTrendInfo> GetProductSalesTrendInfo(int productSysNo);

        #endregion 销售数量

        #region 其他
        /// <summary>
        /// PO  - 判断 对应的自动移仓单已出库
        /// </summary>
        /// <param name="shiftid"></param>
        /// <param name="IsCanAbandom"></param>
        /// <param name="shiftSysNo"></param>
        /// <returns></returns>
        bool IsAutoShift(int shiftid, out int IsCanAbandon, out int shiftSysNo);

        /// <summary>
        ///  PO  - 根据 shiftSysNo 设置 St_Shift 作废
        /// </summary>
        /// <param name="masterSysNo">shiftsysno</param>
        void AbandonShift(int masterSysNo);

        /// <summary>
        /// 审核移仓单
        /// </summary>
        /// <param name="ShiftSysNo">ShiftSysNo 移仓单系统编号</param>
        /// <returns>InventoryTransferEntity</returns>
        ShiftRequestInfo VerifyShiftRequest(int shiftRequestSysNo);

        /// <summary>
        /// 获取指定商品的当前库龄
        /// 在MKT Promotion计算库龄毛利率时需要
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        int GetInStockDaysByProductSysNo(int productSysNo);

        bool EditGoldenTaxNo(string GoldenTaxNo, int stSysNo);

        /// <summary>
        /// 调整商品的批次库存
        /// </summary>
        /// <param name="xml"></param>
        void AdjustBatchNumberInventory(string xml);
        /// <summary>
        /// 成本变价单更新库存成本
        /// </summary>
        /// <param name="costChangeInfo"></param>
        /// <returns></returns>
        void UpdateCostInWhenCostChange(CostChangeInfo costChangeInfo);

        #endregion 其他
    }
}
