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
using Newegg.Oversea.Silverlight.ControlPanel.Core;

using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class ProductQueryPriceChangeLogFacade
    {
        private readonly RestClient restClient;
        private const string GetProductNotifyUrl = "/IMService/ProductQueryPriceChangeLog/GetProductQueryPriceChangeLog";

        /// <summary>
        /// IMService服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("IM", "ServiceBaseUrl"); 
            }
        }

        public ProductQueryPriceChangeLogFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public ProductQueryPriceChangeLogFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }
    
        public void GetProductQueryPriceChangeLog(ProductQueryPriceChangeLogQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ProductPriceChangeLogQueryFilter query = new ProductPriceChangeLogQueryFilter();
            query = model.ConvertVM<ProductQueryPriceChangeLogQueryVM, ProductPriceChangeLogQueryFilter>();
            query.PageInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            restClient.QueryDynamicData(GetProductNotifyUrl, query, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }
    }
}
