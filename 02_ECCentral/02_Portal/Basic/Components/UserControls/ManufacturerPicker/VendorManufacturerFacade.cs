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
using ECCentral.QueryFilter.IM;
using ECCentral.BizEntity.IM;

namespace ECCentral.Portal.Basic.Components.UserControls.ManufacturerPicker
{
    public class VendorManufacturerFacade
    {
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("IM", "ServiceBaseUrl");
            }
        }
        public VendorManufacturerFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryManufacturers(ManufacturerQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/IMService/Manufacturer/QueryManufacturer";
            restClient.QueryDynamicData(relativeUrl, request, callback);
        }

        public void QueryManufacturerBySysNo(string manufacturerSysNo, EventHandler<RestClientEventArgs<ManufacturerInfo>> callback)
        {
            string relativeUrl = "/IMService/Manufacturer/GetManufacturerInfoBySysNo";
            restClient.Query<ManufacturerInfo>(relativeUrl, manufacturerSysNo, callback);
        }

        public void QueryBrands(BrandQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/IMService/Brand/QueryBrand";
            restClient.QueryDynamicData(relativeUrl, request, callback);
        }

        public void QueryBrandBySysNo(string brandSysNo, EventHandler<RestClientEventArgs<BrandInfo>> callback)
        {
            string relativeUrl = "/IMService/Brand/GetBrandInfoBySysNo";
            restClient.Query<BrandInfo>(relativeUrl, brandSysNo, callback);
        }
    }
}
