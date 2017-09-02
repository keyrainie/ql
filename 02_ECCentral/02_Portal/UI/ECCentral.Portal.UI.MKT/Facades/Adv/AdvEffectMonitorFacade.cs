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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.Restful.ResponseMsg;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class AdvEffectMonitorFacade
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

        public AdvEffectMonitorFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public AdvEffectMonitorFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询广告效果
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void QueryAdvEffect(AdvEffectQueryFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/AdvInfo/QueryAdvEffect";
            //restClient.Query<AdvEffectQueryRsp>(relativeUrl, filter, callback);
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void ExportExcelFile(AdvEffectQueryFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/MKTService/AdvInfo/QueryAdvEffect";
            restClient.ExportFile(relativeUrl, filter, columns);
        }

        /// <summary>
        /// 查询广告效果所涉及的价钱总计
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        //public void QueryAdvEffectToltalPrice(AdvEffectQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        //{
        //    string relativeUrl = "/MKTService/AdvInfo/QueryAdvEffectToltalPrice";
        //    restClient.QueryDynamicData(relativeUrl, filter, callback);
        //}

        /// <summary>
        /// 查询广告效果BBS推广
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void QueryAdvEffectBBS(AdvEffectBBSQueryFilter filter, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.PageInfo = new ECCentral.QueryFilter.Common.PagingInfo()
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/MKTService/AdvInfo/QueryAdvEffectBBS";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void ExportEffectBBSExcelFile(AdvEffectBBSQueryFilter filter, ColumnSet[] columns)
        {
            string relativeUrl = "/MKTService/AdvInfo/QueryAdvEffectBBS";
            restClient.ExportFile(relativeUrl, filter, columns);
        }
    }
}
