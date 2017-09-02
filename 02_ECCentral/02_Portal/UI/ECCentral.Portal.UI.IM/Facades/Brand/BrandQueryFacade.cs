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
using System.Collections.Generic;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class BrandQueryFacade
    {
        private readonly RestClient restClient;

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

        public BrandQueryFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public BrandQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryBrand(BrandQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            BrandQueryFilter filter;
            filter = model.ConvertVM<BrandQueryVM, BrandQueryFilter>();

            filter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };

            string relativeUrl = "/IMService/Brand/QueryBrand";
            restClient.QueryDynamicData(relativeUrl, filter,
                (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }

                    if (!(args == null || args.Result == null || args.Result.Rows == null))
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
        /// <summary>
        /// 批量置顶
        /// </summary>
        /// <param name="list"></param>
        /// <param name="callback"></param>
        public void SetTopBrands(List<string> list, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string SetTopBrandsUrl = "/IMService/Brand/SetTopBrands";
            restClient.Update(SetTopBrandsUrl, list, callback);
        }

    }
}
