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
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls;
using System.Collections.Generic;
using ECCentral.QueryFilter.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.MKT.Facades
{
    public class ExternalServiceFacade
    {
        private IPage _currentPage;
        private readonly RestClient imRestClient;
        public ExternalServiceFacade(IPage currentPage)
        {
            _currentPage = currentPage;
            var baseUrl = _currentPage.Context.Window.Configuration.GetConfigValue("IM", "ServiceBaseUrl");
            imRestClient = new RestClient(baseUrl, _currentPage);
        }

        public void GetAllActiveBackendC3(EventHandler<RestClientEventArgs<List<CategoryInfo>>> callback)
        {
            string relativeUrl = string.Format("/IMService/Category/QueryAllCategory{0}", 3);
            var queryFilter = new CategoryQueryFilter();
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            queryFilter.Status = CategoryStatus.Active;
            imRestClient.Query<List<CategoryInfo>>(relativeUrl, queryFilter, callback);
        }

        public void GetProductInfo(int productSysNo, EventHandler<RestClientEventArgs<ProductInfo>> callback)
        {
            imRestClient.Query("/IMService/Product/GetProductInfo", productSysNo, callback);
        }

        public void QueryVendorStoreList(int vendorSysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            imRestClient.QueryDynamicData("/POService/Vendor/QueryVendorStoreList", vendorSysNo, callback);
        }
    }
}
