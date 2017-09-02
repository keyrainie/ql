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
using ECCentral.QueryFilter.MKT;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.MKT.NeweggCN.Facades
{
    public class BigAreaFacade
    {
        private readonly RestClient restClient;

        /// <summary>
        /// 所有的大区数据缓存。
        /// </summary>
        public static object AllBigAreas;

        /// <summary>
        /// 服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get;
            private set;
        }

        public BigAreaFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 获取所有的大区。
        /// </summary>
        /// <param name="queryVM"></param>
        /// <param name="p"></param>
        /// <param name="callback"></param>
        public void GetAllBigAreas(EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            BigAreaQueryFilter filter = new BigAreaQueryFilter();
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/MKTService/BigArea/Query";
            restClient.QueryDynamicData(relativeUrl, filter, callback);

        }
    }
}
