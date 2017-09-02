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
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class DefaultKeywordsQueryFacade
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

        public DefaultKeywordsQueryFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询默认关键字
        /// </summary>
        /// <param name="callback"></param>
        public void QueryDefaultKeywords(DefaultKeywordsQueryFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/KeywordsInfo/QueryDefaultKeywords";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void ExportExcelFile(DefaultKeywordsQueryFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/QueryDefaultKeywords";
            restClient.ExportFile(relativeUrl, filter, columns);
        }

        /// <summary>
        /// 加载默认关键字
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void LoadDefaultKeywordsInfo(int sysNo, EventHandler<RestClientEventArgs<DefaultKeywordsInfo>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/LoadDefaultKeywordsInfo/" + sysNo.ToString();
            restClient.Query<DefaultKeywordsInfo>(relativeUrl, callback);
        }

        /// <summary>
        /// 添加默认关键字
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void AddDefaultKeywords(DefaultKeywordsInfo item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/AddDefaultKeywords";
            restClient.Create(relativeUrl, item, callback);
        }

        /// <summary>
        /// 更新默认关键字
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void EditDefaultKeywords(DefaultKeywordsInfo item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/EditDefaultKeywords";
            restClient.Update(relativeUrl, item, callback);
        }
    }
}