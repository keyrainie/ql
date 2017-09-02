using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using ECCentral.BizEntity.Invoice.Refund;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.NeweggCN.Models;
using ECCentral.QueryFilter.Common;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Invoice.Restful.RequestMsg;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.Invoice.NeweggCN.Facades
{
    public class BalanceRefundFacade
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

        public BalanceRefundFacade(IPage page)
        {
            this.viewPage = page;
            this.restClient = new RestClient(InvoiceServiceBaseUrl, page);
        }

        public void Query(BalanceRefundQueryVM queryVM, int pageSize, int pageIndex, string sortField, Action<dynamic> callback)
        {
            BalanceRefundQueryFilter filter = queryVM.ConvertVM<BalanceRefundQueryVM, BalanceRefundQueryFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };

            string relativeUrl = "/InvoiceService/BalanceRefund/Query";
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

        public void ExportExcelFile(BalanceRefundQueryVM queryVM, ColumnSet[] columnSet)
        {
            BalanceRefundQueryFilter queryFilter = queryVM.ConvertVM<BalanceRefundQueryVM, BalanceRefundQueryFilter>();
            queryFilter.PagingInfo = new PagingInfo
            {
                PageSize = ECCentral.Portal.Basic.ConstValue.MaxRowCountLimit,
                PageIndex = 0,
                SortBy = ""
            };
            queryFilter.CompanyCode = CPApplication.Current.CompanyCode;
            string relativeUrl = "/InvoiceService/BalanceRefund/Export";
            restClient.ExportFile(relativeUrl, queryFilter, columnSet);
        }

        public void Update(BalanceRefundVM maintainVM, Action callback)
        {
            var refundEntity = maintainVM.ConvertVM<BalanceRefundVM, BalanceRefundInfo>();
            refundEntity.CompanyCode = CPApplication.Current.CompanyCode;

            string relativeUrl = "/InvoiceService/BalanceRefund/Update";
            restClient.Update(relativeUrl, refundEntity, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }

        public void BatchFinConfirm(List<int> sysNoList, Action<string> callback)
        {
            string relativeUrl = "/InvoiceService/BalanceRefund/FinConfirm";
            restClient.Update<string>(relativeUrl, sysNoList, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        public void BatchCSConfirm(List<int> sysNoList, Action<string> callback)
        {
            string relativeUrl = "/InvoiceService/BalanceRefund/CSConfirm";
            restClient.Update<string>(relativeUrl, sysNoList, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        public void BatchCancelConfirm(List<int> sysNoList, Action<string> callback)
        {
            string relativeUrl = "/InvoiceService/BalanceRefund/CancelConfirm";
            restClient.Update<string>(relativeUrl, sysNoList, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        public void BatchAbandon(List<int> sysNoList, Action<string> callback)
        {
            string relativeUrl = "/InvoiceService/BalanceRefund/Abandon";
            restClient.Update<string>(relativeUrl, sysNoList, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        public void BatchSetReferenceID(List<int> sysNoList, string referenceID, Action<string> callback)
        {
            BatchSetBalanceRefundReferenceIDReq req = new BatchSetBalanceRefundReferenceIDReq();
            req.SysNoList = sysNoList;
            req.ReferenceID = referenceID;

            string relativeUrl = "/InvoiceService/BalanceRefund/SetReferenceID";
            restClient.Update<string>(relativeUrl, req, (obj, args) =>
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