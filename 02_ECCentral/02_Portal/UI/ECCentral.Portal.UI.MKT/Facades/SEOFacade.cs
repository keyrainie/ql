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
    public class SEOFacade
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

        public SEOFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        ///查询SEO信息
        /// </summary>
        /// <param name="callback"></param>
        public void SEOQuery(SEOQueryFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/SEOInfo/QuerySEO";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void ExportExcelFile(SEOQueryFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/MKTService/SEOInfo/QuerySEO";
            restClient.ExportFile(relativeUrl, filter, columns);
        }

        /// <summary>
        /// 加载SEO
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void LoadSEOInfo(int sysNo, EventHandler<RestClientEventArgs<SEOItem>> callback)
        {
            string relativeUrl = "/MKTService/SEOInfo/LoadSEOInfo";
            restClient.Query<SEOItem>(relativeUrl, sysNo, callback);
        }

        /// <summary>
        /// 添加SEO维护 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void AddSEOInfo(SEOItem item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/SEOInfo/AddSEOInfo";
            restClient.Create(relativeUrl, item, callback);
        }

        /// <summary>
        /// 更新SEO维护
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void UpdateSEOInfo(SEOItem item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/SEOInfo/UpdateSEOInfo";
            restClient.Update(relativeUrl, item, callback);
        }
    }
}