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
    public class AccruedReportFacade
    {
        private readonly RestClient restClient;
        private string serviceBaseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.Key_ServiceBaseUrl);

        public AccruedReportFacade()
        {
            restClient = new RestClient(serviceBaseUrl);
        }

        public AccruedReportFacade(IPage page)
        {
            restClient = new RestClient(serviceBaseUrl, page);
        }

        #region 应计报表

        #region 应计返利报表(周期)
        /// <summary>
        /// 应计返利报表查询（周期）
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="callback">回调函数</param>
        public void AccruedByPeriod(AccruedQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "ExternalSYSService/ExternalSYS/QueryAccruedByPeriod";
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }
        #endregion

        #region 应计返利报表(供应商)
        /// <summary>
        /// 应计返利报表查询（供应商）
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="callback">回调函数</param>
        public void AccruedByVendor(AccruedQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "ExternalSYSService/ExternalSYS/QueryAccruedByVendor";
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }
        #endregion

        #region 应计返利报表(合同)
        /// <summary>
        /// 应计返利报表查询（合同）
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="callback">回调函数</param>
        public void AccruedByRule(AccruedQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "ExternalSYSService/ExternalSYS/QueryAccruedByRule";
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }
        #endregion

        #region 应计返利报表(PM)
        /// <summary>
        /// 应计返利报表查询（PM）
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="callback">回调函数</param>
        public void AccruedByPM(AccruedQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "ExternalSYSService/ExternalSYS/QueryAccruedByPM";
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }
        #endregion

        #endregion

        #region 报表导出
        public void ExportAccruedByPeriod(AccruedQueryFilter filter, ColumnSet[] columns)
        {
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.ExportFile("/ExternalSYSService/ExternalSYS/QueryAccruedByPeriod", filter, columns);
        }

        public void ExportAccruedByVendor(AccruedQueryFilter filter, ColumnSet[] columns)
        {
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.ExportFile("/ExternalSYSService/ExternalSYS/QueryAccruedByVendor", filter, columns);
        }

        public void ExportAccruedByRule(AccruedQueryFilter filter, ColumnSet[] columns)
        {
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.ExportFile("/ExternalSYSService/ExternalSYS/QueryAccruedByRule", filter, columns);
        }

        public void ExportAccruedByPM(AccruedQueryFilter filter, ColumnSet[] columns)
        {
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.ExportFile("/ExternalSYSService/ExternalSYS/QueryAccruedByPM", filter, columns);
        }
        #endregion 
    }
}
