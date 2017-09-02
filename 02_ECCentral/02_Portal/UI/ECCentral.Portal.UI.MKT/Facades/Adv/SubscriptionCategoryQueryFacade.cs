using System;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.QueryFilter.MKT;
using System.Collections.Generic;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class SubscriptionCategoryQueryFacade
    {
        private readonly RestClient _restClient;
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

        public SubscriptionCategoryQueryFacade()
        {
            _restClient = new RestClient(ServiceBaseUrl);
        }

        public SubscriptionCategoryQueryFacade(IPage page)
        {
            _restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询订阅分类
        /// </summary>
        /// <param name="callback">回调函数</param>
        public void QuerySubscriptionCategory(EventHandler<RestClientEventArgs<List<SubscriptionCategory>>> callback)
        {
            const string relativeUrl = "/MKTService/AdvInfo/QuerySubscriptionCategory";
            _restClient.Query(relativeUrl,null,callback);
        }
    }
}
