//************************************************************************
// 用户名				泰隆优选
// 系统名				生产商管理
// 子系统名		        生产商管理查询Facades端
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
using System.ServiceModel.DomainServices.Client;

namespace ECCentral.Portal.UI.IM.Facades
{
    public class ManufacturerQueryFacade
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

        public ManufacturerQueryFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public ManufacturerQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryManufacturer(ManufacturerQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ManufacturerQueryFilter filter;
            filter = model.ConvertVM<ManufacturerQueryVM, ManufacturerQueryFilter>();

            filter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };

            string relativeUrl = "/IMService/Manufacturer/QueryManufacturer";
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

        public void QueryManufacturerCategory(string manufacturerSysNo, int pageSize, int pageIndex, string sortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            ManufacturerQueryFilter filter = new ManufacturerQueryFilter();
            filter.PagingInfo = new PagingInfo
            {
                PageSize = pageSize,
                PageIndex = pageIndex,
                SortBy = sortField
            };
            filter.ManufacturerID = manufacturerSysNo.ToString();
            string relativeUrl = "/IMService/Manufacturer/QueryManufacturerCategory";
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
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
