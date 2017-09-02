using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.Service.Invoice.AppService;
using ECCentral.Service.Invoice.Restful.RequestMsg;
using ECCentral.Service.Invoice.Restful.ResponseMsg;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.Restful
{
    public partial class InvoiceService
    {
        /// <summary>
        /// 创建电汇邮局付款记录信息
        /// </summary>
        [WebInvoke(UriTemplate = "/PostPay/Create", Method = "POST", ResponseFormat = WebMessageFormat.Json)]
        public void CreatePostPay(CheckPostPayReq request)
        {
            SOIncomeRefundInfo refundInfo = null;
            PostPayInfo postpayInfo = null;
            request.Convert(out postpayInfo, out refundInfo);

            ObjectFactory<PostPayAppService>.Instance.Create(postpayInfo, refundInfo, request.IsForceCheck.Value);
        }

        /// <summary>
        /// 取得银行电汇-邮局付款支付方式列表
        /// </summary>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/PostPay/GetBankOrPostPayTypeList", Method = "GET")]
        public List<PayType> GetBankOrPostPayTypeList()
        {
            return ObjectFactory<PostPayAppService>.Instance.GetBankOrPostPayTypeList();
        }

        /// <summary>
        /// 根据订单系统编号加载postpay信息
        /// </summary>
        /// <param name="soSysNo"></param>
        [WebInvoke(UriTemplate = "/PostPay/Load/{soSysNo}", Method = "GET")]
        public PostPayResp LoadPostPayForCreateBySOSysNo(string soSysNo)
        {
            decimal remainAmt;
            SOBaseInfo soBaseInfo;

            var confirmedOrderList = ObjectFactory<PostPayAppService>.Instance.LoadForCreateBySOSysNo(int.Parse(soSysNo)
                 , out soBaseInfo, out remainAmt);

            PostPayResp resp = new PostPayResp();
            resp.ConfirmedOrderList = confirmedOrderList;
            resp.RemainAmt = remainAmt;
            resp.SOBaseInfo = soBaseInfo;

            return resp;
        }
    }
}