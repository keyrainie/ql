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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Common;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class NewsFacade
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

        public NewsFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 加载编辑人员列表
        /// </summary>
        /// <param name="callback"></param>
        public void LoadCreateUsers(EventHandler<RestClientEventArgs<List<UserInfo>>> callback)
        {
            string relativeUrl = "/MKTService/NewsInfo/GetCreateUsers";
            restClient.Query(relativeUrl, CPApplication.Current.CompanyCode, callback);
        }

        /// <summary>
        /// 查询新闻公告
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void QueryNews(NewsInfoQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/NewsInfo/QueryNews";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void Update(NewsInfo filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/NewsInfo/UpdateNewsInfo";
            restClient.Update(relativeUrl, filter, callback);
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void Create(NewsInfo filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/NewsInfo/CreateNewsInfo";
            restClient.Create(relativeUrl, filter, callback);
        }
        /// <summary>
        /// Load
        /// </summary>
        /// <param name="SysNo"></param>
        /// <param name="callback"></param>
        public void GetNewsInfo(int SysNo, EventHandler<RestClientEventArgs<NewsInfo>> callback)
        {
            string relativeUrl = string.Format("/MKTService/LoadNewsInfo/{0}", SysNo);
            restClient.Query<NewsInfo>(relativeUrl, callback);
        }

        public void Deactive(List<int> sysNoList, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/NewsInfo/CancelNewsInfo";
            restClient.Update(relativeUrl, sysNoList, callback);
        }
    }
}
