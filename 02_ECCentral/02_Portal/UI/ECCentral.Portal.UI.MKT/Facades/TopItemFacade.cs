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
    public class TopItemFacade
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

        public TopItemFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询置顶商品
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void QueryTopItemList(TopItemFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/TopItem/Query";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        /// <summary>
        /// 设置置顶商品 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="callback"></param>
        public void SetTopItem(TopItemInfo entity, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/TopItem/Create";
            restClient.Create(relativeUrl, entity, callback);
        }

        /// <summary>
        /// 取消置顶
        /// </summary>
        /// <param name="list"></param>
        /// <param name="callback"></param>
        public void CancleTopItem(List<TopItemInfo> list, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/TopItem/Cancle";
            restClient.Update(relativeUrl, list, callback);
        }

        /// <summary>
        /// load配置信息
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void QueryTopItemConfig(TopItemFilter filter, EventHandler<RestClientEventArgs<TopItemConfigInfo>> callback)
        {
            string relativeUrl = "/MKTService/TopItemConfig/Load";
            restClient.Query<TopItemConfigInfo>(relativeUrl, filter, callback);
        }
        /// <summary>
        /// 更新配置信息
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="callback"></param>
        public void UpdateTopItemConfig(TopItemConfigInfo entity, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/TopItemConfig/Update";
            restClient.Update(relativeUrl, entity, callback);
        }
    }
}
