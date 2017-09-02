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
using System.Collections.Generic;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class StopWordsQueryFacade
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

        public StopWordsQueryFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询阻止词
        /// </summary>
        /// <param name="callback"></param>
        public void QueryStopWords(StopWordsQueryFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/KeywordsInfo/QueryStopWords";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }
        public void ExportExcelFile(StopWordsQueryFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/QueryStopWords";
            restClient.ExportFile(relativeUrl, filter, columns);
        }

        /// <summary>
        /// 加载阻止词
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void LoadStopWordsInfo(int sysNo, EventHandler<RestClientEventArgs<StopWordsInfo>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/LoadStopWords";
            restClient.Query<StopWordsInfo>(relativeUrl, sysNo, callback);
        }

        /// <summary>
        /// 添加阻止词
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void AddStopWordsInfo(StopWordsInfo item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/CreateStopWordsInfo";
            restClient.Create(relativeUrl, item, callback);
        }

        /// <summary>
        /// 更新阻止词
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void BatchUpdateStopWords(List<StopWordsInfo> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/BatchUpdateStopWords";
            restClient.Update(relativeUrl, items, callback);
        }

    }
}
