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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.MKT;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class AdvertiserFacade
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

        public AdvertiserFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public AdvertiserFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询广告商
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void QueryAdvertiser(AdvertiserQueryFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/AdvInfo/QueryAdvertiser";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void ExportExcelFile(AdvertiserQueryFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/MKTService/AdvInfo/QueryAdvertiser";
            restClient.ExportFile(relativeUrl, filter, columns);
        }

        /// <summary>
        /// 加载广告商
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void LoadAdvertiser(int sysNo, EventHandler<RestClientEventArgs<Advertisers>> callback)
        {
            string relativeUrl = "/MKTService/AdvInfo/LoadAdvertiser";
            restClient.Query<Advertisers>(relativeUrl, sysNo, callback);
        }

        /// <summary>
        /// 添加广告商
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void AddAdvertiser(Advertisers adv, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/AdvInfo/CreateAdvertisers";
            restClient.Create(relativeUrl, adv, callback);
        }

        /// <summary>
        /// 更新广告商
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void UpdateAdvertiser(Advertisers adv, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/AdvInfo/UpdateAdvertisers";
            restClient.Update(relativeUrl, adv, callback);
        }

        /// <summary>
        /// 批量设置有效
        /// </summary>
        /// <param name="adv"></param>
        /// <param name="callback"></param>
        public void SetAdvertiserValid(List<int> adv, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/AdvInfo/SetAdvertiserValid";
            restClient.Update(relativeUrl, adv, callback);
        }

        /// <summary>
        /// 批量设置无效
        /// </summary>
        /// <param name="adv"></param>
        /// <param name="callback"></param>
        public void SetAdvertiserInvalid(List<int> adv, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/MKTService/AdvInfo/SetAdvertiserInvalid";
            restClient.Update(relativeUrl, adv, callback);
        }
    }
}
