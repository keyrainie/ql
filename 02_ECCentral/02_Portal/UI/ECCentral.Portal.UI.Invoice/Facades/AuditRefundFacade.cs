using System;
using System.Collections.Generic;
using System.Linq;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.Restful.RequestMsg;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.Invoice.Facades
{
    public class AuditRefundFacade
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

        public AuditRefundFacade()
        {
            this.restClient = new RestClient(InvoiceServiceBaseUrl);
        }

        public AuditRefundFacade(IPage page)
        {
            this.viewPage = page;
            this.restClient = new RestClient(InvoiceServiceBaseUrl, page);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="sortField"></param>
        /// <param name="callback"></param>
        public void Query(AuditRefundQueryVM query, int pageSize, int pageIndex, string sortField, Action<AuditRefundQueryResultVM> callback)
        {
            AuditRefundQueryFilter filter = query.ConvertVM<AuditRefundQueryVM, AuditRefundQueryFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };

            string relativeUrl = "/InvoiceService/SOIncomeRefund/Query";

            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                AuditRefundQueryResultVM result = new AuditRefundQueryResultVM();
                result.ResultList = DynamicConverter<AuditRefundVM>.ConvertToVMList(args.Result.Rows);
                result.TotalCount = args.Result.TotalCount;

                callback(result);
            });
        }

        /// <summary>
        /// 批量CS审核
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <param name="callback"></param>
        public void BatchCSAudit(List<AuditRefundVM> vmList, Action<string> callback)
        {
            string relativeUrl = "/InvoiceService/SOIncomeRefund/BatchCSAudit";
            var data = vmList.ConvertVM<AuditRefundVM, SOIncomeRefundInfo, List<SOIncomeRefundInfo>>();
            restClient.Update<string>(relativeUrl, data, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 批量CS审核拒绝
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <param name="callback"></param>
        public void BatchCSReject(List<int> sysNoList, Action<string> callback)
        {
            string relativeUrl = "/InvoiceService/SOIncomeRefund/BatchCSReject";

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
        /// 批量财务审核
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <param name="callback"></param>
        public void BatchFinAudit(List<AuditRefundVM> vmList, Action<string> callback)
        {
            string relativeUrl = "/InvoiceService/SOIncomeRefund/BatchFinAudit";

            var data = vmList.ConvertVM<AuditRefundVM, SOIncomeRefundInfo, List<SOIncomeRefundInfo>>();
            restClient.Update<string>(relativeUrl, data, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 批量财务审核拒绝
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <param name="callback"></param>
        public void BatchFinReject(List<int> sysNoList, string finAppendNote, Action<string> callback)
        {
            string relativeUrl = "/InvoiceService/SOIncomeRefund/BatchFinReject";

            var request = new BatchFinRejectSOIncomeRefundReq()
            {
                SysNoList = sysNoList,
                FinAppendNote = finAppendNote
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
        /// 批量取消确认
        /// </summary>
        /// <param name="sysNoList"></param>
        public void BatchCancelAudit(List<int> sysNoList, Action<string> callback)
        {
            string relativeUrl = "/InvoiceService/SOIncomeRefund/BatchCancelAudit";

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
        /// 审核RMA物流拒收
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="callback"></param>
        public void AuditAutoRMA(int sysNo, Action callback)
        {
            string relativeUrl = "/InvoiceService/SOIncomeRefund/AuditAutoRMA";

            restClient.Update(relativeUrl, sysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="callback"></param>
        public void Update(SOIncomeRefundMaintainVM vm, Action callback)
        {
            var entity = vm.ConvertVM<SOIncomeRefundMaintainVM, SOIncomeRefundInfo>((s, t) =>
            {
                t.Status = s.AuditStatus;
                t.OrderSysNo = s.RMANumber; //退款单号
                t.HaveAutoRMA = s.ShipRejected; //是否RMA物流拒收
                t.RefundReason = !string.IsNullOrEmpty(s.RefundReasonSysNo) ? Convert.ToInt32(s.RefundReasonSysNo) : (int?)null;
                t.BankName = s.RefundInfo.BankName;
                t.BranchBankName = s.RefundInfo.BranchBankName;
                t.CardNumber = s.RefundInfo.CardNumber;
                t.CardOwnerName = s.RefundInfo.CardOwnerName;
                t.PostAddress = s.RefundInfo.PostAddress;
                t.PostCode = s.RefundInfo.PostCode;
                t.ReceiverName = s.RefundInfo.CashReceiver;
                t.Note = s.RefundInfo.RefundMemo;
                t.RefundPayType = s.RefundInfo.RefundPayType;
            });

            string relativeUrl = "/InvoiceService/SOIncomeRefund/Update";
            restClient.Update(relativeUrl, entity, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }

        /// <summary>
        /// 神州运通 退预付卡
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="callback"></param>
        public void RefundPrepayCard(RefundPrepayCardVM vm, EventHandler<RestClientEventArgs<int>> callback)
        {
            var data = vm.ConvertVM<RefundPrepayCardVM, RefundPrepayCardInfo>();
            data.UserSysNo = CPApplication.Current.LoginUser.UserSysNo;
            data.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InvoiceService/Invoice/RefundPrepayCard";
            restClient.Update(relativeUrl, data, callback);
        }

        public void ExportExcelFile(AuditRefundQueryVM queryVM, ColumnSet[] columnSet)
        {
            AuditRefundQueryFilter queryFilter = queryVM.ConvertVM<AuditRefundQueryVM, AuditRefundQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = null
            };
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InvoiceService/SOIncomeRefund/Export";
            restClient.ExportFile(relativeUrl, queryFilter, columnSet);
        }
    }
}