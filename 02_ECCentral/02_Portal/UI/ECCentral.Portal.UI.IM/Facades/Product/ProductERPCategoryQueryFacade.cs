using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models.Product;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
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

namespace ECCentral.Portal.UI.IM.Facades
{
    public class ProductERPCategoryQueryFacade
    {
        #region 字段以及构造函数
        private readonly RestClient restClient;
        const string GetRelativeUrl = "/IMService/ProductERPCategory/QueryProductERPCategoryList";

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

        public ProductERPCategoryQueryFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public ProductERPCategoryQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }
        #endregion

        /// <summary>
        /// 查询ERP大类码
        /// </summary>
        /// <param name="model"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="SortField"></param>
        /// <param name="callback"></param>
        public void QueryProductERPCategoryList(ProductERPCategoryQueryVM model, PagingInfo p, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ProductERPCategoryQueryFilter filter = model.ConvertVM<ProductERPCategoryQueryVM, ProductERPCategoryQueryFilter>();

            filter.PagingInfo = p;

            restClient.QueryDynamicData(GetRelativeUrl, filter,
                (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    if (!(args.Result == null || args.Result.Rows == null))
                    {
                        foreach (var item in args.Result.Rows)
                        {
                            item.IsChecked = false;
                        }
                    }
                    callback(obj, args);
                }
                );
        }
    }
}
