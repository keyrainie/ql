using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.Restful.RequestMsg
{
    public class CheckNetPayReq : BaseCheckPaymentReq<NetPayInfo>
    {
        /// <summary>
        ///  CS确认的订单号
        /// </summary>
        public int? RelatedSOSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 网上支付信息的来源
        /// </summary>
        public NetPayStatus? Status
        {
            get;
            set;
        }

        /// <summary>
        /// 网上支付信息的来源
        /// </summary>
        public NetPaySource? Source
        {
            get;
            set;
        }

        protected override NetPayInfo ConvertPayInfo()
        {
            NetPayInfo netpayInfo = new NetPayInfo();
            netpayInfo.Source = this.Source;
            netpayInfo.Status = this.Status;
            netpayInfo.PayTypeSysNo = this.PayReq.PayTypeSysNo;
            netpayInfo.SOSysNo = this.PayReq.SOSysNo;
            netpayInfo.PayAmount = this.PayReq.PayAmt;
            netpayInfo.RelatedSoSysNo = this.RelatedSOSysNo;

            return netpayInfo;
        }
    }
}