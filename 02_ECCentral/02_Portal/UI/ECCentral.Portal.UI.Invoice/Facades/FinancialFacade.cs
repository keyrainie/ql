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
using ECCentral.QueryFilter.Invoice;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.UI.Invoice.Models;

namespace ECCentral.Portal.UI.Invoice.Facades
{
    public class FinancialFacade
    {
        private readonly RestClient restClient;
        private string serviceBaseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_Invoice, ConstValue.Key_ServiceBaseUrl);

        public FinancialFacade()
        {
            restClient = new RestClient(serviceBaseUrl);
        }

        public FinancialFacade(IPage page)
        {
            restClient = new RestClient(serviceBaseUrl, page);
        }

        #region 应付款汇总查询
        /// <summary>
        /// 应付款汇总查询
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="callback">回调函数</param>
        public void QueryFinance(FinanceQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "InvoiceService/Invoice/QueryFinance";
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }
        #endregion

        /// <summary>
        /// 获取PMGroup信息
        /// </summary>
        /// <param name="callback"></param>
        public void GetPMGroup(EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "InvoiceService/Invoice/GetPMGroup";
            restClient.QueryDynamicData(relativeUrl, callback);
        }

        /// <summary>
        /// 添加备注
        /// </summary>
        /// <param name="payVM"></param>
        /// <param name="callback"></param>
        public void AddMemo(PayableVM payableVM, EventHandler<RestClientEventArgs<PayableInfo>> callback)
        {
            payableVM.OperationUserFullName = CPApplication.Current.LoginUser.DisplayName;
            payableVM.CompanyCode = CPApplication.Current.CompanyCode;
            var data = payableVM.ConvertVM<PayableVM, PayableInfo>();
            string relativeUrl = "InvoiceService/Invoice/AddMemo";
            restClient.Create<PayableInfo>(relativeUrl, data, callback);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="payableVM"></param>
        /// <param name="callback"></param>
        public void PayableQuery(PayableVM payableVM, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var data = payableVM.ConvertVM<PayableVM, PayableInfo>();
            string relativeUrl = "InvoiceService/Invoice/PaybleQuery";
            restClient.QueryDynamicData(relativeUrl, data, callback);
        }

        /// <summary>
        /// 根据SysNo查询Memo
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void GetMemoBySysNo(int? sysNo, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = string.Format("InvoiceService/Invoice/GetMemoBySysNo/{0}", sysNo.ToString());
            restClient.QueryDynamicData(relativeUrl, callback);
        }

        #region 报表导出
        public void ExportFinance(FinanceQueryFilter filter, ColumnSet[] columns)
        {
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.ExportFile("/InvoiceService/Invoice/ExportFinance", filter, "FinanceExporter", columns);
        }
        public void ExportFinanceVendorByGroup(FinanceQueryFilter filter, ColumnSet[] columns)
        {
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.ExportFile("/InvoiceService/Invoice/ExportFinance", filter, "FinanceGroupByVendorExporter", columns);
        }
        #endregion
    }
}
