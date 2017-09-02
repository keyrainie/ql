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
using ECCentral.QueryFilter.Customer;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.Customer.Facades
{
    public class PrePayFacade
    {
         private readonly RestClient restClient;
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Customer", "ServiceBaseUrl");
            }
        }
        public PrePayFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }
        public PrePayFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);

        }
        public void QueryPrePayLogIncome(PrePayQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/PrePay/QueryPrePayLogIncome";
            restClient.QueryDynamicData(relativeUrl, request, callback);
        }
        public void QueryPrePayLogPayment(PrePayQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/PrePay/QueryPrePayLogPayment";
            restClient.QueryDynamicData(relativeUrl, request, callback);
        }
    }
}
