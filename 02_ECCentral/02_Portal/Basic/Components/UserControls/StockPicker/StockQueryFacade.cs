using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Collections.Generic;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Inventory;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Portal.Basic.Components.UserControls.StockPicker
{
    public class StockQueryFacade
    {
        private readonly RestClient restClient;
        /// <summary>
        /// InventoryService服务基地址
        /// </summary>
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Inventory", "ServiceBaseUrl");
            }
        }

        public StockQueryFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public StockQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryStock(StockQueryVM vm,PagingInfo p, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var data = vm.ConvertVM<StockQueryVM, StockQueryFilter>();
            data.PagingInfo = p;
            data.CompanyCode = "8601";//CPApplication.Current.CompanyCode;
            string relativeUrl = "/InventoryService/Stock/QueryStock";
            restClient.QueryDynamicData(relativeUrl, data, callback);
        }

        public void LoadStockBySysNo(int StockSysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            StockQueryFilter filter = new StockQueryFilter();
            filter.StockSysNo = StockSysNo.ToString();
            filter.CompanyCode = "8601";//CPApplication.Current.CompanyCode;
            filter.PagingInfo = new PagingInfo { PageIndex = 0, PageSize = int.MaxValue };
            string relativeUrl = "/InventoryService/Stock/QueryStock";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void LoadStockByID(string StockID, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            StockQueryFilter filter = new StockQueryFilter();
            filter.StockID = StockID;
            filter.CompanyCode = "8601";//CPApplication.Current.CompanyCode;
            filter.PagingInfo = new PagingInfo { PageIndex = 0, PageSize = int.MaxValue };
            string relativeUrl = "/InventoryService/Stock/QueryStock";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        /// <summary>
        /// 查询Stock List
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="callback"></param>
        public void QueryStockList(StockQueryFilter queryFilter, EventHandler<RestClientEventArgs<List<StockInfo>>> callback)
        {
            queryFilter.CompanyCode = "8601";
            string relativeUrl = "/InventoryService/Stock/QueryStockList";
            restClient.Query<List<StockInfo>>(relativeUrl, queryFilter, callback);
        }

        /// <summary>
        /// 根据WebChannelID查询Stock List
        /// </summary>
        /// <param name="webChannelID"></param>
        /// <param name="callback"></param>
        public void QueryStockListByWebChannel(string webChannelID, EventHandler<RestClientEventArgs<List<StockInfo>>> callback)
        {
            string relativeUrl = "/InventoryService/Stock/QueryStockListByWebChannelID";
            restClient.Query<List<StockInfo>>(relativeUrl, webChannelID, callback);
        }
        public void QueryStockAll(EventHandler<RestClientEventArgs<List<StockInfo>>> callback)
        {
            string relativeUrl = "/InventoryService/Stock/QueryStockAll";
            restClient.Query<List<StockInfo>>(relativeUrl, callback);
        }

        public void QueryStockListByChannelAndMerchant(int? MerchantSysNo, string webChannelID, EventHandler<RestClientEventArgs<List<StockInfo>>> callback)
        {
            StockQuerySimpleFilter filter = new StockQuerySimpleFilter();
            if (MerchantSysNo.HasValue && MerchantSysNo.Value > 0)
            {
                filter.MerchantSysNo = MerchantSysNo;
            }
            filter.WebChannelID = webChannelID;
            string relativeUrl = "/InventoryService/Stock/QueryStockListByChannelAndMerchant";
            restClient.Query<List<StockInfo>>(relativeUrl, filter, callback);
        }
        
    }
}
