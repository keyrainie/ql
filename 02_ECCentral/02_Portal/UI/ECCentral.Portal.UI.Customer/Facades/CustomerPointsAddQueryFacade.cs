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
using ECCentral.Portal.UI.Customer.Views;
using ECCentral.Portal.UI.Customer.Models;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.Customer;
using System.Collections.Generic;
using ECCentral.Portal.UI.Customer.UserControls;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Service.Customer.Restful.RequestMsg;
using ECCentral.QueryFilter.Customer;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.SO;

namespace ECCentral.Portal.UI.Customer.Facades
{
    public class CustomerPointsAddQueryFacade
    {

        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Customer", "ServiceBaseUrl");
            }
        }

        public CustomerPointsAddQueryFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public CustomerPointsAddQueryFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl,page);
        }
        #region [Query Methods]

        public void QueryCustomerPointsAddList(CustomerPointsAddRequestFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/Points/QueryPointsAddRequest";
            restClient.QueryDynamicData(relativeUrl, request, callback);
        }

        public void ExportCustomerPointsAddList(CustomerPointsAddRequestFilter request, ColumnSet[] columns)
        {
            string relativeUrl = "/CustomerService/Points/QueryPointsAddRequest";
            restClient.ExportFile(relativeUrl, request, columns);
        }


        public void QueryCustomerPointsAddItem(CustomerPointsAddRequestFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string itemRelativeUrl = "/CustomerService/Points/QueryPointsAddRequestItem";
            restClient.QueryDynamicData(itemRelativeUrl, request, callback);
        }


        public void QuerySO(int soSysNo,string sysAccount, EventHandler<RestClientEventArgs<SOInfo>> callback)
        {
            //string itemRelativeUrl = "/CustomerService/Points/GetSoDetail/{0}/{1}";
            //restClient.Query<SOInfo>(string.Format(itemRelativeUrl, soSysNo, sysAccount), callback);
            string itemRelativeUrl = "/CustomerService/Points/GetSoDetail/{0}";
            restClient.Query<SOInfo>(string.Format(itemRelativeUrl, soSysNo), callback);
        }

        #endregion

        #region [Action Methods]

        public void ConfirmCustomerPointsAddRequest(CustomerPointsAddRequest request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string itemRelativeUrl = "/CustomerService/Point/ConfirmCustomerPointsAddRequest";
            restClient.Update(itemRelativeUrl, request, callback);
        }

        public void CreateCustomerPointsAddRequest(CustomerPointsAddRequest request, EventHandler<RestClientEventArgs<CustomerPointsAddRequest>> callback)
        {
            string itemRelativeUrl = "/CustomerService/Point/CreateCustomerPointsAddRequest";
            restClient.Update(itemRelativeUrl, request, callback);
        }

        #endregion
    }
}
