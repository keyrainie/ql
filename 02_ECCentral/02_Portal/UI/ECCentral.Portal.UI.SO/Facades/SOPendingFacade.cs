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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.SO.Models;
using System.Collections.Generic;
using ECCentral.BizEntity.SO;
using ECCentral.QueryFilter.SO;

namespace ECCentral.Portal.UI.SO.Facades
{
    public class SOPendingFacade
    {
        private readonly RestClient restClient;
        private string serviceBaseUrl;

        public SOPendingFacade(IPage page)
        {
            serviceBaseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_SO, ConstValue.Key_ServiceBaseUrl);
            restClient = new RestClient(serviceBaseUrl, page);
        }

        public SOPendingFacade()
            : this(null)
        {
        }

        public void OpenSOPending(int soSysNo, EventHandler<RestClientEventArgs<int>> callback)
        {
            string relativeUrl = "/SOService/SO/OpenPending";
            restClient.Update<int>(relativeUrl, soSysNo, callback);
        }

        public void CloseSOPending(int soSysNo, EventHandler<RestClientEventArgs<int>> callback)
        {
            string relativeUrl = "/SOService/SO/ClosePending";
            restClient.Update<int>(relativeUrl, soSysNo, callback);
        }

        public void UpdateSOPending(int soSysNo, EventHandler<RestClientEventArgs<int>> callback)
        {
            string relativeUrl = "/SOService/SO/UpdatePending";
            restClient.Update<int>(relativeUrl, soSysNo, callback);
        }
    }
}
