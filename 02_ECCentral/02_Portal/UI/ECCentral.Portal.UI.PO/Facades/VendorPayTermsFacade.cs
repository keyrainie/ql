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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.PO;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.PO.Facades
{
    public class VendorPayTermsFacade
    {
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("PO", "ServiceBaseUrl");
            }
        }

        public VendorPayTermsFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryVendorPayTermsList(string companyCode, EventHandler<RestClientEventArgs<List<VendorPayTermsItemInfo>>> callback)
        {
            string relativeUrl = "/POService/Vendor/GetVendorPayTermsList";
            restClient.Query<List<VendorPayTermsItemInfo>>(relativeUrl, companyCode, callback);
        }

    }
}
