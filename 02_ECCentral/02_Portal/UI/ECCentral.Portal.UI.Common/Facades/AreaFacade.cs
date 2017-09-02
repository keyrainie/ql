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
using ECCentral.Portal.UI.Common.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Common;

namespace ECCentral.Portal.UI.Common.Facades
{
    public class AreaFacade
    {  
        private readonly RestClient restClient;

         public IPage Page { get; set; }
        /// <summary>
        /// 服务基地址
        /// </summary>
        protected string ServiceBaseUrl
        {
            get;
            private set;
        }

        public AreaFacade(IPage page)
        {
            this.Page = page;
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("Common", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }
        public void QueryAreaList( AreaQueryFilterVM filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/Area/QueryAreaList";

            var msg = filter.ConvertVM<AreaQueryFilterVM, AreaQueryFilter>();

            restClient.QueryDynamicData(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(this, new RestClientEventArgs<dynamic>(args.Result, this.Page));
            });
        }

        public void Create(AreaInfoVM infoVM, EventHandler<RestClientEventArgs<AreaInfoVM>> callback)
        {
            string relativeUrl = "/CommonService/Area/Create";
            var msg = infoVM.ConvertVM<AreaInfoVM, AreaInfo>((s,t)=>
                {
                    if (!s.provinceSysNo.HasValue) t.ProvinceName = s.AreaName;
                    else if (!s.CitySysNo.HasValue && s.provinceSysNo.HasValue) t.CityName = s.AreaName;
                    //else if (!string.IsNullOrEmpty(s.DistrictName)) t.DistrictName = s.AreaName;
                    else if (s.CitySysNo.HasValue) t.DistrictName = s.AreaName;
                    
                });
            restClient.Create<AreaInfoVM>(relativeUrl, msg, (s, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    infoVM = args.Result;
                    callback(s, new RestClientEventArgs<AreaInfoVM>(infoVM, restClient.Page));

                });
        }
        public void Load(int? sysNo, EventHandler<RestClientEventArgs<AreaInfoVM>> callback)
        {
            string relativeUrl = "/CommonService/Area/Load/" + sysNo;
            if (sysNo.HasValue)
            {
                restClient.Query<AreaInfo>(relativeUrl, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }

                    AreaInfo entity = args.Result;
                    AreaInfoVM _viewModel = null;
                    if (entity == null)
                    {
                        _viewModel = new AreaInfoVM();
                    }
                    else
                    {
                        _viewModel = entity.Convert<AreaInfo, AreaInfoVM>();
                    }

                    callback(obj, new RestClientEventArgs<AreaInfoVM>(_viewModel, restClient.Page));
                });
            }
            else
            {
                callback(new Object(), new RestClientEventArgs<AreaInfoVM>(new AreaInfoVM(), restClient.Page));
            }
        }
        public void Update(AreaInfoVM  _viewMode,EventHandler<RestClientEventArgs<AreaInfoVM>> callback)
        {
            string relativeUrl = "/CommonService/Area/Update";
            var msg = _viewMode.ConvertVM<AreaInfoVM, AreaInfo>((s, t) =>
                {
                    if (string.IsNullOrEmpty(s.DistrictName) && string.IsNullOrEmpty(s.CityName)) { t.ProvinceName = s.AreaName; t.ProvinceSysNo = null; }
                    else if (string.IsNullOrEmpty(s.DistrictName) && !string.IsNullOrEmpty(s.CityName)) { t.CityName = s.AreaName; t.CitySysNo = null; }
                    else if (!string.IsNullOrEmpty(s.DistrictName)) t.DistrictName = s.AreaName;
                    
                });
            restClient.Update(relativeUrl, msg, callback);

        }
    }
}
