using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.BizEntity.Common;
using System.Collections.Generic;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Portal.UI.Inventory.Facades
{
    public class InventoryQueryFacade
    {
        private readonly RestClient restClient;
        private readonly IPage page;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Inventory", "ServiceBaseUrl");
            }
        }
        public InventoryQueryFacade(IPage page)
        {
            this.page = page;
            this.restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryInventoryList(InventoryQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "InventoryService/Inventory/QueryInventoryList";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        public void ExportExcelForInventoryList(InventoryQueryFilter queryFilter, ColumnSet[] columns)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "InventoryService/Inventory/QueryInventoryList";
            restClient.ExportFile(relativeUrl, queryFilter, columns);
        }

        public void QueryPMMonitoringPerformanceIndicators(PMMonitoringPerformanceIndicatorsQueryFilterVM queryFilterVM, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "InventoryService/Inventory/QueryPMMonitoringPerformanceIndicators";
            var msg = queryFilterVM.ConvertVM<PMMonitoringPerformanceIndicatorsQueryFilterVM, PMMonitoringPerformanceIndicatorsQueryFilter>();
            restClient.QueryDynamicData(relativeUrl, msg, callback);
        }

        public void ExportExcelForVendors(PMMonitoringPerformanceIndicatorsQueryFilterVM request, ColumnSet[] columns)
        {
            string relativeUrl = "InventoryService/Inventory/QueryPMMonitoringPerformanceIndicators";
            restClient.ExportFile(relativeUrl, request, columns);
        }

        public void CheckOperateRightForCurrentUser(InventoryQueryFilter queryFilter, EventHandler<RestClientEventArgs<bool>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "InventoryService/Inventory/CheckOperateRightForCurrentUser";
            restClient.Query(relativeUrl, queryFilter, callback);
        }

        public void GetProductLineSysNoByProductList(InventoryQueryFilter queryFilter, EventHandler<RestClientEventArgs<List<ProductPMLine>>> callback)
        {
            string relativeUrl = "InventoryService/Inventory/GetProductLineSysNoByProductList";
            restClient.Query<List<ProductPMLine>>(relativeUrl, queryFilter, callback);
        }

        public void QueryUnmarketableInventoryInfo(int productSysNo, EventHandler<RestClientEventArgs<List<UnmarketabelInventoryInfo>>> callback)
        {
            string relativeUrl = string.Format("InventoryService/Inventory/QueryUnmarketableInventoryInfo/{0}/{1}", productSysNo, CPApplication.Current.CompanyCode);
            restClient.Query<List<UnmarketabelInventoryInfo>>(relativeUrl, callback);
        }

        /// <summary>
        /// 商品入库出库报表查询
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callback"></param>
        public void QueryCostInAndCostOutReport(CostInAndCostOutReportQueryVM model, int pageIndex, int pageSize, string sortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            CostInAndCostOutReportQueryFilter filter = model.ConvertVM<CostInAndCostOutReportQueryVM, CostInAndCostOutReportQueryFilter>();

            filter.PagingInfo = new QueryFilter.Common.PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };

            string relativeUrl = "/InventoryService/Inventory/QueryCostInAndCostOutReport";

            restClient.QueryDynamicData(relativeUrl, filter, (_, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(this, new RestClientEventArgs<dynamic>(args.Result, this.page));
            });

        }

        /// <summary>
        /// 商品入库出库报表导出
        /// </summary>
        /// <param name="model"></param>
        /// <param name="textInfoList"></param>
        /// <param name="columnSet"></param>
        public void ExportExcelForCostInAndCostOutReport(CostInAndCostOutReportQueryVM model, ColumnSet[] columnSet)
        {
            CostInAndCostOutReportQueryFilter filter = model.ConvertVM<CostInAndCostOutReportQueryVM, CostInAndCostOutReportQueryFilter>();

            filter.PagingInfo = new QueryFilter.Common.PagingInfo()
            {
                PageIndex = 0,
                PageSize = int.MaxValue,
            };

            string relativeUrl = "/InventoryService/Inventory/QueryCostInAndCostOutReport";
            restClient.ExportFile(relativeUrl, filter, columnSet);
        }

        /// <summary>
        /// 商品库龄报表查询
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortField"></param>
        /// <param name="callback"></param>
        public void QueryStockAgeReport(StockAgeReportQueryVM model, int pageIndex, int pageSize, string sortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            StockAgeReportQueryFilter filter = EntityConverter<StockAgeReportQueryVM, StockAgeReportQueryFilter>.Convert(model, (s, t) =>
            {
                t.StockAgeTypeList = s.StockAgeTypeList.Where(x => x.Value != null && x.Selected).Select(x => x.Value).ToList();
            }, "StockAgeTypeList");

            filter.PagingInfo = new QueryFilter.Common.PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };

            string relativeUrl = "/InventoryService/Inventory/QueryStockAgeReport";

            restClient.QueryDynamicData(relativeUrl, filter, (_, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(this, new RestClientEventArgs<dynamic>(args.Result, this.page));
            });
        }

        /// <summary>
        /// 商品库龄报表导出
        /// </summary>
        /// <param name="model"></param>
        /// <param name="columnSet"></param>
        public void ExportExcelForStockAgeReport(StockAgeReportQueryVM model, ColumnSet[] columnSet)
        {
            StockAgeReportQueryFilter filter = EntityConverter<StockAgeReportQueryVM, StockAgeReportQueryFilter>.Convert(model, (s, t) =>
            {
                t.StockAgeTypeList = s.StockAgeTypeList.Where(x => x.Value != null && x.Selected).Select(x => x.Value).ToList();
            }, "StockAgeTypeList");

            filter.PagingInfo = new QueryFilter.Common.PagingInfo()
            {
                PageIndex = 0,
                PageSize = int.MaxValue,
            };

            //List<TextInfo> textInfoList = new List<TextInfo>();
            //textInfoList.Add(new TextInfo() { Title = "Neticom (Hong Kong) Limited", Memo = string.Empty });
            //textInfoList.Add(new TextInfo() { Title = "As at", Memo = filter.StatisticDate.ToString("dd/MM/yyyy") });
            //textInfoList.Add(new TextInfo() { Title = "Reporting currency:", Memo = "RMB / HKD" });

            string relativeUrl = "/InventoryService/Inventory/QueryStockAgeReport";
            restClient.ExportFile(relativeUrl, filter, columnSet);
        }
    }
}
