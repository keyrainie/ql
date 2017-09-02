using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.Inventory;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.Inventory.IDataAccess.NoBizQuery
{
    public interface IInventoryQueryDA
    {
        /// <summary>
        /// 库存查询(总库存)
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryInventoryList(InventoryQueryFilter queryFilter, out int totalCount);

        DataTable QueryInventoryStockList(InventoryQueryFilter queryFilter, out int totalCount);

        DataTable QueryVendorInfoForBackOrderToday(BackOrderForTodayQueryFilter queryFilter, out int totalCount);

        DataTable QueryProductInventoryByStock(InventoryQueryFilter queryFilter, out int totalCount);

        DataTable QueryProductInventoryTotal(InventoryQueryFilter queryFilter, out int totalCount);

        DataTable QueryPMMonitoringPerformanceIndicators(PMMonitoringPerformanceIndicatorsQueryFilter queryFilter, out int totalCount);

        DataTable QueryProductCostInAndCostOutReport(CostInAndCostOutReportQueryFilter filter, out int totalCount);

        DataTable QueryProductStockAgeReport(StockAgeReportQueryFilter filter, out int totalCount);
        
        DataTable QueryInventoryListEndOfMouth();
        /// <summary>
        /// 获取商品归属PM
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        int GetProductBelongPMSysNo(int productSysNo);

        /// <summary>
        /// 获取商品归属供应商（取最近一次进货PO的供应商）
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        int GetProductBelongVendorSysNo(int productSysNo);

        /// <summary>
        /// 获取滞销库存详细信息
        /// </summary>
        /// <param name="productSysNo">商品系统编号</param>
        /// <param name="companyCode">公司编号</param>
        /// <returns></returns>
        List<UnmarketabelInventoryInfo> GetUnmarketableInventoryInfo(int productSysNo, string companyCode);
    }
}
