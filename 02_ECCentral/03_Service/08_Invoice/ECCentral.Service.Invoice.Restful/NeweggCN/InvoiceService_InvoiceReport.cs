using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.BizEntity.Invoice.InvoiceReport;
using ECCentral.BizEntity.Invoice.Report;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.AppService;
using ECCentral.Service.Invoice.IDataAccess.NoBizQuery;
using ECCentral.Service.Invoice.Restful.RequestMsg;
using ECCentral.Service.Invoice.Restful.ResponseMsg;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.WCF;

namespace ECCentral.Service.Invoice.Restful
{
    public partial class InvoiceService
    {
        [WebInvoke(UriTemplate = "/InvoiceReport/InvoiceDetailReport", Method = "POST")]
        public QueryResult QueryInvoiceDetailReport(InvoiceDetailReportQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IInvoiceReportQueryDA>.Instance.InvoiceDetailReportQuery(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/InvoiceReport/GiftInvoiceDetailReport", Method = "POST")]
        public QueryResult QueryGiftInvoiceDetailReport(GiftInvoiceDetaiReportQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IInvoiceReportQueryDA>.Instance.GiftInvoiceDetailReportQuery(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/InvoiceReport/AllInvoice", Method = "POST")]
        public QueryResult QueryAllInvoice(InvoicePrintAllQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IInvoiceReportQueryDA>.Instance.InvoicePrintAllQuery(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        /// <summary>
        /// 导入运单号
        /// </summary>
        [WebInvoke(UriTemplate = "/InvoiceReport/ImportTrackingNumber", Method = "POST")]
        public ImportTrackingNumberResp ImportInvoiceReportTrackingNumber(ImportTrackingNumberReq request)
        {
            List<TrackingNumberInfo> successList;
            List<TrackingNumberInfo> failedList;

            ObjectFactory<InvoiceReportAppService>.Instance.ImportTrackingNumber(request.FileIdentity, request.StockSysNo.Value
                , out successList, out failedList);

            ImportTrackingNumberResp resp = new ImportTrackingNumberResp();
            resp.SuccessList = successList;
            resp.FailedList = failedList;

            return resp;
        }

        [WebInvoke(UriTemplate = "/InvoiceReport/InvoiceSelfQuery", Method = "POST")]
        public QueryResult QueryInvoiceSelf(InvoiceSelfPrintQueryFilter request)
        {
            int totalCount;
            var dataTable = ObjectFactory<IInvoiceReportQueryDA>.Instance.InvoiceSelfPrintQuery(request, out totalCount);
            return new QueryResult()
            {
                Data = dataTable,
                TotalCount = totalCount
            };
        }

        [WebInvoke(UriTemplate = "/InvoiceReport/InvoiceSelfStockQuery", Method = "GET")]
        public List<CodeNamePair> InvoiceSelfStockQuery()
        {
             var result = ObjectFactory<IInvoiceReportQueryDA>.Instance.InvoiceSelfStockQuery();
             return result;
             //return new QueryResult()
             //{
             //    Data = result.ToDataTable<CodeNamePair>()
             //};
        }

        [WebInvoke(UriTemplate = "/InvoiceReport/GetNew", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public SOInvoiceInfo GetNew(SOInvoiceInfo entity)
        {
            return ObjectFactory<InvoiceReportAppService>.Instance.GetNew(entity);
        }
    }
}