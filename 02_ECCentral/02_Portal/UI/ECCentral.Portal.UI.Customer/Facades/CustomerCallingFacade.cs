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
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.RMA;

namespace ECCentral.Portal.UI.Customer.Facades
{
    public class CustomerCallingFacade
    {
        private readonly RestClient restClient;
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Customer", "ServiceBaseUrl");
            }
        }
        public CustomerCallingFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }
        public CustomerCallingFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);

        }
        public void QuerySOList(CustomerCallingQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/CallingLog/QuerySOList";
            restClient.QueryDynamicData(relativeUrl, request, callback);
        }

        public void ExportSOList(CustomerCallingQueryFilter request, ColumnSet[] columns)
        {
            string relativeUrl = "/CustomerService/CallingLog/QuerySOList";
            restClient.ExportFile(relativeUrl, request, columns);
        }

        public void QueryRMAList(CustomerCallingQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/CallingLog/QueryRMAList";
            restClient.QueryDynamicData(relativeUrl, request, callback);
        }

        public void GetUpdateUser(EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/CallingLog/GetUpdateUser";
            restClient.QueryDynamicData(relativeUrl, CPApplication.Current.CompanyCode, callback);
        }


        public void ExportRMAList(CustomerCallingQueryFilter request, ColumnSet[] columns)
        {
            string relativeUrl = "/CustomerService/CallingLog/QueryRMAList";
            restClient.ExportFile(relativeUrl, request, columns);
        }


        public void QueryComplainList(CustomerCallingQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/CallingLog/QueryComplainList";
            restClient.QueryDynamicData(relativeUrl, request, callback);
        }

        public void ExportComplainList(CustomerCallingQueryFilter request, ColumnSet[] columns)
        {
            string relativeUrl = "/CustomerService/CallingLog/QueryComplainList";
            restClient.ExportFile(relativeUrl, request, columns);
        }

        public void QueryCalling(CustomerCallingQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/CallingLog/QueryCalling";
            restClient.QueryDynamicData(relativeUrl, request, callback);
        }

        public void ExportCalling(CustomerCallingQueryFilter request, ColumnSet[] columns)
        {
            string relativeUrl = "/CustomerService/CallingLog/QueryCalling";
            restClient.ExportFile(relativeUrl, request, columns);
        }     
        public void GetRMARegisterList(RMARegisterQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/CallingLog/GetRMARegisterList";
            restClient.QueryDynamicData(relativeUrl, request, callback);
        }

        public void CreateCallsEvents(CallsEvents request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/CallsEvents/Create";
            restClient.Create(relativeUrl, request, callback);
        }
        public void CreateCallsEventsFollowUpLog(CallsEventsFollowUpLog request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/CallsEvents/CreateCallsEventsFollowUpLog";
            restClient.Create(relativeUrl, request, callback);
        }
        public void CallingToComplain(SOComplaintCotentInfo request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/CallsEvents/CallingToComplain";
            restClient.Create(relativeUrl, request, callback);
        }

        public void CallingToRMA(InternalMemoInfo request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/CallsEvents/CallingToRMA";
            restClient.Create(relativeUrl, request, callback);
        }

        public void LoadCallsEvent(int sysno, EventHandler<RestClientEventArgs<CallsEvents>> callback)
        {
            string relativeUrl = string.Format("/CustomerService/CallsEvents/Load/{0}", sysno);
            restClient.Query<CallsEvents>(relativeUrl, callback);
        }

        public void QueryCallsEventLog(CustomerCallsEventLogFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = string.Format("/CustomerService/CallsEvents/QueryCallsEventLog");
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }
    }
}
