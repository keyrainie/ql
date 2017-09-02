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
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.Portal.UI.ExternalSYS.Models;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.ExternalSYS.Facades
{
    public class EIMSOrderMgmtFacade
    {
        private readonly RestClient restClient;
        private string serviceBaseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_ExternalSYS, ConstValue.Key_ServiceBaseUrl);

        public EIMSOrderMgmtFacade()
        {
            restClient = new RestClient(serviceBaseUrl);
        }

        public EIMSOrderMgmtFacade(IPage page)
        {
            restClient = new RestClient(serviceBaseUrl, page);
        }

        /// <summary>
        /// EIMS结算类型变更单据查询
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="callback">回调函数</param>
        public void QueryEIMSEventMemo(EIMSEventMemoQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "ExternalSYSService/ExternalSYS/QueryEIMSEventMemo";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        /// <summary>
        /// EIMS发票信息查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void QueryInvoiceInfoList(EIMSInvoiceEntryQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "ExternalSYSService/ExternalSYS/QueryInvoiceInfoList";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        /// <summary>
        /// 根据单据号查询发票信息
        /// </summary>
        /// <param name="invoiceNo">单据号</param>
        /// <returns></returns>
        public void QueryInvoiceList(string invoiceNumber, EventHandler<RestClientEventArgs<EIMSInvoiceInfo>> callback)
        {
            string relativeUrl = string.Format("ExternalSYSService/ExternalSYS/QueryInvoiceList/{0}", invoiceNumber);
            restClient.Query<EIMSInvoiceInfo>(relativeUrl,callback);
        }

        /// <summary>
        /// 根据发票号查询单据信息
        /// </summary>
        /// <param name="invoiceInputSysNos"></param>
        /// <param name="callback"></param>
        public void QueryInvoiceInputByInputSysNo(List<string> invoiceInputSysNos, EventHandler<RestClientEventArgs<EIMSInvoiceInfoEntity>> callback)
        {
            string relativeUrl = string.Format("ExternalSYSService/ExternalSYS/QueryInvoiceInputByInputSysNo/{invoiceInputSysNos}", invoiceInputSysNos);
            restClient.Query<EIMSInvoiceInfoEntity>(relativeUrl, callback);
        }

        /// <summary>
        /// 录入发票信息
        /// </summary>
        /// <param name="viewList"></param>
        /// <param name="callback"></param>
        public void CreateEIMSInvoiceInput(List<EIMSInvoiceInfoEntityVM> viewList, EventHandler<RestClientEventArgs<EIMSInvoiceResult>> callback)
        {
            viewList.ForEach(list =>
                {
                    list.CurrentUser = CPApplication.Current.LoginUser.DisplayName;
                    list.InvoiceInputUser = CPApplication.Current.LoginUser.DisplayName;
                });
            var request = viewList.ConvertVM<EIMSInvoiceInfoEntityVM, EIMSInvoiceInfoEntity, List<EIMSInvoiceInfoEntity>>((o, s) =>
                {
                    o.SysNo = s.SysNo;
                    o.InvoiceInputNo = s.InvoiceInputNo;
                });
            string relativeUrl = "/ExternalSYSService/ExternalSYS/CreateEIMSInvoiceInput";
            restClient.Create<EIMSInvoiceResult>(relativeUrl, request, callback);
        }

        /// <summary>
        /// 修改发票信息
        /// </summary>
        /// <param name="viewList"></param>
        /// <param name="callback"></param>
        public void UpdateEIMSInvoiceInput(List<EIMSInvoiceInfoEntityVM> viewList, EventHandler<RestClientEventArgs<EIMSInvoiceResult>> callback)
        {
            viewList.ForEach(list =>
            {
                list.CurrentUser = CPApplication.Current.LoginUser.DisplayName;
                list.InvoiceInputUser = CPApplication.Current.LoginUser.DisplayName;
            });
            var request = viewList.ConvertVM<EIMSInvoiceInfoEntityVM, EIMSInvoiceInfoEntity, List<EIMSInvoiceInfoEntity>>((o, s) =>
            {
                o.SysNo = s.SysNo;
                o.InvoiceInputNo = s.InvoiceInputNo;
            });
            string relativeUrl = "/ExternalSYSService/ExternalSYS/UpdateEIMSInvoiceInput";
            restClient.Update<EIMSInvoiceResult>(relativeUrl, request, callback);
        }

        public void ExportEventMemo(EIMSEventMemoQueryFilter filter, ColumnSet[] columns)
        {
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.ExportFile("/ExternalSYSService/ExternalSYS/QueryEIMSEventMemo", filter, columns);
        }
    }
}
