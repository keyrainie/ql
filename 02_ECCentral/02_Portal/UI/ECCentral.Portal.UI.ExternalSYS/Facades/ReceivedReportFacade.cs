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
using ECCentral.QueryFilter.ExternalSYS;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.ExternalSYS.Facades
{
    public class ReceivedReportFacade
    {
        private readonly RestClient restClient;
        private string serviceBaseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.Key_ServiceBaseUrl);

        public ReceivedReportFacade()
        {
            restClient = new RestClient(serviceBaseUrl);
        }

        public ReceivedReportFacade(IPage page)
        {
            restClient = new RestClient(serviceBaseUrl, page);
        }

        #region 收款报表

        /// <summary>
        /// 年度收款报表查询
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="callback">回调函数</param>
        public void ReceiveByYearQuery(ReceivedReportQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "ExternalSYSService/ExternalSYS/QueryReceiveByYear";
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        /// <summary>
        /// 供应商对账单查询
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="callback">回调函数</param>
        public void ReceiveByVendorQuery(ReceivedReportQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "ExternalSYSService/ExternalSYS/QueryReceiveByVendor";
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        /// <summary>
        /// 应收账单(单据)查询
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="callback">回调函数</param>
        public void ARReceiveQuery(ReceivedReportQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "ExternalSYSService/ExternalSYS/QueryARReceive";
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        /// <summary>
        /// 应收账单明细
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="callback">回调函数</param>
        public void ARReceiveDetialsQuery(ReceivedReportQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "ExternalSYSService/ExternalSYS/QueryARReceiveDetials";
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }
        #endregion

        #region 导出报表数据
        public void ExportReceiveByYear(ReceivedReportQueryFilter filter, ColumnSet[] columns)
        {
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.ExportFile("/ExternalSYSService/ExternalSYS/QueryReceiveByYear", filter, columns);
        }

        public void ExportReceiveByVendor(ReceivedReportQueryFilter filter, ColumnSet[] columns)
        {
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.ExportFile("/ExternalSYSService/ExternalSYS/QueryReceiveByVendor", filter, columns);
        }

        public void ExportARReceive(ReceivedReportQueryFilter filter, ColumnSet[] columns)
        {
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.ExportFile("/ExternalSYSService/ExternalSYS/QueryARReceive", filter, columns);
        }

        public void ExportARReceiveDetials(ReceivedReportQueryFilter filter, ColumnSet[] columns)
        {
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.ExportFile("/ExternalSYSService/ExternalSYS/QueryARReceiveDetials", filter, columns);
        }
        #endregion
    }
}
