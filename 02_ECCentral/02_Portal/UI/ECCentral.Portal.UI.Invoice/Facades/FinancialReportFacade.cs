using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.RMA;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Facades.RequestMsg;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Utility;
using ECCentral.Portal.UI.Invoice.Views.FinancialReport;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Invoice;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.Invoice.Facades
{
    public class FinancialReportFacade
    {
        private readonly IPage viewPage;
        private readonly RestClient restClient;

        public FinancialReportFacade()
            : this(null)
        {
        }

        public FinancialReportFacade(IPage viewPage)
        {
            this.viewPage = viewPage;
            restClient = new RestClient(ServiceBaseUrl, viewPage);
        }

        /// <summary>
        /// InvoiceService服务基地址
        /// </summary>
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Invoice", "ServiceBaseUrl");
            }
        }

        public void IncomeCostReportQuery(IncomeCostReportQueryVM queryVM,
            int pageSize,
            int pageIndex,
            string sortField,
            Action<IncomeCostReportQueryResultVM> callback)
        {
            var filter = queryVM.ConvertVM<IncomeCostReportQueryVM, IncomeCostReportQueryFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };

            const string relativeUrl = "/InvoiceService/IncomeCostReport/Query";
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                var result = new IncomeCostReportQueryResultVM();
                if (args.Result[0] != null && args.Result[0].Rows != null)
                {
                    result.ResultList = DynamicConverter<IncomeCostReportVM>.ConvertToVMList(args.Result[0].Rows);
                    result.TotalCount = args.Result[0].TotalCount;
                }

                if (args.Result[0] != null && args.Result[1].Rows != null)
                {
                    result.StatisticList =
                        DynamicConverter<IncomeCostReportStatisticVM>
                            .ConvertToVMList<StatisticCollection<IncomeCostReportStatisticVM>>(args.Result[1].Rows);
                    if (result.StatisticList.Count > 0)
                    {
                        result.StatisticList[0].StatisticType = StatisticType.Page;
                        result.StatisticList[1].StatisticType = StatisticType.Total;
                    }
                }
                callback(result);
            });
        }

        public void IncomeCostReportExportExcelFile(IncomeCostReportQueryVM queryVM, ColumnSet[] columnSets)
        {
            var filter = queryVM.ConvertVM<IncomeCostReportQueryVM, IncomeCostReportQueryFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = 0,
                PageSize = ECCentral.Portal.Basic.ConstValue.MaxRowCountLimit,
                SortBy = null
            };

            const string relativeUrl = "/InvoiceService/IncomeCostReport/Export";
            restClient.ExportFile(relativeUrl, filter, columnSets);
        }

        public void SalesStatisticsReportQuery(SalesStatisticsReportQueryVM queryVM,
            int pageSize,
            int pageIndex,
            string sortField,
            Action<SalesStatisticsReportQueryResultVM> callback)
        {
            var filter = queryVM.ConvertVM<SalesStatisticsReportQueryVM, SalesStatisticsReportQueryFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };

            const string relativeUrl = "/InvoiceService/SalesStatisticsReport/Query";
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                var result = new SalesStatisticsReportQueryResultVM();
                if (args.Result[0] != null && args.Result[0].Rows != null)
                {
                    result.ResultList = DynamicConverter<SalesStatisticsReportVM>.ConvertToVMList(args.Result[0].Rows);
                    result.TotalCount = args.Result[0].TotalCount;
                }

                if (args.Result[1] != null && args.Result[1].Rows != null)
                {
                    result.StatisticList = DynamicConverter<SalesStatisticsReportStatisticVM>.ConvertToVMList<StatisticCollection<SalesStatisticsReportStatisticVM>>(args.Result[1].Rows);
                    if (result.StatisticList.Count > 0)
                    {
                        result.StatisticList[0].StatisticType = StatisticType.Page;
                        result.StatisticList[1].StatisticType = StatisticType.Total;
                    }
                }

                callback(result);
            });
        }

        public void SalesStatisticsReportExportExcelFile(SalesStatisticsReportQueryVM queryVM, ColumnSet[] columnSets)
        {
            var filter = queryVM.ConvertVM<SalesStatisticsReportQueryVM, SalesStatisticsReportQueryFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = 0,
                PageSize = ECCentral.Portal.Basic.ConstValue.MaxRowCountLimit,
                SortBy = null
            };

            const string relativeUrl = "/InvoiceService/SalesStatisticsReport/Export";
            restClient.ExportFile(relativeUrl, filter, columnSets);
        }


        public void QuerySOFreightStatDetai(SOFreightStatDetailQueryVM query, int pageSize, int pageIndex, string sortField, Action<dynamic> callback)
        {
            SOFreightStatDetailQueryFilter filter = query.ConvertVM<SOFreightStatDetailQueryVM, SOFreightStatDetailQueryFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };

            string relativeUrl = "/InvoiceService/SOIncome/QuerySOFreightStatDetai";
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        public void ExportSOFreightStatDetai(SOFreightStatDetailQueryVM queryVM, ColumnSet[] columnSet)
        {
            SOFreightStatDetailQueryFilter queryFilter = queryVM.ConvertVM<SOFreightStatDetailQueryVM, SOFreightStatDetailQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = ECCentral.Portal.Basic.ConstValue.MaxRowCountLimit,
                PageIndex = 0
            };

            string relativeUrl = "/InvoiceService/SOIncome/QuerySOFreightStatDetai";
            restClient.ExportFile(relativeUrl, queryFilter, columnSet);
        }

        /// <summary>
        /// 批量确认运费支出
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <param name="callback"></param>
        public void BatchRealFreightConfirm(List<int> sysNoList, Action<string> callback)
        {
            string relativeUrl = "/InvoiceService/SOIncome/BatchRealFreightConfirm";

            restClient.Update<string>(relativeUrl, sysNoList, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 批量确认运费收入
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <param name="callback"></param>
        public void BatchSOFreightConfirm(List<int> sysNoList, Action<string> callback)
        {
            string relativeUrl = "/InvoiceService/SOIncome/BatchSOFreightConfirm";

            restClient.Update<string>(relativeUrl, sysNoList, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }
    }
}
