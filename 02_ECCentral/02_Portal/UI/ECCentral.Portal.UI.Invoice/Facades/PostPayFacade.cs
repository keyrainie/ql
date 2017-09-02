using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Invoice.Models;
using ECCentral.Service.Invoice.Restful.RequestMsg;
using ECCentral.Service.Invoice.Restful.ResponseMsg;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;

namespace ECCentral.Portal.UI.Invoice.Facades
{
    public class PostPayFacade
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

        public PostPayFacade()
            : this(null)
        {
        }

        public PostPayFacade(IPage page)
        {
            this.viewPage = page;
            this.restClient = new RestClient(ServiceBaseUrl, page);
        }

        /// <summary>
        /// 取得银行电汇-邮局付款支付方式列表
        /// </summary>
        /// <param name="callback"></param>
        public void LoadPayTypeList(Action<List<PayType>> callback)
        {
            string relativeUrl = "/InvoiceService/PostPay/GetBankOrPostPayTypeList";
            restClient.Query<List<PayType>>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback(args.Result);
            });
        }

        /// <summary>
        /// 创建PostPay
        /// </summary>
        /// <param name="postPayVM"></param>
        /// <param name="callback"></param>
        public void Create(PostPayVM postPayVM, Action callback)
        {
            CheckPostPayReq req = postPayVM.ConvertVM<PostPayVM, CheckPostPayReq>((s, t) =>
            {
                t.IsForceCheck = s.IsForceCheck;
                t.PayReq = s.PayInfo.ConvertVM<PayInfoVM, PayInfoReqData>();
                t.RefundReq = s.RefundInfo.ConvertVM<RefundInfoVM, RefundInfoReqData>();
            });
            string relativeUrl = "/InvoiceService/PostPay/Create";
            restClient.Create<object>(relativeUrl, req, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                callback();
            });
        }

        public void LoadForEdit(int soSysNo, Action<PostPayVM> callback)
        {
            string relativeUrl = string.Format("/InvoiceService/PostPay/Load/{0}", soSysNo);
            restClient.Query<PostPayResp>(relativeUrl, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                PostPayVM postpayVM = new PostPayVM();
                postpayVM.ConfirmedOrderList = args.Result.ConfirmedOrderList.Convert<PostIncomeConfirmInfo, ConfirmedOrderVM, List<ConfirmedOrderVM>>((s, t) =>
                {
                    t.ConfirmStatus = s.Status;
                    t.SOSysNo = s.ConfirmedSoSysNo;
                })
                .Where(w => w.ConfirmStatus != PostIncomeConfirmStatus.Cancel)
                .ToList();
                postpayVM.PayInfo = args.Result.SOBaseInfo.Convert<SOBaseInfo, PayInfoVM>((s, t) =>
                {
                    t.SOSysNo = s.SysNo.ToString();
                    t.PrepayAmt = s.PrepayAmount ?? 0;
                    t.ReceivableAmt = s.ReceivableAmount;
                    t.SOTotalAmt = s.SOTotalAmount;
                    t.GiftCardPayAmt = s.GiftCardPay ?? 0;
                });
                postpayVM.RefundInfo = new RefundInfoVM();
                postpayVM.RemainAmt = args.Result.RemainAmt;

                callback(postpayVM);
            });
        }
    }
}