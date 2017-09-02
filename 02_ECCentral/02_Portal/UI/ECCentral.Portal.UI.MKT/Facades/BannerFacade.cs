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
    public class BannerFacade
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

        public BannerFacade(IPage page)
        {
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("MKT", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryDimensions(string companyCode, string channelID, int pageTypeID, EventHandler<RestClientEventArgs<List<BannerDimension>>> callback)
        {
            BannerDimensionQueryFilter filter = new BannerDimensionQueryFilter
            {
                CompanyCode = companyCode,
                ChannelID = channelID,
                PageTypeID = pageTypeID
            };
            string relativeUrl = "/MKTService/Banner/QueryDimensions";
            restClient.Query<List<BannerDimension>>(relativeUrl, filter, callback);

        }

        public void Query(BannerQueryVM queryVM, PagingInfo p, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var queryFilter = queryVM.ConvertVM<BannerQueryVM, BannerQueryFilter>();
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            queryFilter.PagingInfo = p;
            string relativeUrl = "/MKTService/Banner/Query";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);

        }

        public void Deactive(int sysNo, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/MKTService/Banner/Deactive/" + sysNo.ToString();
            restClient.Update(relativeUrl, null, callback);
        }

        public void Load(string sysNo, EventHandler<RestClientEventArgs<BannerLocation>> callback)
        {
            string relativeUrl = "/MKTService/Banner/Load/" + sysNo;
            restClient.Query<BannerLocation>(relativeUrl, callback);
        }

        public void Update(BannerLocationVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var model = vm.ConvertVM<BannerLocationVM, BannerLocation>((v, entity) =>
            {
                entity.Infos = v.Infos.ConvertVM<BannerInfoVM, BannerInfo>();
            }); ;
            model.CompanyCode = CPApplication.Current.CompanyCode;
            model.WebChannel = new BizEntity.Common.WebChannel { ChannelID = vm.ChannelID };
            string relativeUrl = "/MKTService/Banner/Update";
            restClient.Update(relativeUrl, model, callback);
        }

        public void Create(BannerLocationVM vm, EventHandler<RestClientEventArgs<object>> callback)
        {
            var model = vm.ConvertVM<BannerLocationVM, BannerLocation>((v, entity) =>
            {
                entity.Infos = v.Infos.ConvertVM<BannerInfoVM, BannerInfo>();
                //BannerInfo的状态和BannerLocation的状态保持一致
                entity.Infos.Status = vm.Status;
                entity.Infos.CompanyCode = CPApplication.Current.CompanyCode;
                entity.Infos.WebChannel = new BizEntity.Common.WebChannel { ChannelID = vm.ChannelID };
            }); ;
            model.CompanyCode = CPApplication.Current.CompanyCode;
            model.WebChannel = new BizEntity.Common.WebChannel { ChannelID = vm.ChannelID };
            string relativeUrl = "/MKTService/Banner/Create";
            restClient.Create(relativeUrl, model, callback);
        }

        public void CountBannerPosition(int pageID, int bannerDimensionSysNo, string channelID, EventHandler<RestClientEventArgs<int>> callback)
        {
            string relativeUrl = string.Format("/MKTService/Banner/CountBannerPosition/{0}/{1}/{2}/{3}",
                CPApplication.Current.CompanyCode,channelID,pageID,bannerDimensionSysNo);
            restClient.Query<int>(relativeUrl, callback);
        }

        public void GetBannerFrame(int pageType, int positionID, EventHandler<RestClientEventArgs<List<BannerFrame>>> callback)
        {
            BannerFrameQueryFilter filter = new BannerFrameQueryFilter
            {
                PageType = pageType,
                PositionID = positionID
            };

            string relativeUrl = "/MKTService/Banner/GetBannerFrame";
            restClient.Query<List<BannerFrame>>(relativeUrl, filter, callback);

        }
    }
}
