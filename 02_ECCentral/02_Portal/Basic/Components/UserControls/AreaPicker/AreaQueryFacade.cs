using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Components.UserControls.AreaPicker;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Common;
using ECCentral.Service.Common.Restful.ResponseMsg;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.Basic.Components.UserControls.AreaPicker
{
    public class AreaQueryFacade
    {
        private readonly RestClient restClient;

        public AreaQueryFacade()
        {
            restClient = new RestClient(CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Common", "ServiceBaseUrl") + "/CommonService");
        }

        /// <summary>
        /// 通过AreaSysNo查询AreaInfo
        /// </summary>
        /// <param name="sysNo">AreaSysNo</param>
        /// <param name="callback">服务端数据返回后客户端回调委托</param>
        public void GetAreaBySysNo(string sysNo, EventHandler<RestClientEventArgs<AreaInfo>> callback)
        {
            string relativeUrl = string.Format("/Area/Load/{0}", sysNo);
            restClient.Query<AreaInfo>(relativeUrl, callback);
        }

        public void QueryAreaList(AreaQueryVM req, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            AreaQueryFilter filter = new AreaQueryFilter()
            {
                ProvinceSysNumber = req.ProvinceSysNumber,
                CitySysNumber = req.CitySysNumber,
                DistrictSysNumber = req.DistrictSysNumber,
                SysNo = req.DistrictSysNumber,
                PagingInfo = new QueryFilter.Common.PagingInfo()
                {
                    PageIndex = PageIndex,
                    PageSize = PageSize,
                    SortBy = SortField
                }
            };
            string relativeUrl = "/Area/QueryAreaNoDistrictList";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        /// <summary>
        /// 查询所有省AreaInfo列表
        /// </summary>
        /// <param name="callback">服务端数据返回后客户端回调委托</param>
        public void QueryProvinceAreaList(EventHandler<RestClientEventArgs<List<AreaInfo>>> callback)
        {
            string relativeUrl = "/Area/QueryProvinceAreaList";
            restClient.Query<List<AreaInfo>>(relativeUrl, callback);
        }

        /// <summary>
        /// 根据省的AreaSysNo查询城市AreaInfo列表
        /// </summary>
        /// <param name="provinceSysNo">城市的AreaSysNo</param>
        /// <param name="callback">服务端数据返回后客户端回调委托</param>
        public void QueryCityAreaListByProvinceSysNo(int provinceSysNo, EventHandler<RestClientEventArgs<List<AreaInfo>>> callback)
        {
            string relativeUrl = string.Format("/Area/QueryCityAreaListByProvinceSysNo/{0}", provinceSysNo);
            restClient.Query<List<AreaInfo>>(relativeUrl, callback);
        }

        /// <summary>
        /// 根据城市的AreaSysNo查询地区的AreaInfo列表
        /// </summary>
        /// <param name="citySysNo">城市的AreaSysNo</param>
        /// <param name="callback">服务端数据返回后客户端回调委托</param>
        public void QueryDistrictAreaListByCitySysNo(int citySysNo, EventHandler<RestClientEventArgs<List<AreaInfo>>> callback)
        {
            string relativeUrl = string.Format("/Area/QueryProvinceAreaList/{0}", citySysNo);
            restClient.Query<List<AreaInfo>>(relativeUrl, callback);
        }

        /// <summary>
        /// 根据AreaSysNo查询包含该地区的省、市、区AreaInfo结构。
        /// 如果查询的Area不存在，则返回的结果包含完整的省列表，市和区列表为空；
        /// 如果查询的Area是省，则返回的结果包含完整的省列表和该省对应的下级城市列表，区列表为空；
        /// 如果查询的Area是市，则返回的结果包含完整的省列表和与该市同级的城市列表以及该城市对应的下级地区列表；
        /// 如果查询的Area是区，则返回完整的与该地区对应的省、市、区结构。
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void QueryCurrentAreaStructure(int sysNo, EventHandler<RestClientEventArgs<AreaQueryResponse>> callback)
        {
            string relativeUrl = string.Format("/Area/QueryCurrentAreaStructure/{0}", sysNo);
            restClient.Query<AreaQueryResponse>(relativeUrl, callback);
        }

        /// <summary>
        /// 根据AreaSysNo查询包含该地区的省、市、区AreaInfo结构。
        /// 如果查询的Area不存在，则返回的结果包含完整的省列表，市和区列表为空；
        /// 如果查询的Area是省，则返回的结果包含完整的省列表和该省对应的下级城市列表，区列表为空；
        /// 如果查询的Area是市，则返回的结果包含完整的省列表和与该市同级的城市列表以及该城市对应的下级地区列表；
        /// 如果查询的Area是区，则返回完整的与该地区对应的省、市、区结构。
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void QueryCurrentAreaStructure_Old(int sysNo, EventHandler<RestClientEventArgs<AreaQueryResponse>> callback)
        {
            string relativeUrl = string.Format("/Area/QueryCurrentAreaStructure_Old/{0}", sysNo);
            restClient.Query<AreaQueryResponse>(relativeUrl, callback);
        }
    }
}