//************************************************************************
// 用户名				泰隆优选
// 系统名				类别管理
// 子系统名		        类别管理查询Facades端
// 作成者				Tom
// 改版日				2012.5.14
// 改版内容				新建
//************************************************************************

using System;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;


namespace ECCentral.Portal.UI.IM.Facades
{
    public class CategoryQueryFacade
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

        public CategoryQueryFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public CategoryQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryCategory(CategoryQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            CategoryQueryFilter filter = new CategoryQueryFilter() { Type = model.Type, Category1SysNo = model.Category1SysNo, Category2SysNo = model.Category2SysNo, Status = model.Status,CategoryName=model.CategoryName};

            filter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };

            const string relativeUrl = "/IMService/Category/GetCategoryListByType";
            restClient.QueryDynamicData(relativeUrl, filter,
                (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    callback(obj, args);
                }
                );
        }
    }
}
