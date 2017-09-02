using System;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.Basic.Components.UserControls.CustomerPicker
{
    public class CustomerFacade
    {
        private readonly RestClient restClient;
        /// <summary>
        /// CustomerService服务基地址
        /// </summary>
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Customer", "ServiceBaseUrl");
            }
        }

        public CustomerFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public CustomerFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryCustomer(CustomerSimpleQueryVM vm, PagingInfo p, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var data = vm.ConvertVM<CustomerSimpleQueryVM, CustomerSimpleQueryFilter>();
            data.PagingInfo = p;
            data.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/CustomerService/Customer/Picker";
            restClient.QueryDynamicData(relativeUrl, data, callback);
        }

        public void LoadCustomerBySysNo(int customerSysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            CustomerSimpleQueryFilter filter = new CustomerSimpleQueryFilter();
            filter.CustomerSysNo = customerSysNo;
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            filter.PagingInfo = new PagingInfo { PageIndex = 0, PageSize = int.MaxValue };
            string relativeUrl = "/CustomerService/Customer/Picker";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        public void LoadCustomerByID(string customerID, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            CustomerSimpleQueryFilter filter = new CustomerSimpleQueryFilter();
            filter.CustomerID = customerID;
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            filter.PagingInfo = new PagingInfo { PageIndex = 0, PageSize = int.MaxValue };
            string relativeUrl = "/CustomerService/Customer/Picker";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }
    }
}
