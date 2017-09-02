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
using ECCentral.BizEntity.Invoice;
using System.Collections.Generic;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Portal.UI.Invoice.NeweggCN.Models;
using ECCentral.BizEntity.Invoice.ReconReport;

namespace ECCentral.Portal.UI.Invoice.Facades
{
    public class HeaderDataFacade
    {
        private readonly RestClient restClient;
        private string serviceBaseUrl = CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_Invoice, ConstValue.Key_ServiceBaseUrl);

        public HeaderDataFacade()
        {
            restClient = new RestClient(serviceBaseUrl);
        }

        public HeaderDataFacade(IPage page)
        {
            restClient = new RestClient(serviceBaseUrl, page);
        }

        /// <summary>
        /// 查询上传SAP数据
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void HeaderDataQuery(HeaderDataQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/InvoiceService/HeaderData/QuerySAPDOCHeader";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        /// <summary>
        /// 查询SAP明细数据
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="callback"></param>
        public void HeaderDataDetailsQuery(HeaderDataQueryFilter filter, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/InvoiceService/HeaderData/QuerySAPDOCHeaderDetails";
            restClient.QueryDynamicData(relativeUrl, filter, callback);
        }

        /// <summary>
        /// 获取公司代码(SAP)
        /// </summary>
        /// <param name="callback"></param>
        public void GetCompanyCode(EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            string relativeUrl = "/InvoiceService/HeaderData/GetCompanyCode";
            restClient.QueryDynamicData(relativeUrl, callback);
        }

        /// <summary>
        /// 重置SAP状态
        /// </summary>
        /// <param name="TransNumbers"></param>
        /// <param name="callback"></param>
        public void UpdateSAPStatus(List<int> TransNumbers, EventHandler<RestClientEventArgs<int>> callback)
        {
            string relativeUrl = "/InvoiceService/HeaderData/UpdateSAPStatus";
            restClient.Update(relativeUrl, TransNumbers, callback);
        }

        /// <summary>
        /// 手动创建报表
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="callback"></param>
        public void CreateReconReportByWeb(GenerateReportVM vm, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            var data = vm.ConvertVM<GenerateReportVM,GenerateReportInfo>();
            string relativeUrl = "/InvoiceService/ReconReport/CreateReconReportByWeb";
            restClient.Create(relativeUrl, data, callback);
        }
    }
}
