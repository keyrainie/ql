using ECCentral.BizEntity.Common;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
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

namespace ECCentral.Portal.UI.Customer.Facades.RequestMsg
{
    public class CommissionTypeQueryMVFilter : ModelBase
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

        public CommissionTypeQueryMVFilter(IPage page)
        {
            this.Page = page;
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("Common", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryPayTypeList(CommissionTypeQueryVM filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/PayType/QueryPayType";

            var msg = filter.ConvertVM<CommissionTypeQueryVM, CommissionTypeQueryFilter>();

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

        public void UpdatePayType(CommissionTypeQueryVM _viewInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/PayType/Update";
            var msg = _viewInfo.ConvertVM<CommissionTypeQueryVM, PayType>();
            restClient.Update(relativeUrl, msg, callback);
        }

        public void LoadPayType(int? sysNo, EventHandler<RestClientEventArgs<CommissionTypeQueryVM>> callback)
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
                    CommissionTypeQueryVM _viewModel = null;
                    PayType entity = args.Result;
                    if (entity == null)
                    {
                        _viewModel = new CommissionTypeQueryVM();
                    }
                    else
                    {
                        _viewModel = entity.Convert<PayType, CommissionTypeQueryVM>();
                    }
                    callback(obj, new RestClientEventArgs<CommissionTypeQueryVM>(_viewModel, restClient.Page));
                });
            }
        }
    }
}
