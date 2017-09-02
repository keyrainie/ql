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
using ECCentral.BizEntity.Common;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.QueryFilter.MKT;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class HomePageSectionFacade
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

        public HomePageSectionFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void Query(HomePageSectionQueryVM queryVM, PagingInfo p, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var queryFilter = queryVM.ConvertVM<HomePageSectionQueryVM, HomePageSectionQueryFilter>();
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            queryFilter.PagingInfo = p;
            string relativeUrl = "/MKTService/HomePageSection/Query";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);

        }

        public void Create(HomePageSectionMaintainVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var entity = vm.ConvertVM<HomePageSectionMaintainVM, HomePageSectionInfo>();
            entity.WebChannel = new WebChannel
            {
                ChannelID = vm.ChannelID
            };
            entity.CompanyCode = CPApplication.Current.CompanyCode;
            entity.LanguageCode = "zh-CN";
            string relativeUrl = "/MKTService/HomePageSection/Create";
            restClient.Create(relativeUrl, entity, callback);
        }

        public void Update(HomePageSectionMaintainVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var entity = vm.ConvertVM<HomePageSectionMaintainVM, HomePageSectionInfo>();
            entity.WebChannel = new WebChannel
            {
                ChannelID = vm.ChannelID
            };
            entity.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/MKTService/HomePageSection/Update";
            restClient.Update(relativeUrl, entity, callback);
        }

        public void Load(string sysNo, EventHandler<RestClientEventArgs<HomePageSectionInfo>> callback)
        {
            string relativeUrl = "/MKTService/HomePageSection/" + sysNo;
            restClient.Query(relativeUrl, callback);
        }
    }
}
