using System;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.IM;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.Basic.Components.UserControls.ProductPicker
{
    public class ProductQueryFacade
    {
        private readonly RestClient restClient;
        /// <summary>
        /// ProductService服务基地址
        /// </summary>
        private string ServiceBaseUrl
        {
            get
            {
                //return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Product", "ServiceBaseUrl");
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("IM", "ServiceBaseUrl");

            }
        }

        public ProductQueryFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public ProductQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        private RestClient GetRestClient(string domainName, IPage page)
        {
            string baseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(domainName, "ServiceBaseUrl");
            RestClient restClient = new RestClient(baseUrl, page);
            return restClient;
        }

        private RestClient GetRestClient(string domainName)
        {
            return GetRestClient(domainName, CPApplication.Current.CurrentPage);
        }

        public void QueryProduct(ProductSimpleQueryVM vm, PagingInfo p, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var data = vm.ConvertVM<ProductSimpleQueryVM, ProductQueryFilter>();
            data.PagingInfo = p;
            QueryProduct(data,callback);
        }

        public void QueryProduct(ProductQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.QueryDynamicData("/IMService/Product/QueryProduct", filter, callback);
        }

        public void LoadProductBySysNo(int ProductSysNo, EventHandler<RestClientEventArgs<ProductInfo>> callback)
        {
            string relativeUrl = "/IMService/Product/GetProductInfo";
            restClient.Query<ProductInfo>(relativeUrl, ProductSysNo, callback);
        }

        public void LoadProductByID(string ProductID, EventHandler<RestClientEventArgs<ProductInfo>> callback)
        {
            string relativeUrl = "/IMService/Product/GetProductInfoByID";
            restClient.Query<ProductInfo>(relativeUrl, ProductID, callback);
        }

        public void GetValidVenderGifts(object productSysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            GetRestClient("MKT").QueryDynamicData(string.Format("/MKTService/SaleGift/GetValidVenderGifts/{0}", productSysNo), callback);
        }

        /// <summary>
        /// 延保查询
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <param name="callback">回调函数</param>
        public void GetExtendWarranty(CategoryExtendWarrantyQueryFilter query, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            restClient.QueryDynamicData("/IMService/CategoryExtendWarranty/QueryCategoryExtendWarranty",query, callback);
        }
    }
}
