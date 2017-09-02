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
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.QueryFilter.ExternalSYS;

namespace ECCentral.Portal.UI.ExternalSYS.Facades
{
    public class ComprehensiveReportFacade
    {
        private readonly RestClient restClient;
        private string serviceBaseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.Key_ServiceBaseUrl);

        public ComprehensiveReportFacade()
        {
            restClient = new RestClient(serviceBaseUrl);
        }

        public ComprehensiveReportFacade(IPage page)
        {
            restClient = new RestClient(serviceBaseUrl, page);
        }

        #region 综合报表
        /// <summary>
        /// EIMS单据查询
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="callback">回调函数</param>
        public void QueryEIMSInvoice(EIMSInvoiceQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "ExternalSYSService/ExternalSYS/QueryEIMSInvoice";
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        /// <summary>
        /// 合同与对应单据
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="callback">回调函数</param>
        public void QueryUnbilledRuleList(UnbilledRuleListQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "ExternalSYSService/ExternalSYS/QueryUnbilledRuleList";
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        /// <summary>
        /// 综合报表
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="callback">回调函数</param>
        public void QueryComprehensive(EIMSComprehensiveQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "ExternalSYSService/ExternalSYS/QueryComprehensive";
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        #endregion 

        /// <summary>
        /// 数据导出
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="columns"></param>
        public void ExportEIMSInvoice(EIMSInvoiceQueryFilter filter, ColumnSet[] columns)
        {
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.ExportFile("/ExternalSYSService/ExternalSYS/QueryEIMSInvoice", filter, columns);
        }

        public void ExportUnbilledRuleList(UnbilledRuleListQueryFilter filter, ColumnSet[] columns)
        {
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.ExportFile("/ExternalSYSService/ExternalSYS/QueryUnbilledRuleList", filter, columns);
        }

        public void ExportComprehensive(EIMSComprehensiveQueryFilter filter, ColumnSet[] columns)
        {
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.ExportFile("/ExternalSYSService/ExternalSYS/QueryComprehensive", filter, columns);
        }
    }
}
