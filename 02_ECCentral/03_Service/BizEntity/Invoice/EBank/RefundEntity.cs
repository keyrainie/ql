using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Invoice.EBank
{
    public class RefundEntity
    {
        /// <summary>
        /// 订单sosysNO
        /// </summary>
        public int SOSysNo { get; set; }

        /// <summary>
        /// 退款金额
        /// </summary>
        /// <example>
        /// 退款金额的单位是分。
        /// 如果退款金额 103.21 元,那么退款金额就是10321
        /// </example>
        public decimal RefundAmt
        {
            get;
            set;
        }

        /// <summary>
        /// 订单总金额
        /// </summary>
        public decimal SOAmt { get; set; }

        /// <summary>
        /// 退款单单号
        /// </summary>
        public int RefundSysNo
        {
            get;
            set;
        }

        public string CompanyCode { get; set; }

        public DateTime OrderDate { get; set; }
        
        /// <summary>
        /// 货款
        /// </summary>
        public decimal? ProductAmount { get; set; }
        /// <summary>
        /// 税费
        /// </summary>
        public decimal? TaxFeeAmount { get; set; }
        /// <summary>
        /// 运费
        /// </summary>
        public decimal? ShippingFeeAmount { get; set; }
        /// <summary>
        /// 订单类型
        /// </summary>
        public SOIncomeOrderType? OrderType { get; set; }

    }
}
