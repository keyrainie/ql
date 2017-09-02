using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.Restful.RequestMsg
{
    public class CheckPostPayReq : BaseCheckPaymentReq<PostPayInfo>
    {
        /// <summary>
        /// 收款单剩余金额
        /// </summary>
        public decimal? RemainAmt
        {
            get;
            set;
        }

        /// <summary>
        /// CS确认的订单号
        /// </summary>
        public int? ConfirmedSOSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 构造PostPayInfo数据
        /// </summary>
        protected override PostPayInfo ConvertPayInfo()
        {
            PostPayInfo postpayInfo = new PostPayInfo();
            postpayInfo.PayTypeSysNo = this.PayReq.PayTypeSysNo;
            postpayInfo.SOSysNo = this.PayReq.SOSysNo;
            postpayInfo.PayAmount = this.PayReq.PayAmt;
            postpayInfo.ConfirmedSOSysNo = this.ConfirmedSOSysNo;
            postpayInfo.RemainAmt = this.RemainAmt;

            return postpayInfo;
        }
    }
}