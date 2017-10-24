using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.Customer.Facades.RequestMsg;
using ECCentral.BizEntity.Customer.Society;

namespace ECCentral.Portal.UI.Customer.Facades
{
    public class CommissionTypeQueryFacade
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

        public CommissionTypeQueryFacade(IPage page)
        {
            this.Page = page;
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("Customer", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
           
        }

        public void QueryCommissionType(CommissionTypeQueryVM filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/CommissionType/QueryCommissionType";
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

        public void CreateCommissionType(CommissionTypeVM _viewInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/CommissionType/Create";
            var msg = _viewInfo.ConvertVM<CommissionTypeVM, CommissionType>();
            restClient.Create(relativeUrl, msg, callback);
        }

        public void UpdateCommissionType(CommissionTypeVM _viewInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/CommissionType/Update";
            var msg = _viewInfo.ConvertVM<CommissionTypeVM, CommissionType>();
            restClient.Update(relativeUrl, msg, callback);
        }

        public void LoadCommissionType(int? sysNo, EventHandler<RestClientEventArgs<CommissionTypeVM>> callback)
        {
            string relativeUrl = "/CommonService/CommissionType/Load/" + sysNo;
            if (sysNo.HasValue)
            {
                restClient.Query<CommissionType>(relativeUrl, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    CommissionTypeVM _viewModel = null;
                    CommissionType entity = args.Result;
                    if (entity == null)
                    {
                        _viewModel = new CommissionTypeVM();
                    }
                    else
                    {
                        _viewModel = entity.Convert<CommissionType, CommissionTypeVM>();
                    }
                    callback(obj, new RestClientEventArgs<CommissionTypeVM>(_viewModel, restClient.Page));
                });
            }
        }
        public void SocietyCommissionQuery(CommissionTypeQueryVM filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/CommissionType/SocietyCommissionQuery";
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
    }
}
