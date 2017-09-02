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
using ECCentral.QueryFilter.MKT;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class SubscriptionQueryFacade
    {
        
        private readonly RestClient restClient;
        /// <summary>
        /// 服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            }
        }

        public SubscriptionQueryFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public SubscriptionQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询订阅维护
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void QuerySubscription(SubscriptionQueryFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/AdvInfo/QuerySubscription";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void ExportExcelFile(SubscriptionQueryFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/MKTService/AdvInfo/QuerySubscription";
            restClient.ExportFile(relativeUrl, filter, columns);
        }
    }
}
