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

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class AdvancedKeywordsQueryFacade
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

        public AdvancedKeywordsQueryFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询跳转关键字
        /// </summary>
        /// <param name="callback"></param>
        public void QueryAdvancedKeywords(AdvancedKeywordsQueryFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()   //这个PagingInfo是我们在ECCentral.BizEntity里自己定义的，只有这3个属性
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/KeywordsInfo/QueryAdvancedKeywords";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void ExportExcelFile(AdvancedKeywordsQueryFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/QueryAdvancedKeywords";
            restClient.ExportFile(relativeUrl, filter, columns);
        }

        /// <summary>
        /// 加载跳转关键字
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void LoadAdvancedKeywordsInfo(int sysNo, EventHandler<RestClientEventArgs<AdvancedKeywordsInfo>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/LoadAdvancedKeywordsInfo";
            restClient.Query<AdvancedKeywordsInfo>(relativeUrl, sysNo, callback);
        }

        /// <summary>
        /// 添加跳转关键字
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void AddAdvancedKeywords(AdvancedKeywordsInfo item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/AddAdvancedKeywords";
            restClient.Create(relativeUrl, item, callback);
        }

        /// <summary>
        /// 更新跳转关键字
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void EditAdvancedKeywords(AdvancedKeywordsInfo item, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/KeywordsInfo/EditAdvancedKeywords";
            restClient.Update(relativeUrl, item, callback);
        }
    }
}
