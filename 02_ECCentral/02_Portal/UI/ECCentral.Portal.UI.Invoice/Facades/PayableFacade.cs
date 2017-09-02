using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.Invoice;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.Restful.RequestMsg;
using ECCentral.Service.Invoice.Restful.ResponseMsg;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.Invoice.Facades
{
    public class PayableFacade
    {
        private readonly IPage viewPage;
        private readonly RestClient restClient;

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

        public PayableFacade(IPage page)
        {
            this.viewPage = page;
            this.restClient = new RestClient(ServiceBaseUrl, page);
        }

        public void Query(PayableQueryVM queryVM, int PageSize, int PageIndex, string SortField, EventHandler<RestClientEventArgs<dynamic>> callback)
        {
            PayableQueryFilter queryFilter = new PayableQueryFilter();
            queryFilter = queryVM.ConvertVM<PayableQueryVM, PayableQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = PageSize,
                PageIndex = PageIndex,
                SortBy = SortField
            };
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;

            string relativeUrl = "/InvoiceService/Payable/Query";
            restClient.QueryDynamicData(relativeUrl, queryFilter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        public void ExportExcelFile(PayableQueryVM queryVM, ColumnSet[] columnSet)
        {
            PayableQueryFilter queryFilter = queryVM.ConvertVM<PayableQueryVM, PayableQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = ECCentral.Portal.Basic.ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = null
            };
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InvoiceService/Payable/Export";
            restClient.ExportFile(relativeUrl, queryFilter, columnSet);
        }

        /// <summary>
        /// 更新发票状态
        /// </summary>
        /// <param name="data"></param>
        /// <param name="callback"></param>
        public void UpdateInvoiceStatus(List<PayableInfo> data, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/InvoiceService/Payable/UpdateInvoice";
            restClient.Update<string>(relativeUrl, data, callback);
        }

        public void LoadPayDetailInfoForEdit(PayItemDetailInfoReq request, Action<PaymentOrderMaintainVM> callback)
        {
            string relativeUrl = "/InvoiceService/Payable/LoadForEdit";
            request.CompanyCode = CPApplication.Current.CompanyCode;
            restClient.Query<PayDetailInfoResp>(relativeUrl, request, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                PaymentOrderMaintainVM result = EntityConverter<OrderInfo, PaymentOrderMaintainVM>.Convert(args.Result.OrderInfo);
                result.TotalAmt = args.Result.TotalInfo.TotalAmt;
                result.PaidAmt = args.Result.TotalInfo.PaidAmt;
                result.PayItemList = args.Result.PayItemList.Convert<PayItemInfo, PayItemVM>((s, t) =>
                    {
                        t.PayItemSysNo = s.SysNo;
                        t.IsVendorHolded = args.Result.OrderInfo.IsVendorHoldedControl;
                    });
                callback(result);
            });
        }

        /// <summary>
        /// 取得所有账期类型
        /// </summary>
        /// <param name="callback"></param>
        public void GetAllVendorPayTerms(Action<List<CodeNamePair>> callback)
        {
            string relativeUrl = string.Format("/InvoiceService/Payable/AllVendorPayTerms/{0}", CPApplication.Current.CompanyCode);
            restClient.Query<List<CodeNamePair>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 批量审核
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void BatchAudit(List<PayableInfo> info, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/InvoiceService/Payable/BatchAudit";
            restClient.Update<string>(relativeUrl, info, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        /// <summary>
        /// 批量审核拒绝
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void BatchRefuseAudit(List<PayableInfo> info, EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/InvoiceService/Payable/BatchRefuseAudit";
            restClient.Update<string>(relativeUrl, info, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(obj, args);
            });
        }

        /// <summary>
        /// 批量付款
        /// </summary>
        /// <param name="info"></param>
        /// <param name="callback"></param>
        public void BatchUpdateStatusAndAlreadyPayAmt(List<PayableInfo> info,EventHandler<RestClientEventArgs<string>> callback)
        {
            string relativeUrl = "/InvoiceService/Payable/BatchUpdateStatusAndAlreadyPayAmt";
            restClient.Update<string>(relativeUrl, info, (obj, args) =>
                {
                    if (args.FaultsHandle()) return;
                    callback(obj, args);
                });
        }
    }
}