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
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.QueryFilter.Customer;
using ECCentral.Service.Customer.Restful.RequestMsg;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.Customer.Facades
{
    public class SMSFacade
    {
        private readonly RestClient restClient;
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Customer", "ServiceBaseUrl");
            }
        }
        public SMSFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }
        public SMSFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);

        }
        public void Query(SMSQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/SMS/Query";
            restClient.QueryDynamicData(relativeUrl, request, callback);
        }
        public void SendSMSByCellphone(SendSMSReq request, EventHandler<RestClientEventArgs<List<string>>> callback)
        {
            string relativeUrl = "/CustomerService/SMS/SendByCellphone";
            restClient.Create(relativeUrl, request, callback);
        }
        public void SendSMSBySOSysNo(SendSMSReq request, EventHandler<RestClientEventArgs<List<string>>> callback)
        {
            string relativeUrl = "/CustomerService/SMS/SendBySOSysNo";
            restClient.Create(relativeUrl, request, callback);
        }
        public void SendEmail(SendEmailReq request, EventHandler<RestClientEventArgs<List<string>>> callback)
        {
            string relativeUrl = "/CustomerService/SMS/SendEmail";
            restClient.Create(relativeUrl, request, callback);
        }


    }
}
