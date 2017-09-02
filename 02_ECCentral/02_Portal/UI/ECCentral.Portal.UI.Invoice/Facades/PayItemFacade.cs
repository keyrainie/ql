using System;
using System.Collections.Generic;
using System.Linq;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Portal.UI.Invoice.Utility;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Invoice;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.Invoice.Facades
{
    public class PayItemFacade
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

        public PayItemFacade(IPage page)
        {
            this.viewPage = page;
            this.restClient = new RestClient(InvoiceServiceBaseUrl, page);
        }

        #region NoBizQuery

        /// <summary>
        /// 查询付款单
        /// </summary>
        public void QueryPayItem(PayItemQueryVM query, int pageSize, int pageIndex, string sortField, Action<PayItemQueryResultVM> callback)
        {
            PayItemQueryFilter filter = query.ConvertVM<PayItemQueryVM, PayItemQueryFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };

            string relativeUrl = "/InvoiceService/PayItem/Query";

            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                PayItemQueryResultVM result = new PayItemQueryResultVM();
                if (args.Result[0] != null && args.Result[0].Rows != null)
                {
                    result.ResultList = DynamicConverter<PayItemVM>.ConvertToVMList(args.Result[0].Rows);
                    result.TotalCount = args.Result[0].TotalCount;
                }
                if (args.Result[1] != null && args.Result[1].Rows != null)
                {
                    result.Statistic = DynamicConverter<PayItemQueryStatisticVM>.ConvertToVM(args.Result[1].Rows[0]);
                    result.Statistic.PagePayAmt = result.ResultList.Sum(s => s.PayAmt ?? 0M);
                }
                callback(result);
            });
        }

        /// <summary>
        /// 添加付款单时单据查询
        /// </summary>
        public void QueryCanBePayOrder(PayItemListOrderQueryVM query, int pageSize, int pageIndex, string sortField, Action<dynamic> callback)
        {
            CanBePayOrderQueryFilter filter = query.ConvertVM<PayItemListOrderQueryVM, CanBePayOrderQueryFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };

            string relativeUrl = "/InvoiceService/PayItem/OrderQuery";

            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        public void ExportExcelFile(PayItemQueryVM queryVM, ColumnSet[] columnSet)
        {
            PayItemQueryFilter queryFilter = queryVM.ConvertVM<PayItemQueryVM, PayItemQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = ECCentral.Portal.Basic.ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = ""
            };
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InvoiceService/PayItem/Export";
            restClient.ExportFile(relativeUrl, queryFilter, columnSet);
        }

        #endregion NoBizQuery

        #region 付款单Action

        private void PayItemManualMapper(PayItemVM s, PayItemInfo t)
        {
            t.SysNo = s.PayItemSysNo;
        }

        /// <summary>
        /// 创建付款单
        /// </summary>
        /// <param name="payItem"></param>
        /// <param name="callback"></param>
        public void Create(PayItemVM payItem, Action callback)
        {
            var data = payItem.ConvertVM<PayItemVM, PayItemInfo>(PayItemManualMapper);

            string relativeUrl = "/InvoiceService/PayItem/Create";
            restClient.Create<object>(relativeUrl, data, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }

        /// <summary>
        /// 设置凭证号
        /// </summary>
        /// <param name="payItemList"></param>
        /// <param name="callback"></param>
        public void BatchSetReferenceID(List<PayItemVM> payItemList, Action<string> callback)
        {
            var request = payItemList.ConvertVM<PayItemVM, PayItemInfo, List<PayItemInfo>>(PayItemManualMapper);
            string relativeUrl = "/InvoiceService/PayItem/BatchSetPayItemReferenceID";
            restClient.Update(relativeUrl, request, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result.ToString());
            });
        }

        /// <summary>
        /// 更新付款单
        /// </summary>
        /// <param name="payItem"></param>
        /// <param name="callback"></param>
        public void Update(PayItemVM payItem, Action callback)
        {
            var data = payItem.ConvertVM<PayItemVM, PayItemInfo>(PayItemManualMapper);

            string relativeUrl = "/InvoiceService/PayItem/Update";
            restClient.Update(relativeUrl, data, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }

        /// <summary>
        /// 支付付款单
        /// </summary>
        /// <param name="payItem"></param>
        /// <param name="callback"></param>
        public void Pay(PayItemVM payItem, Action callback)
        {
            var data = payItem.ConvertVM<PayItemVM, PayItemInfo>(PayItemManualMapper);

            string relativeUrl = "/InvoiceService/PayItem/Pay";
            restClient.Update(relativeUrl, data, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }

        /// <summary>
        /// 取消支付付款单
        /// </summary>
        /// <param name="payItem"></param>
        /// <param name="callback"></param>
        public void CancelPay(PayItemVM payItem, Action callback)
        {
            var data = payItem.ConvertVM<PayItemVM, PayItemInfo>(PayItemManualMapper);

            string relativeUrl = "/InvoiceService/PayItem/CancelPay";
            restClient.Update(relativeUrl, data, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }

        /// <summary>
        /// 作废付款单
        /// </summary>
        /// <param name="payItem"></param>
        /// <param name="callback"></param>
        public void Abandon(PayItemVM payItem, Action callback)
        {
            var data = payItem.ConvertVM<PayItemVM, PayItemInfo>(PayItemManualMapper);

            string relativeUrl = "/InvoiceService/PayItem/Abandon";
            restClient.Update(relativeUrl, data, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }

        /// <summary>
        /// 取消作废付款单
        /// </summary>
        /// <param name="payItem"></param>
        /// <param name="callback"></param>
        public void CancelAbandon(PayItemVM payItem, Action callback)
        {
            var data = payItem.ConvertVM<PayItemVM, PayItemInfo>(PayItemManualMapper);

            string relativeUrl = "/InvoiceService/PayItem/CancelAbandon";
            restClient.Update(relativeUrl, data, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }

        /// <summary>
        /// 锁定付款单
        /// </summary>
        /// <param name="payItem"></param>
        /// <param name="callback"></param>
        public void Lock(PayItemVM payItem, Action callback)
        {
            var data = payItem.ConvertVM<PayItemVM, PayItemInfo>(PayItemManualMapper);

            string relativeUrl = "/InvoiceService/PayItem/Lock";
            restClient.Update(relativeUrl, data, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }

        /// <summary>
        /// 取消锁定付款单
        /// </summary>
        /// <param name="payItem"></param>
        /// <param name="callback"></param>
        public void CancelLock(PayItemVM payItem, Action callback)
        {
            var data = payItem.ConvertVM<PayItemVM, PayItemInfo>(PayItemManualMapper);

            string relativeUrl = "/InvoiceService/PayItem/CancelLock";
            restClient.Update(relativeUrl, data, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }

        /// <summary>
        /// 批量支付付款单
        /// </summary>
        /// <param name="payItemList"></param>
        /// <param name="callback"></param>
        public void BatchPay(List<PayItemVM> payItemList, Action<string> callback)
        {
            var request = payItemList.ConvertVM<PayItemVM, PayItemInfo, List<PayItemInfo>>(PayItemManualMapper);

            string relativeUrl = "/InvoiceService/PayItem/BatchPay";
            restClient.Update<string>(relativeUrl, request, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        #endregion 付款单Action
    }
}