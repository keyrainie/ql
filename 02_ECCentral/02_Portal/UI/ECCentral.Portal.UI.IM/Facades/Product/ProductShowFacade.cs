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
    public class ProductShowFacade
    {
         private readonly RestClient restClient;
         private const string GetProductShowUrl = "/IMService/ProductShow/GetProductShowByQuery";
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

        public ProductShowFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public ProductShowFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }
        /// <summary>
        /// 根据query得到上架商品信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="SortField"></param>
        /// <param name="callback"></param>
        public void GetProductRelatedByQuery(ProductShowQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {

            ProductShowQueryFilter query;
            query = model.ConvertVM<ProductShowQueryVM, ProductShowQueryFilter>();
            query.PageInfo = new PagingInfo() { PageIndex = PageIndex, PageSize = PageSize, SortBy = SortField };
            restClient.QueryDynamicData(GetProductShowUrl, query, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }
        public void ExprotExecl(ProductShowQueryVM model, ColumnSet[] columns)
        {

            ProductShowQueryFilter query;
            query = model.ConvertVM<ProductShowQueryVM, ProductShowQueryFilter>();
            query.PageInfo = new PagingInfo();
            restClient.ExportFile(GetProductShowUrl, query, columns);
        }
    }
}
