using System;
using System.Linq;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.QueryFilter.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.Inventory.Facades
{
    public class StockQueryFacade
    {
        private readonly RestClient restClient;

        /// <summary>
        /// IMService服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_Inventory, ConstValue.Key_ServiceBaseUrl);
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

        public void QueryStock(StockQueryVM model, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            StockQueryFilter filter = model.ConvertVM<StockQueryVM, StockQueryFilter>();           
            string relativeUrl = "/InventoryService/Stock/QueryStock";
            restClient.QueryDynamicData(relativeUrl, filter,
                (obj, args) =>
                {

                    if (!args.FaultsHandle())
                    {
                        foreach (var item in args.Result.Rows)
                        {
                            item.IsChecked = false;
                        }
                        callback(obj, args);
                    }
                });
        }

        /// <summary>
        /// 导出全部
        /// </summary>
        /// <param name="model"></param>
        /// <param name="columns"></param>
        public void ExportExcelForStockQuery(StockQueryVM model, ColumnSet[] columns)
        {
            StockQueryFilter queryFilter;
            model.CompanyCode = CPApplication.Current.CompanyCode;
            queryFilter = model.ConvertVM<StockQueryVM, StockQueryFilter>();
            string relativeUrl = "/InventoryService/Stock/QueryStock";
            restClient.ExportFile(relativeUrl, queryFilter, columns);
        }        
    }
}
