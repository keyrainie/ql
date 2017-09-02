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
    public class ControlPanelUserFacade
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

        public ControlPanelUserFacade(IPage page)
        {
            this.Page = page;
            ServiceBaseUrl = page.Context.Window.Configuration.GetConfigValue("Common", "ServiceBaseUrl");
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryUserList(ControlPanelUserQueryFilterVM filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/ControlPanelUser/QueryUser";

            var msg = filter.ConvertVM<ControlPanelUserQueryFilterVM, ControlPanelUserQueryFilter>();

            restClient.QueryDynamicData(relativeUrl, msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(this, new RestClientEventArgs<dynamic>(args.Result, this.Page));
            });
        }

        public void CreateUser(ControlPanelUserVM _viewInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/ControlPanelUser/CreateUser";
            var msg = _viewInfo.ConvertVM<ControlPanelUserVM, ControlPanelUser>();
            restClient.Create(relativeUrl, msg, callback);
        }

        public void UpdateUser(ControlPanelUserVM _viewInfo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CommonService/ControlPanelUser/UpdateUser";
            var msg = _viewInfo.ConvertVM<ControlPanelUserVM, ControlPanelUser>();
            restClient.Update(relativeUrl, msg, callback);
        }

        public void GetControlPanelUserByLoginName(string loginName, EventHandler<RestClientEventArgs<ControlPanelUserVM>> callback)
        {
            string relativeUrl = "/CommonService/ControlPanelUser/GetControlPanelUserByLoginName";
            if (!string.IsNullOrEmpty(loginName))
            {
                restClient.Query<ControlPanelUser>(relativeUrl,loginName, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    ControlPanelUserVM _viewModel = null;
                    ControlPanelUser entity = args.Result;
                    if (entity == null)
                    {
                        _viewModel = new ControlPanelUserVM();
                    }
                    else
                    {
                        _viewModel = entity.Convert<ControlPanelUser, ControlPanelUserVM>();
                    }
                    callback(obj, new RestClientEventArgs<ControlPanelUserVM>(_viewModel, restClient.Page));
                });
            }
        }

        public void GetUserBySysNo(int? sysNo, EventHandler<RestClientEventArgs<ControlPanelUserVM>> callback)
        {
            string relativeUrl = "/CommonService/ControlPanelUser/GetUser/" + sysNo;
            if (sysNo.HasValue)
            {
                restClient.Query<ControlPanelUser>(relativeUrl, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    ControlPanelUserVM _viewModel = null;
                    ControlPanelUser entity = args.Result;
                    if (entity == null)
                    {
                        _viewModel = new ControlPanelUserVM();
                    }
                    else
                    {
                        _viewModel = entity.Convert<ControlPanelUser, ControlPanelUserVM>();
                    }
                    callback(obj, new RestClientEventArgs<ControlPanelUserVM>(_viewModel, restClient.Page));
                });
            }
        }
    }
}
