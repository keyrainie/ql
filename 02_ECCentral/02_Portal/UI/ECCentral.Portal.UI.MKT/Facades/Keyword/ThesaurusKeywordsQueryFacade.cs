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
    public class ThesaurusKeywordsQueryFacade
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

        public ThesaurusKeywordsQueryFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询同义词
        /// </summary>
        /// <param name="callback"></param>
        public void QueryThesaurusKeywords(ThesaurusKeywordsQueryFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/KeywordsInfo/QueryThesaurusKeywords";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }
        public void ExportExcelFile(ThesaurusKeywordsQueryFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/QueryThesaurusKeywords";
            restClient.ExportFile(relativeUrl, filter, columns);
        }

        /// <summary>
        /// 添加同义词
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void AddThesaurusWords(ThesaurusInfo item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/AddThesaurusWords";
            restClient.Create(relativeUrl, item, callback);
        }

        /// <summary>
        /// 更新同义词
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        //public void EditThesaurusWords(ThesaurusInfo item, EventHandler<RestClientEventArgs<dynamic>> callback)
        //{
        //    string relativeUrl = "/MKTService/KeywordsInfo/EditThesaurusWords";
        //    restClient.Update(relativeUrl, item, callback);
        //}

        /// <summary>
        /// 更新阻止词
        /// </summary>
        /// <param name="items"></param>
        /// <param name="callback"></param>
        public void BatchUpdateThesaurusInfo(List<ThesaurusInfo> items, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/BatchUpdateThesaurusInfo";
            restClient.Update(relativeUrl, items, callback);
        }

        /// <summary>
        /// 加载同义词
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void LoadThesaurusWords(int sysNo, EventHandler<RestClientEventArgs<ThesaurusInfo>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/LoadThesaurusWords";
            restClient.Query<ThesaurusInfo>(relativeUrl, sysNo, callback);
        }
    }
}
