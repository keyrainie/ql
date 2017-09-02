using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Invoice.Invoice
{
    public class TransactionCheckBill
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 交易类型
        /// </summary>
        public CheckTransactionType? TransactionType { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public int? SoSysNo { get; set; }

        /// <summary>
        /// 支付流水号
        /// </summary>
        public string SerialNo { get; set; }

        /// <summary>
        /// 提交订单时间
        /// </summary>
        public string SubOrderTime { get; set; }

        /// <summary>
        /// 支付平台处理时间
        /// </summary>
        public string ProcessingTime { get; set; }

        /// <summary>
        /// 支付总金额（退款总金额）
        /// </summary>
        public decimal? TotalAmount { get; set; }

        /// <summary>
        /// 商品金额（退商品金额）
        /// </summary>
        public decimal? ProductAmount { get; set; }

        /// <summary>
        /// 结算外汇币种
        /// </summary>
        public string ForexCurrency { get; set; }

        /// <summary>
        /// 结算外汇金额
        /// </summary>
        public decimal? ForexAmount { get; set; }

        /// <summary>
        /// 购汇汇率
        /// </summary>
        public decimal? Exchange { get; set; }

        /// <summary>
        /// 预扣关税（退税）
        /// </summary>
        public decimal? Tariff { get; set; }

        /// <summary>
        /// 运费金额（退费金额）
        /// </summary>
        public decimal? FareAmount { get; set; }

        /// <summary>
        /// 运费币种
        /// </summary>
        public string FareCurrency { get; set; }

        /// <summary>
        /// 商品+运费金额
        /// </summary>
        public decimal? SubtotalAmount { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string MerchantName { get; set; }

    }
}
