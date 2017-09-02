using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Customer.Models.CSTools;
using ECCentral.Portal.UI.Customer.Views;
using ECCentral.QueryFilter;
using ECCentral.Service.Customer.Restful.RequestMsg;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.Customer.Facades
{
    public class OrderCheckItemFacade
    {
        private readonly RestClient restClient;
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Customer", "ServiceBaseUrl");
            }
        }

        public OrderCheckItemFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }

        public OrderCheckItemFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryOrderCheckItem(OrderCheckItemQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/OrderCheckItem/Query";
            restClient.QueryDynamicData(relativeUrl, request, callback);
        }

        public void CreateOrderCheckItem(OrderCheckItemVM data, EventHandler<RestClientEventArgs<OrderCheckItemVM>> callback)
        {
            string relativeUrl = "/CustomerService/OrderCheckItem/Create";
            OrderCheckItem msg = data.ConvertVM<OrderCheckItemVM, OrderCheckItem>();
            restClient.Create<OrderCheckItemVM>(relativeUrl, msg, callback);
        }

        public void UpdateOrderCheckItem(OrderCheckItemVM data, EventHandler<RestClientEventArgs<OrderCheckItemVM>> callback)
        {
            string relativeUrl = "/CustomerService/OrderCheckItem/Update";
            OrderCheckItem msg = data.ConvertVM<OrderCheckItemVM, OrderCheckItem>();
            restClient.Update(relativeUrl, msg, callback);
        }

        public void BatchCreateOrderCheckItem(BatchCreatOrderCheckItemReq req, EventHandler<RestClientEventArgs<OrderCheckItemVM>> callback)
        {
            string relativeUrl = "/CustomerService/OrderCheckItem/BatchCreate";
            restClient.Create<OrderCheckItemVM>(relativeUrl, req, callback);
        }

        public List<OrderCheckItem> ConvertToBatchOperation(List<OrderCheckItemVM> vmList)
        {
            List<OrderCheckItem> dataList = new List<OrderCheckItem>(vmList.Count);
            foreach (var vm in vmList)
            {
                var msg = vm.ConvertVM<OrderCheckItemVM, OrderCheckItem>();
                dataList.Add(msg);
            }

            return dataList;
        }

        public void GetPayTypeList(EventHandler<RestClientEventArgs<List<PayType>>> callback)
        {
            string relativeUrl = string.Format("CommonService/PayType/GetAll/{0}", CPApplication.Current.CompanyCode);
            restClient.Query<List<PayType>>(relativeUrl, callback);
        }
    }
}