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

using ECCentral.BizEntity.RMA;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.RMA.Models;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.Basic;


namespace ECCentral.Portal.UI.RMA.Facades
{
    public class CustomerContactFacade
    {
        private readonly IPage viewPage;
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_RMA, "ServiceBaseUrl");
            }
        }

        public CustomerContactFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public CustomerContactFacade(IPage page)
        {
            viewPage = page;
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void LoadByRequestSysNo(int sysNo, EventHandler<RestClientEventArgs<CustomerContactInfo>> callback)
        {
            string relativeUrl = string.Format("/RMAService/CustomerContact/Load/{0}", sysNo);
            restClient.Query<CustomerContactInfo>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void LoadOriginByRequestSysNo(int sysNo, EventHandler<RestClientEventArgs<CustomerContactInfo>> callback)
        {
            string relativeUrl = string.Format("/RMAService/CustomerContact/LoadOrigin/{0}", sysNo);
            restClient.Query<CustomerContactInfo>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void Create(CustomerContactVM vm, EventHandler<RestClientEventArgs<CustomerContactInfo>> callback)
        {
            string relativeUrl = "/RMAService/CustomerContact/Create";
            var msg = vm.ConvertVM<CustomerContactVM, CustomerContactInfo>();
            restClient.Create<CustomerContactInfo>(relativeUrl,msg, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }

                callback(obj, args);
            });
        }

        public void Update(CustomerContactVM vm, EventHandler<RestClientEventArgs<CustomerContactInfo>> callback)
        {
            string relativeUrl = "/RMAService/CustomerContact/Update";
            var msg = vm.ConvertVM<CustomerContactVM, CustomerContactInfo>();
            restClient.Update<CustomerContactInfo>(relativeUrl, msg, (obj, args) =>
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
