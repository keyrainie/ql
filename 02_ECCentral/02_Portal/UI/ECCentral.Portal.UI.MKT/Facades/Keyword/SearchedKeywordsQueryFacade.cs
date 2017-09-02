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
using Newegg.Oversea.Silverlight.Controls;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class SearchedKeywordsQueryFacade
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

        public SearchedKeywordsQueryFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 加载编辑人员列表
        /// </summary>
        /// <param name="callback"></param>
        public void LoadEditUsers(string filter, EventHandler<RestClientEventArgs<List<UserInfo>>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/LoadEditUsers";
            restClient.Query<List<UserInfo>>(relativeUrl, filter, callback);
        }

        /// <summary>
        /// 查询自动匹配关键字
        /// </summary>
        /// <param name="callback"></param>
        public void QuerySearchedKeywords(SearchedKeywordsFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/KeywordsInfo/QuerySearchedKeywords";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void ExportExcelFile(SearchedKeywordsFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/QuerySearchedKeywords";
            restClient.ExportFile(relativeUrl, filter, columns);
        }
        /// <summary>
        /// 批量设置自动匹配关键字无效
        /// </summary>
        /// <param name="adv"></param>
        /// <param name="callback"></param>
        //public void SetSearchedKeywordsInvalid(List<int> items, EventHandler<RestClientEventArgs<List<int>>> callback)
        //{
        //    string relativeUrl = "/MKTService/KeywordsInfo/SetSearchedKeywordsInvalid";
        //    restClient.Update<List<int>>(relativeUrl, items, callback);
        //}

        /// <summary>
        /// 添加自动匹配关键字
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void AddSearchedKeywords(SearchedKeywords item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/AddSearchedKeywords";
            restClient.Create(relativeUrl, item, callback);
        }

        /// <summary>
        /// 更新自动匹配关键字
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void EditSearchedKeywords(SearchedKeywords item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/EditSearchedKeywords";
            restClient.Update(relativeUrl, item, callback);
        }

        /// <summary>
        /// 批量删除自动匹配关键字
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void DeleteSearchedKeywords(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/DeleteSearchedKeywords";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 批量屏蔽自动匹配关键字
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void ChangeSearchedKeywordsStatus(List<int> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/ChangeSearchedKeywordsStatus";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 加载自动匹配关键字
        /// </summary>
        /// <param name="adv"></param>
        /// <param name="callback"></param>
        public void LoadSearchedKeywords(int sysNo, EventHandler<RestClientEventArgs<SearchedKeywords>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/LoadSearchedKeywords";
            restClient.Query<SearchedKeywords>(relativeUrl, sysNo, callback);
        }
    }
}
