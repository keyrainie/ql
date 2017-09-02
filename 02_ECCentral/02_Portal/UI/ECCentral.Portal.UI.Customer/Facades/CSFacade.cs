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
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.QueryFilter;
using ECCentral.QueryFilter.Customer;
using ECCentral.Portal.UI.Customer.Models;
using ECCentral.BizEntity.Customer;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.BizEntity.Common;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.Customer.Facades
{
    public class CSFacade
    {
        private readonly RestClient restClient;
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Customer", "ServiceBaseUrl");
            }
        }
        public CSFacade()
        {
            restClient = new RestClient(ServiceBaseUrl);
        }
        public CSFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);

        }
        public void Query(CSQueryFilter request, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/CustomerService/CS/Query";
            request.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.QueryDynamicData(relativeUrl, request, callback);
        }

        public void Create(CSVM vm, EventHandler<RestClientEventArgs<CSInfo>> callback)
        {
            string relativeUrl = "/CustomerService/CS/Create";
            CSInfo msg = vm.ConvertVM<CSVM, CSInfo>();
            msg.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.Create<CSInfo>(relativeUrl, msg, callback);
        }
        public void Update(CSVM vm, EventHandler<RestClientEventArgs<CSInfo>> callback)
        {
            string relativeUrl = "/CustomerService/CS/Update";
            CSInfo msg = vm.ConvertVM<CSVM, CSInfo>();
            msg.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.Update<CSInfo>(relativeUrl, msg, callback);
        }
        public void GetCSWithDepartmentId(int depId, EventHandler<RestClientEventArgs<List<CSInfo>>> callback)
        {
            string relativeUrl = "/CustomerService/CS/GetCSWithDepartmentId";
            restClient.Query<List<CSInfo>>(relativeUrl, depId, callback);
        }
        public void GetAllCS(EventHandler<RestClientEventArgs<List<CSInfo>>> callback)
        {
            string relativeUrl = "/CustomerService/CS/GetAllCS";
            restClient.Query<List<CSInfo>>(relativeUrl, CPApplication.Current.CompanyCode, callback);
        }
        public void GetCSByLeaderSysNo(int leadersysno, EventHandler<RestClientEventArgs<List<CSInfo>>> callback)
        {
            string relativeUrl = "/CustomerService/CS/GetCSByLeaderSysNo";
            restClient.Query<List<CSInfo>>(relativeUrl, leadersysno, callback);
        }

    }
}
