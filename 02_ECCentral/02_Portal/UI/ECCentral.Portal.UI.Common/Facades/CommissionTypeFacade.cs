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
    public class CommissionTypeFacade
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

        public CommissionTypeFacade(IPage page)
        {
            this.Page = page;
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("Common", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryPayTypeList(PayTypeQueryVM filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/PayType/QueryPayType";

            var msg = filter.ConvertVM<PayTypeQueryVM, PayTypeQueryFilter>();

            restClient.QueryDynamicData(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(this, new RestClientEventArgs<dynamic>(args.Result, this.Page));
            });
        }

        public void CreatePayType(CommissionTypeQueryVM _viewInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/PayType/Create";
            var msg = _viewInfo.ConvertVM<CommissionTypeQueryVM, PayType>();
            restClient.Create(relativeUrl, msg, callback);
        }

        public void UpdatePayType(PayTypeVM _viewInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/PayType/Update";
            var msg = _viewInfo.ConvertVM<PayTypeVM, PayType>();
            restClient.Update(relativeUrl, msg, callback);
        }

        public void LoadPayType(int? sysNo, EventHandler<RestClientEventArgs<PayTypeVM>> callback)
        {
            string relativeUrl = "/CommonService/PayType/Load/" + sysNo;
            if (sysNo.HasValue)
            {
                restClient.Query<PayType>(relativeUrl, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    PayTypeVM _viewModel = null;
                    PayType entity = args.Result;
                    if (entity == null)
                    {
                        _viewModel = new PayTypeVM();
                    }
                    else
                    {
                        _viewModel = entity.Convert<PayType, PayTypeVM>();
                    }
                    callback(obj, new RestClientEventArgs<PayTypeVM>(_viewModel, restClient.Page));
                });
            }
        }
    }
}
