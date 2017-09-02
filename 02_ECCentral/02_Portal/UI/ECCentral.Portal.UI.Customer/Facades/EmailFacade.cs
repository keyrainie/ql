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
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.QueryFilter.Customer;

namespace ECCentral.Portal.UI.Customer.Facades
{
    public class EmailFacade
    {
        private readonly RestClient restClient;
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Customer", "ServiceBaseUrl");
            }
        }
        public EmailFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }
        public EmailFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }
        public void Query(EmailQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/Email/Query";
            restClient.QueryDynamicData(relativeUrl, request, callback);
        }

        public void GetMailContent(int SysNo, string dbName, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = string.Format("/CustomerService/Email/{0}Load/{1}", dbName, SysNo);
            restClient.Query(relativeUrl, callback);
        }
    }
}
