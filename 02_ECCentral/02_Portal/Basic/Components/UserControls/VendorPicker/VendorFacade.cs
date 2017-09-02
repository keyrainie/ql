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
using ECCentral.QueryFilter.PO;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.PO;

namespace ECCentral.Portal.Basic.Components.UserControls.VendorPicker
{
    public class VendorFacade
    {
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("PO", "ServiceBaseUrl");
            }
        }

        public VendorFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询供应商列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="callback"></param>
        public void QueryVendors(VendorQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/POService/Vendor/QueryVendorList";
            restClient.QueryDynamicData(relativeUrl, request, callback);
        }

        /// <summary>
        /// 加载单个供应商信息
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <param name="callback"></param>
        public void GetVendorBySysNo(string vendorSysNo, EventHandler<RestClientEventArgs<VendorInfo>> callback)
        {
            string relativeUrl = string.Format("/POService/Vendor/GetVendorInfo/{0}", vendorSysNo);
            restClient.Query<VendorInfo>(relativeUrl, callback);
        }
    }
}
