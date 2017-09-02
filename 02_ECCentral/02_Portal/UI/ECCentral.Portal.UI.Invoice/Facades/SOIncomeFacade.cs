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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Utility;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.Restful.RequestMsg;
using ECCentral.Service.Invoice.Restful.ResponseMsg;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.Invoice.Facades
{
    /// <summary>
    /// 销售收款单Facade
    /// </summary>
    public class SaleIncomeFacade
    {
        private readonly IPage viewPage;
        private readonly RestClient restClient;

        /// <summary>
        /// CustomerService服务基地址
        /// </summary>
        private string ServiceBaseUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue("Invoice", "ServiceBaseUrl");
            }
        }

        public SaleIncomeFacade()
            : this(null)
        {
        }

        public SaleIncomeFacade(IPage page)
        {
            this.viewPage = page;
            this.restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询销售收款单
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="sortField">排序字段（带asc或desc）</param>
        /// <param name="callback">查询结果返回回调</param>
        public void Query(SaleIncomeQueryVM query, int pageSize, int pageIndex, string sortField, Action<SaleIncomeQueryResultVM> callback)
        {
            SOIncomeQueryFilter filter = query.ConvertVM<SaleIncomeQueryVM, SOIncomeQueryFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };

            string relativeUrl = "/InvoiceService/SOIncome/Query";

            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                SaleIncomeQueryResultVM result = new SaleIncomeQueryResultVM();
                if (args.Result[0] != null && args.Result[0].Rows != null)
                {
                    result.ResultList = DynamicConverter<SaleIncomeVM>.ConvertToVMList(args.Result[0].Rows);
                    result.TotalCount = args.Result[0].TotalCount;
                }
                if (args.Result[1] != null && args.Result[1].Rows != null)
                {
                    result.Statistic = DynamicConverter<SaleIncomeQueryStatisticVM>.ConvertToVMList<StatisticCollection<SaleIncomeQueryStatisticVM>>(args.Result[1].Rows);
                }
                callback(result);
            });
        }

        public void ExportSuccessResult(SaleIncomeQueryVM query, ColumnSet[] columnSet)
        {

            SOIncomeQueryFilter filter = query.ConvertVM<SaleIncomeQueryVM, SOIncomeQueryFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = 0,
                PageSize = ECCentral.Portal.Basic.ConstValue.MaxRowCountLimit,
                SortBy = "SysNo desc"
            };

            string relativeUrl = "/InvoiceService/SOIncome/Export";
            restClient.ExportFile(relativeUrl, filter, columnSet);


        }
        /// <summary>
        /// 自动确认收款单时查询未确认收款的订单
        /// </summary>
        public void QuerySO(string soSysNo, int pageSize, int pageIndex, string sortField, Action<dynamic> callback)
        {
            SOQueryFilter filter = new SOQueryFilter();
            filter.SOSysNo = soSysNo;
            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };

            string relativeUrl = "/InvoiceService/SOIncome/QuerySO";
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        public void ExportConfirmFailed(string soSysNo, ColumnSet[] columnSet)
        {
            SOQueryFilter filter = new SOQueryFilter();
            filter.SOSysNo = soSysNo;
            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = 0,
                PageSize = ECCentral.Portal.Basic.ConstValue.MaxRowCountLimit,
                SortBy = "sm.SysNo desc"
            };


            string relativeUrl = "/InvoiceService/SOIncome/ExportQuerySO";
            restClient.ExportFile(relativeUrl, filter, columnSet);

        }
        /// <summary>
        /// 导出收款单
        /// </summary>
        /// <param name="queryVM"></param>
        /// <param name="columnSet"></param>
        public void ExportExcelFile(SaleIncomeQueryVM queryVM, ColumnSet[] columnSet)
        {
            SOIncomeQueryFilter queryFilter = queryVM.ConvertVM<SaleIncomeQueryVM, SOIncomeQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = ECCentral.Portal.Basic.ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = "SysNo desc"
            };
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InvoiceService/SOIncome/Export";
            restClient.ExportFile(relativeUrl, queryFilter, columnSet);
        }

        /// <summary>
        /// 导出RO
        /// </summary>
        /// <param name="queryVM"></param>
        /// <param name="columnSet"></param>
        public void ExportROExcelFile(SaleIncomeQueryVM queryVM, ColumnSet[] columnSet)
        {
            SOIncomeQueryFilter queryFilter = queryVM.ConvertVM<SaleIncomeQueryVM, SOIncomeQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = ECCentral.Portal.Basic.ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = "SysNo desc"
            };
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InvoiceService/SOIncome/ExportRO";
            restClient.ExportFile(relativeUrl, queryFilter, columnSet);
        }

        /// <summary>
        /// 通过订单系统编号取得已确认的退款金额，负值（仅针对多付款退款单）
        /// </summary>
        /// <param name="confirmedSOSysNoList"></param>
        /// <param name="callback"></param>
        public void GetRefundAmtForOPByConfirmedSOSysNoList(List<int> confirmedSOSysNoList, Action<decimal> callback)
        {
            string relativeUrl = "/InvoiceService/SOIncome/GetRefundAmtForOPByConfirmedSOSysNoList";
            restClient.Query<decimal>(relativeUrl, confirmedSOSysNoList, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 批量确认收款单
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <param name="callback"></param>
        public void BatchConfirm(List<int> sysNoList, bool hasRight, Action<string> callback)
        {

            string relativeUrl = "/InvoiceService/SOIncome/BatchConfirm";
            //TODO:请求中蛋收款单确认专用RestfulService
            //string relativeUrl = "/InvoiceService/SOIncome/NECNBatchConfirm";
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
        /// 批量网关手动退款
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <param name="callback"></param>
        public void BatchManualRefund(List<int> sysNoList, Action<string> callback)
        {
            string relativeUrl = "/InvoiceService/SOIncome/BatchManualRefund";
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
        /// 批量取消确认收款单
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <param name="callback"></param>
        public void BatchCancelConfirm(List<int> sysNoList, Action<string> callback)
        {
            string relativeUrl = "/InvoiceService/SOIncome/BatchCancelConfirm";

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
        /// 批量作废收款单
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <param name="callback"></param>
        public void BatchAbandon(List<int> sysNoList, Action<string> callback)
        {
            string relativeUrl = "/InvoiceService/SOIncome/BatchAbandon";

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
        /// 设置收款单实收金额
        /// </summary>
        /// <param name="sysNo">收款单系统编号</param>
        /// <param name="incomceAmt">收款单实收金额</param>
        /// <param name="callback"></param>
        public void SetIncomeAmount(int sysNo, decimal incomceAmt, Action callback)
        {
            string relativeUrl = "/InvoiceService/SOIncome/SetIncomeAmt";

            var request = new SetIncomeAmtReq()
            {
                SysNo = sysNo,
                IncomeAmt = incomceAmt
            };

            restClient.Update<string>(relativeUrl, request, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }

        /// <summary>
        /// 批量设置凭证号
        /// </summary>
        /// <param name="sysNoList">需要批量设置凭证号的收款单系统编号列表</param>
        /// <param name="referenceID">凭证号</param>
        /// <param name="callback"></param>
        public void BatchSetReferenceID(List<int> sysNoList, string referenceID, Action<string> callback)
        {
            string relativeUrl = "/InvoiceService/SOIncome/BatchSetReferenceID";

            var request = new BatchSetSOIncomeReferenceIDReq()
            {
                SysNoList = sysNoList,
                ReferenceID = referenceID
            };

            restClient.Update<string>(relativeUrl, request, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 自动确认收款单
        /// </summary>
        /// <param name="fileIdentity"></param>
        /// <param name="soOutFromDate"></param>
        /// <param name="soOutToDate"></param>
        /// <param name="callback"></param>
        public void AutoConfirm(string fileIdentity, DateTime? soOutFromDate, DateTime? soOutToDate, Action<AutoConfirmSOIncomeResp> callback)
        {
            string relativeUrl = "/InvoiceService/SOIncome/AutoConfirm";

            var request = new AutoConfirmSOIncomeReq()
            {
                FileIdentity = fileIdentity,
                SOOutFromDate = soOutFromDate,
                SOOutToDate = soOutToDate
            };

            restClient.Update<AutoConfirmSOIncomeResp>(relativeUrl, request, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        public void SyncSAPSales(SaleIncomeQueryVM query, int pageSize, int pageIndex, string sortField, Action callback)
        {
            SOIncomeQueryFilter filter = query.ConvertVM<SaleIncomeQueryVM, SOIncomeQueryFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };

            string relativeUrl = "/InvoiceService/SyncSAPSales/SOIncome";
            restClient.Update(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }

        public void BatchForcesConfirm(List<int> sysNoList, Action<string> callback)
        {

            string relativeUrl = "/InvoiceService/SOIncome/BatchForcesConfirm";
            restClient.Update<string>(relativeUrl, sysNoList, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        public void QuerySaleReceivables(SaleReceivablesQueryVM query, int pageSize, int pageIndex, string sortField, Action<dynamic> callback)
        {
            SaleReceivablesQueryFilter filter = query.ConvertVM<SaleReceivablesQueryVM, SaleReceivablesQueryFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };

            string relativeUrl = "/InvoiceService/SOIncome/QuerySaleReceivables";
            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        public void ExportSaleReceivablesExcelFile(SaleReceivablesQueryVM queryVM, ColumnSet[] columnSet)
        {
            SaleReceivablesQueryFilter queryFilter = queryVM.ConvertVM<SaleReceivablesQueryVM, SaleReceivablesQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = ECCentral.Portal.Basic.ConstValue.MaxRowCountLimit,
                PageIndex = 0
            };

            //List<TextInfo> textInfoList = new List<TextInfo>();
            //textInfoList.Add(new TextInfo() { Title = "Neticom (Hong Kong) Limited", Memo = string.Empty });
            //textInfoList.Add(new TextInfo() { Title = "As at", Memo = queryVM.QueryDate.Value.ToString("dd/MM/yyyy") });
            //textInfoList.Add(new TextInfo() { Title = "Reporting currency:", Memo = "RMB / HKD" });

            string relativeUrl = "/InvoiceService/SOIncome/QuerySaleReceivables";
            restClient.ExportFile(relativeUrl, queryFilter, columnSet);
        }

    }
}