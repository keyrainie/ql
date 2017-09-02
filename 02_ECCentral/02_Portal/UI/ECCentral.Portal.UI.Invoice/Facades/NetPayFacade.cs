using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
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
    public class NetPayFacade
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

        public NetPayFacade(IPage page)
        {
            this.viewPage = page;
            this.restClient = new RestClient(InvoiceServiceBaseUrl, page);
        }

        public void Query(NetPayQueryVM req, int pageSize, int pageIndex, string sortField, Action<NetPayQueryResultVM> callback)
        {
            NetPayQueryFilter filter = req.ConvertVM<NetPayQueryVM, NetPayQueryFilter>();

            filter.PagingInfo = new PagingInfo()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                SortBy = sortField
            };

            string relativeUrl = "/InvoiceService/NetPay/Query";

            restClient.QueryDynamicData(relativeUrl, filter, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                NetPayQueryResultVM result = new NetPayQueryResultVM();
                result.ResultList = DynamicConverter<NetPayVM>.ConvertToVMList(args.Result.Rows);
                result.TotalCount = args.Result.TotalCount;

                callback(result);
            });
        }

        public void LoadForAudit(int netpaySysNo, Action<NetPayMaintainVM> callback)
        {
            string relativeUrl = string.Format("/InvoiceService/NetPay/Load/{0}", netpaySysNo);
            restClient.Query<NetPayResp>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                NetPayMaintainVM netpayVM = new NetPayMaintainVM();

                netpayVM.Source = args.Result.NetPay.Source;
                netpayVM.Status = args.Result.NetPay.Status;
                netpayVM.SysNo = args.Result.NetPay.SysNo;
                netpayVM.IsForceCheck = args.Result.Refund != null;
                netpayVM.PayInfo = args.Result.SOBaseInfo.Convert<SOBaseInfo, PayInfoVM>((s, t) =>
                {
                    t.SOSysNo = s.SysNo.ToString();
                    t.SOTotalAmt = s.SOTotalAmount;
                    t.ReceivableAmt = s.OriginalReceivableAmount;
                    t.PrepayAmt = s.PrepayAmount ?? 0;
                });
                netpayVM.PayInfo.PayAmt = args.Result.NetPay.PayAmount.ToString();
                netpayVM.PayInfo.RelatedSOSysNo = args.Result.NetPay.RelatedSoSysNo.ToString();
                netpayVM.PayInfo.GiftCardPayAmt = args.Result.NetPay.GiftCardPayAmt ?? 0;

                if (args.Result.Refund != null)
                {
                    netpayVM.RefundInfo = args.Result.Refund.Convert<SOIncomeRefundInfo, RefundInfoVM>((s, t) =>
                    {
                        t.RefundMemo = s.Note;
                        t.CashReceiver = s.ReceiverName;
                    });
                }
                callback(netpayVM);
            });
        }

        /// <summary>
        /// 创建网上支付
        /// </summary>
        /// <param name="vm"></param>
        /// <param name="callback"></param>
        public void Create(NetPayMaintainVM vm, Action callback)
        {
            CheckNetPayReq req = vm.ConvertVM<NetPayMaintainVM, CheckNetPayReq>((s, t) =>
            {
                t.IsForceCheck = s.IsForceCheck;
                t.RelatedSOSysNo = int.Parse(s.PayInfo.RelatedSOSysNo);
                t.PayReq = s.PayInfo.ConvertVM<PayInfoVM, PayInfoReqData>();
                t.RefundReq = s.RefundInfo.ConvertVM<RefundInfoVM, RefundInfoReqData>();
            });
            string relativeUrl = "/InvoiceService/NetPay/Create";
            restClient.Create<object>(relativeUrl, req, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }

        /// <summary>
        /// 批量审核网上支付
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <param name="callback"></param>
        public void BatchAudit(List<int> sysNoList, Action<string> callback)
        {
            string relativeUrl = "/InvoiceService/NetPay/BatchAudit";

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
        /// 作废单条网上支付
        /// </summary>
        /// <param name="sysNoList"></param>
        /// <param name="callback"></param>
        public void Abandon(int sysNo, Action callback)
        {
            string relativeUrl = "/InvoiceService/NetPay/Abandon";
            restClient.Update(relativeUrl, sysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }
    }
}