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
using System.Collections.Generic;
using ECCentral.Service.Common.Restful.ResponseMsg;

namespace ECCentral.Portal.UI.Common.Facades
{
    public class AreaDeliveryFacade
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

        public AreaDeliveryFacade(IPage page)
        {
            this.Page = page;
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("Common", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryAreaDeliveryList(AreaDeliveryQueryFilterVM filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/AreaDelivery/QueryAreaDelivery";

            var msg = filter.ConvertVM<AreaDeliveryQueryFilterVM, AreaDeliveryQueryFilter>();

            restClient.QueryDynamicData(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(this, new RestClientEventArgs<dynamic>(args.Result, this.Page));
            });
        }

        public void QueryWHAreaList(EventHandler<RestClientEventArgs<List<AreaDelidayResponse>>> callback)
        {
            string relativeUrl = "/CommonService/AreaDelivery/GetWHArea";
            restClient.Query<List<AreaDelidayResponse>>(relativeUrl, callback);
        }

        public void CreateAreaDelivery(AreaDeliveryInfoVM _viewInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/AreaDelivery/Create";
            var msg = _viewInfo.ConvertVM<AreaDeliveryInfoVM, AreaDeliveryInfo>();
            restClient.Create(relativeUrl, msg, callback);
        }

        public void UpdateAreaDelivery(AreaDeliveryInfoVM _viewInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/AreaDelivery/Update";
            var msg = _viewInfo.ConvertVM<AreaDeliveryInfoVM, AreaDeliveryInfo>();
            restClient.Update(relativeUrl, msg, callback);
        }

        public void DeleteAreaDelivery(string sysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/AreaDelivery/Delete";
            restClient.Delete(relativeUrl, sysNo, callback);
        }

        /// <summary>
        /// 根据系统id加载记录
        /// </summary>
        public void GetAreaDeliveryInfoByID(int? sysNo, EventHandler<RestClientEventArgs<AreaDeliveryInfoVM>> callback)
        {
            string relativeUrl = "/CommonService/AreaDelivery/Load/" + sysNo;
            if (sysNo.HasValue)
            {
                restClient.Query<AreaDeliveryInfo>(relativeUrl, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    AreaDeliveryInfoVM _viewModel = null;
                    AreaDeliveryInfo entity = args.Result;
                    if (entity == null)
                    {
                        _viewModel = new AreaDeliveryInfoVM();
                    }
                    else
                    {
                        _viewModel = entity.Convert<AreaDeliveryInfo, AreaDeliveryInfoVM>();
                    }
                    callback(obj, new RestClientEventArgs<AreaDeliveryInfoVM>(_viewModel, restClient.Page));
                });
            }
        }
    }
}
