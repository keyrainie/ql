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
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class CategoryKeywordsQueryFacade
    {
        private readonly RestClient restClient;
        /// <summary>
        /// 服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get;
            private set;
        }

        public CategoryKeywordsQueryFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询分类关键字
        /// </summary>
        /// <param name="callback"></param>
        public void QueryCategoryKeywords(CategoryKeywordsQueryFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/KeywordsInfo/QueryCategoryKeywords";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        /// <summary>
        /// 检查是否存在该通用关键字，是否需要覆盖
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        public void CheckCategoryKeywordsC3SysNo(CategoryKeywords item, EventHandler<RestClientEventArgs<bool>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/CheckCategoryKeywordsC3SysNo";
            restClient.Query<bool>(relativeUrl, item, callback);
        }

        #region 三级类通用关键字

        ///// <summary>
        ///// 加载类通用关键字
        ///// </summary>
        ///// <param name="sysNo"></param>
        ///// <param name="callback"></param>
        //public void LoadCommonKeyWords(int sysNo, EventHandler<RestClientEventArgs<CommonKeyWords>> callback)
        //{
        //    string relativeUrl = "/MKTService/KeywordsInfo/LoadCommonKeyWords";
        //    restClient.Query<CommonKeyWords>(relativeUrl, sysNo, callback);
        //}

        /// <summary>
        /// 添加类通用关键字
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        public void AddCommonKeyWords(CategoryKeywords item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/AddCommonKeyWords";
            restClient.Create(relativeUrl, item, callback);
        }

        /// <summary>
        /// 更新类通用关键字
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        public void UpdateCommonKeyWords(CategoryKeywords item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/UpdateCommonKeyWords";
            restClient.Update(relativeUrl, item, callback);
        }
        #endregion

        #region 三级类别属性关键字


        /// <summary>
        /// 获取三级类别下的属性列表
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetPropertyByCategory(int sysNo, EventHandler<RestClientEventArgs<List<ECCentral.BizEntity.IM.CategoryProperty>>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/GetPropertyByCategory";
            restClient.Query<List<ECCentral.BizEntity.IM.CategoryProperty>>(relativeUrl, sysNo, callback);
        }

        /// <summary>
        /// 加载类别属性关键字
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        //public void LoadPropertyKeywords(int sysNo, EventHandler<RestClientEventArgs<PropertyKeywords>> callback)
        //{
        //    string relativeUrl = "/MKTService/KeywordsInfo/LoadPropertyKeywords";
        //    restClient.Query<PropertyKeywords>(relativeUrl, sysNo, callback);
        //}

        /// <summary>
        /// 添加类别属性关键字
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        public void AddPropertyKeywords(CategoryKeywords item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/AddPropertyKeywords";
            restClient.Create(relativeUrl, item, callback);
        }

        /// <summary>
        /// 更新类别属性关键字
        /// </summary>
        /// <param name="item"></param>
        /// <param name="callback"></param>
        public void UpdatePropertyKeywords(CategoryKeywords item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/UpdatePropertyKeywords";
            restClient.Update(relativeUrl, item, callback);
        }
        #endregion

        public void ExportExcelFile(CategoryKeywordsQueryFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/QueryCategoryKeywords";
            restClient.ExportFile(relativeUrl, filter, columns);
        }

    }
}
