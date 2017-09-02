using System;
using System.Collections.Generic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.NeweggCN.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.Restful.RequestMsg;
using ECCentral.Service.Invoice.Restful.ResponseMsg;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.Invoice.NeweggCN.Facades
{
    public class InvoiceReportFacade
    {
        private readonly IPage viewPage;
        private readonly RestClient restClient;

        /// <summary>
        /// InvoiceService服务基地址
        /// </summary>
        private string InvoiceServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Invoice", "ServiceBaseUrl");
            }
        }

        public InvoiceReportFacade(IPage page)
        {
            this.viewPage = page;
            this.restClient = new RestClient(InvoiceServiceBaseUrl, page);
        }

        /// <summary>
        /// 发票明细报表查询
        /// </summary>
        public void QueryInvoiceDetailReport(InvoiceDetailReportQueryVM queryVM, int pageSize, int pageIndex, string sortField, Action<dynamic> callback)
        {
            InvoiceDetailReportQueryFilter filter = queryVM.ConvertVM<InvoiceDetailReportQueryVM, InvoiceDetailReportQueryFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };

            string relativeUrl = "/InvoiceService/InvoiceReport/InvoiceDetailReport";
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 发票明细表导出
        /// </summary>
        /// <param name="queryVM"></param>
        /// <param name="columnSet"></param>
        public void ExportInvoiceDetailReportExcelFile(InvoiceDetailReportQueryVM queryVM, ColumnSet[] columnSet)
        {
            InvoiceDetailReportQueryFilter queryFilter = queryVM.ConvertVM<InvoiceDetailReportQueryVM, InvoiceDetailReportQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = ECCentral.Portal.Basic.ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = ""
            };
            string relativeUrl = "/InvoiceService/InvoiceReport/InvoiceDetailReport";
            restClient.ExportFile(relativeUrl, queryFilter, columnSet);
        }

        /// <summary>
        /// 礼品卡发票明细表查询
        /// </summary>
        /// <param name="queryVM"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="sortField"></param>
        /// <param name="callback"></param>
        public void QueryGiftInvoiceDetailReport(GiftInvoiceDetaiReportQueryVM queryVM, int pageSize, int pageIndex, string sortField, Action<dynamic> callback)
        {
            GiftInvoiceDetaiReportQueryFilter filter = queryVM.ConvertVM<GiftInvoiceDetaiReportQueryVM, GiftInvoiceDetaiReportQueryFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };

            string relativeUrl = "/InvoiceService/InvoiceReport/GiftInvoiceDetailReport";
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 礼品卡发票明细表导出
        /// </summary>
        /// <param name="queryVM"></param>
        /// <param name="columnSet"></param>
        public void ExportGiftInvoiceDetailReportExcelFile(GiftInvoiceDetaiReportQueryVM queryVM, ColumnSet[] columnSet)
        {
            GiftInvoiceDetaiReportQueryFilter queryFilter = queryVM.ConvertVM<GiftInvoiceDetaiReportQueryVM, GiftInvoiceDetaiReportQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = ECCentral.Portal.Basic.ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = ""
            };
            string relativeUrl = "/InvoiceService/InvoiceReport/GiftInvoiceDetailReport";
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.ExportFile(relativeUrl, queryFilter, columnSet);
        }

        /// <summary>
        /// 发票打印查询所有发票
        /// </summary>
        /// <param name="queryVM"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="sortField"></param>
        /// <param name="callback"></param>
        public void QueryAllInvoice(InvoicePrintAllQueryVM queryVM, int pageSize, int pageIndex, string sortField, Action<dynamic> callback)
        {
            InvoicePrintAllQueryFilter filter = queryVM.ConvertVM<InvoicePrintAllQueryVM, InvoicePrintAllQueryFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };

            string relativeUrl = "/InvoiceService/InvoiceReport/AllInvoice";
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 发票打印导出
        /// </summary>
        /// <param name="queryVM"></param>
        /// <param name="columnSet"></param>
        public void ExportAllInvoiceExcelFile(InvoicePrintAllQueryVM queryVM, ColumnSet[] columnSet)
        {
            InvoicePrintAllQueryFilter queryFilter = queryVM.ConvertVM<InvoicePrintAllQueryVM, InvoicePrintAllQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = ECCentral.Portal.Basic.ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = ""
            };
            string relativeUrl = "/InvoiceService/InvoiceReport/AllInvoice";
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.ExportFile(relativeUrl, queryFilter, columnSet);
        }

        /// <summary>
        /// 导入运单号
        /// </summary>
        /// <param name="fileIdentity"></param>
        /// <param name="stockSysNo"></param>
        /// <param name="callback"></param>
        public void ImportTrackingNumber(string fileIdentity, int stockSysNo, Action<ImportTrackingNumberResp> callback)
        {
            ImportTrackingNumberReq request = new ImportTrackingNumberReq();
            request.FileIdentity = fileIdentity;
            request.StockSysNo = stockSysNo;

            string relativeUrl = "/InvoiceService/InvoiceReport/ImportTrackingNumber";
            restClient.Create<ImportTrackingNumberResp>(relativeUrl, request, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 自印发票系统查询
        /// </summary>
        /// <param name="queryVM"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="sortField"></param>
        /// <param name="callback"></param>
        public void QueryInvoiceSelf(InvoiceSelfPrintQueryVM queryVM, int pageSize, int pageIndex, string sortField, Action<dynamic> callback)
        {
            InvoiceSelfPrintQueryFilter filter = queryVM.ConvertVM<InvoiceSelfPrintQueryVM, InvoiceSelfPrintQueryFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };

            string relativeUrl = "/InvoiceService/InvoiceReport/InvoiceSelfQuery";
            filter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 支持自印发票的仓库列表
        /// </summary>
        /// <param name="queryVM"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="sortField"></param>
        /// <param name="callback"></param>
        public void QueryInvoiceSelfStock(EventHandler<RestClientEventArgs<List<CodeNamePair>>> callbackAction)
        {
            string relativeUrl = "/InvoiceService/InvoiceReport/InvoiceSelfStockQuery";
            restClient.Query(relativeUrl, callbackAction);
        }

        /// <summary>
        /// 自印发票导出
        /// </summary>
        /// <param name="queryVM"></param>
        /// <param name="columnSet"></param>
        public void ExportInvoiceSelfExcelFile(InvoiceSelfPrintQueryVM queryVM, ColumnSet[] columnSet)
        {
            InvoiceSelfPrintQueryFilter queryFilter = queryVM.ConvertVM<InvoiceSelfPrintQueryVM, InvoiceSelfPrintQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = ECCentral.Portal.Basic.ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = ""
            };
            string relativeUrl = "/InvoiceService/InvoiceReport/InvoiceSelfQuery";
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.ExportFile(relativeUrl, queryFilter, columnSet);
        }
    }
}