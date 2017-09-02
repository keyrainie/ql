using System;

using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic.Utilities;

using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.RMA.Facades
{
    public class ExternalFacade
    {
        private RestClient restClient;

        private string SOServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("SO", "ServiceBaseUrl");
            }
        }

        private string CustomerServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Customer", "ServiceBaseUrl");
            }
        }       

        public void GetSOInfo(int soSysNo, EventHandler<RestClientEventArgs<SOInfo>> callback)
        {
            restClient = new RestClient(SOServiceBaseUrl);
            string relativeUrl = string.Format("/SOService/SOInfo/Query/{0}", soSysNo);
            restClient.Query<SOInfo>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void GetCustomerInfo(int customerSysNo, EventHandler<RestClientEventArgs<CustomerInfo>> callback)
        {
            restClient = new RestClient(CustomerServiceBaseUrl);
            string relativeUrl = string.Format("/CustomerService/Customer/Load/{0}", customerSysNo);
            restClient.Query<CustomerInfo>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }
    }
}