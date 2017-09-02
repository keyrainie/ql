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
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.QueryFilter.Common;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class BannerDimensionFacade
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

        public BannerDimensionFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void Query(BannerDimensionQueryVM queryVM, PagingInfo p, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var queryFilter = queryVM.ConvertVM<BannerDimensionQueryVM, BannerDimensionQueryFilter>();
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            queryFilter.PageInfo = p;
            string relativeUrl = "/MKTService/BannerDimension/Query";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        public void Create(BannerDimensionVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var entity = vm.ConvertVM<BannerDimensionVM, BannerDimension>();
            entity.CompanyCode = CPApplication.Current.CompanyCode;
            entity.WebChannel = new WebChannel
            {
                ChannelID = vm.ChannelID
            };
            string relativeUrl = "/MKTService/BannerDimension/Create";
            restClient.Create(relativeUrl, entity, callback);
        }

        public void Update(BannerDimensionVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var entity = vm.ConvertVM<BannerDimensionVM, BannerDimension>();
            entity.CompanyCode = CPApplication.Current.CompanyCode;
            entity.WebChannel = new WebChannel
            {
                ChannelID = vm.ChannelID
            };
            string relativeUrl = "/MKTService/BannerDimension/Update";
            restClient.Update(relativeUrl, entity, callback);
        }

        public void Load(int sysNo, EventHandler<RestClientEventArgs<BannerDimension>> callback)
        {
            string relativeUrl = "/MKTService/BannerDimension/" + sysNo.ToString();
            restClient.Query<BannerDimension>(relativeUrl, callback);
        }
    }
}
