//************************************************************************
// 用户名				泰隆优选
// 系统名				类别管理
// 子系统名		        类别管理查询Facades端
// 作成者				Tom
// 改版日				2012.5.14
// 改版内容				新建
//************************************************************************

using System;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;


namespace ECCentral.Portal.UI.IM.Facades
{
    public class CategoryExtendWarrantyQueryFacade
    {
        #region 字段以及构造函数
        private readonly RestClient restClient;
        const string GetRelativeUrl = "/IMService/CategoryExtendWarranty/QueryCategoryExtendWarranty";
        const string GetDisuseBrandRelativeUrl = "/IMService/CategoryExtendWarranty/QueryCategoryExtendWarrantyDisuseBrand";
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

        public CategoryExtendWarrantyQueryFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public CategoryExtendWarrantyQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }
        #endregion

        /// <summary>
        /// 查询分类属性
        /// </summary>
        /// <param name="model"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="SortField"></param>
        /// <param name="callback"></param>
        public void QueryCategoryExtendWarranty(CategoryExtendWarrantyQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
           
            CategoryExtendWarrantyQueryFilter filter = model.ConvertVM<CategoryExtendWarrantyQueryVM, CategoryExtendWarrantyQueryFilter>();
            if (string.IsNullOrEmpty(model.Cost))
            {
                filter.Cost = -1;
            }
            filter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };


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

        /// <summary>
        /// 查询分类属性
        /// </summary>
        /// <param name="model"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <param name="SortField"></param>
        /// <param name="callback"></param>
        public void QueryCategoryExtendWarrantyDisuseBrand(CategoryExtendWarrantyDisuseBrandQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            CategoryExtendWarrantyDisuseBrandQueryFilter filter = model.ConvertVM<CategoryExtendWarrantyDisuseBrandQueryVM, CategoryExtendWarrantyDisuseBrandQueryFilter>();

            filter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };


            restClient.QueryDynamicData(GetDisuseBrandRelativeUrl, filter,
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
