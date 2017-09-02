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
using ECCentral.Portal.UI.MKT.Models;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class InernetKeywordQueryFacade
    {
        private readonly RestClient _restClient;
        /// <summary>
        /// 服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get;
            private set;
        }

        public InernetKeywordQueryFacade()
        {
            _restClient = new RestClient(ServiceBaseUrl);
        }

        public InernetKeywordQueryFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            _restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="sortField"></param>
        /// <param name="callback"></param>
        public void QueryKeyword(InternetKeywordQueryVM model, int pageSize, int pageIndex, string sortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var filter = model.ConvertVM<InternetKeywordQueryVM, InternetKeywordQueryFilter>();
            filter.PageInfo = new QueryFilter.Common.PagingInfo()
            {
                PageSize = pageSize,
                PageIndex = pageIndex,
                SortBy = sortField
            };
            const string relativeUrl = "/MKTService/InternetKeyword/QueryKeyword";
            _restClient.QueryDynamicData(relativeUrl, filter, callback);
        }
       
    }
}
