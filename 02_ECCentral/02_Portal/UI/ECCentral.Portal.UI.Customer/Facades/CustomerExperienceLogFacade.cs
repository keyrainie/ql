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
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.Portal.UI.Customer.UserControls;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Customer;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.Customer.Facades
{
    public class CustomerExperienceLogFacade
    {
        private readonly RestClient restClient;

        /// <summary>
        /// CustomerService服务基地址
        /// </summary>
        private string ServiceBaseUrl
        {
            get
            {
                return Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Customer", "ServiceBaseUrl");
            }
        }

        public CustomerExperienceLogFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public CustomerExperienceLogFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void Query(CustomerExperienceLogQueryVM model, int PageSize, int PageIndex, string SortField, EventHandler<ECCentral.Portal.Basic.Utilities.RestClientEventArgs<dynamic>> callback)
        {
            CustomerExperienceLogQueryFilter filter = model.ConvertVM<CustomerExperienceLogQueryVM, CustomerExperienceLogQueryFilter>();
            filter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            string relativeUrl = "/CustomerService/Experience/Query";
            restClient.QueryDynamicData(relativeUrl, filter, callback);

        }
    }
}
