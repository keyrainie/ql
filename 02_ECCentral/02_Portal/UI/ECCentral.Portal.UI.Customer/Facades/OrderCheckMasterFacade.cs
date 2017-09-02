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
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.Customer.Facades;
using ECCentral.Portal.UI.Customer.UserControls;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic.Utilities;
using System.Dynamic;
using System.ComponentModel;
using ECCentral.Portal.UI.Customer.Views;
using ECCentral.Portal.UI.Customer.Models.CSTools;
using ECCentral.QueryFilter;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Service.Customer.Restful.RequestMsg;


namespace ECCentral.Portal.UI.Customer.Facades
{
    public class OrderCheckMasterFacade
    {
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Customer", "ServiceBaseUrl");
            }
        }

        public OrderCheckMasterFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public OrderCheckMasterFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);

        }
        public void QueryOrderCheckMaster(OrderCheckMasterQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/OrderCheckMaster/Query";
            restClient.QueryDynamicData(relativeUrl, request, callback);
        }

        public void BatchUpdateOrderCheckMasterStatus(OrderCheckMasterReq request, EventHandler<RestClientEventArgs<object>> callback)
        {
            string relativeUrl = "/CustomerService/OrderCheckMaster/Update";
            restClient.Update(relativeUrl, request, callback);
        }
    }
}
