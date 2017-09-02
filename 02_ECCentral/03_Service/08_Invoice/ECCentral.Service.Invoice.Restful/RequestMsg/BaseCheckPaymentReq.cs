using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.Restful.RequestMsg
{
    public abstract class BaseCheckPaymentReq<T>
    {
        public PayInfoReqData PayReq
        {
            get;
            set;
        }

        public RefundInfoReqData RefundReq
        {
            get;
            set;
        }

        public bool? IsForceCheck
        {
            get;
            set;
        }

        public void Convert(out T payInfo, out SOIncomeRefundInfo refundInfo)
        {
            payInfo = ConvertPayInfo();
            refundInfo = ConvertRefundInfo();
        }

        protected abstract T ConvertPayInfo();

        protected virtual SOIncomeRefundInfo ConvertRefundInfo()
        {
            SOIncomeRefundInfo refundInfo;
            if (!(this.IsForceCheck ?? false))
            {
                return null;
            }
            refundInfo = new SOIncomeRefundInfo();
            refundInfo.BankName = this.RefundReq.BankName;
            refundInfo.BranchBankName = this.RefundReq.BranchBankName;
            refundInfo.CardNumber = this.RefundReq.CardNumber;
            refundInfo.CardOwnerName = this.RefundReq.CardOwnerName;
            refundInfo.PostAddress = this.RefundReq.PostAddress;
            refundInfo.PostCode = this.RefundReq.PostCode;
            refundInfo.ReceiverName = this.RefundReq.CashReceiver;
            refundInfo.RefundPayType = this.RefundReq.RefundPayType;
            refundInfo.RefundCashAmt = this.RefundReq.RefundCashAmt;
            refundInfo.Note = this.RefundReq.RefundMemo;
            refundInfo.ToleranceAmt = this.RefundReq.ToleranceAmt;

            return refundInfo;
        }
    }

    /// <summary>
    /// 支付信息请求
    /// </summary>
    public class PayInfoReqData
    {
        /// <summary>
        /// 配送方式编号
        /// </summary>
        public int? PayTypeSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 订单号
        /// </summary>
        public int? SOSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal? PayAmt
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 退款信息请求
    /// </summary>
    public class RefundInfoReqData
    {
        /// <summary>
        /// 开户银行
        /// </summary>
        public string BankName
        {
            get;
            set;
        }

        /// <summary>
        /// 支行
        /// </summary>
        public string BranchBankName
        {
            get;
            set;
        }

        /// <summary>
        /// 银行卡号
        /// </summary>
        public string CardNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 持卡人
        /// </summary>
        public string CardOwnerName
        {
            get;
            set;
        }

        /// <summary>
        /// 邮政地址
        /// </summary>
        public string PostAddress
        {
            get;
            set;
        }

        /// <summary>
        /// 邮政编号
        /// </summary>
        public string PostCode
        {
            get;
            set;
        }

        /// <summary>
        /// 收款人
        /// </summary>
        public string CashReceiver
        {
            get;
            set;
        }

        /// <summary>
        /// 退款方式
        /// </summary>
        public RefundPayType? RefundPayType
        {
            get;
            set;
        }

        /// <summary>
        /// 退还现金
        /// </summary>
        public decimal? RefundCashAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 系统精度冗余(>=0)
        /// </summary>
        public decimal? ToleranceAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 退款备注
        /// </summary>
        public string RefundMemo
        {
            get;
            set;
        }
    }
}