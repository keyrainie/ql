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
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.QueryFilter.PO;

namespace ECCentral.Portal.UI.PO.Facades
{
    public class ConsignToAccountLogFacade
    {
        private readonly RestClient restClient;

        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("PO", "ServiceBaseUrl");
            }
        }
        public ConsignToAccountLogFacade(IPage page)
        {
            restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void QueryConsignToAccountLog(ConsignToAccountLogQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/POService/Vendor/QueryConsignToAccountLog";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }

        public void ExportDataDForConsignToAccountLog(ConsignToAccountLogQueryFilter queryFilter, ColumnSet[] columns)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/POService/Vendor/QueryConsignToAccountLog";
            restClient.ExportFile(relativeUrl, queryFilter, columns);
        }

        public void QueryConsignToAccountLogTotalAmt(ConsignToAccountLogQueryFilter queryFilter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/POService/Vendor/QueryConsignToAccountLogTotalAmt";
            restClient.QueryDynamicData(relativeUrl, queryFilter, callback);
        }
    }
}
