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
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class CategoryKPIQueryFacade
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

        public CategoryKPIQueryFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public CategoryKPIQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryCategoryKPIList(CategroyKPIQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            CategoryKPIQueryFilter filter;
            filter = model.ConvertVM<CategroyKPIQueryVM, CategoryKPIQueryFilter>();

            filter.C1SysNo = model.C1SysNo;
            filter.C2SysNo = model.C2SysNo;
            filter.C3SysNo = model.C3SysNo;
            //filter.PMUserSysNo = model.PMUserSysNo;
            filter.Status = model.Status;
            filter.CategoryType = model.CategoryType;
            filter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };

            string relativeUrl = "/IMService/CategoryKPI/QueryCategoryKPIList";
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
