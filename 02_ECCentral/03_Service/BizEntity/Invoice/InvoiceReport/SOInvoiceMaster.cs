using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Invoice.InvoiceReport
{
    public class SOInvoiceMaster
    {
        /// <summary>
        /// 订单No
        /// </summary>
        public int SOSysNo { get; set; }

        /// <summary>
        /// 分仓No
        /// </summary>
        public int StockSysNo { get; set; }

        /// <summary>
        /// 发票总金额
        /// </summary>
        public decimal InvoiceAmt { get; set; }

        /// <summary>
        /// 应收总金额
        /// </summary>
        public decimal SOTotalAmt { get; set; }

        /// <summary>
        /// 商品购买金额
        /// </summary>
        public decimal SOAmt { get; set; }

        /// <summary>
        /// 预付款金额
        /// </summary>
        public decimal PrepayAmt { get; set; }

        /// <summary>
        /// 现金支付额
        /// </summary>
        public decimal CashPay { get; set; }

        /// <summary>
        /// 保价费
        /// </summary>
        public decimal PremiumAmt { get; set; }

        /// <summary>
        /// 运费
        /// </summary>
        public decimal ShippingCharge { get; set; }

        /// <summary>
        /// 手续费
        /// </summary>
        public decimal PayPrice { get; set; }

        /// <summary>
        /// 折扣额
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// 礼品卡支付额
        /// </summary>
        public decimal GiftCardPay { get; set; }

        /// <summary>
        /// 优惠卷支付额
        /// </summary>
        public decimal Promotion { get; set; }

        /// <summary>
        /// 本单积分抵扣
        /// </summary>
        public decimal PointPay { get; set; }

        /// <summary>
        /// 本单可得积分
        /// </summary>
        public int PointAmt { get; set; }

        /// <summary>
        /// 应用优惠券的ID
        /// </summary>
        public int PromotionCodeSysNo { get; set; }

        /// <summary>
        /// 优惠券名称
        /// </summary>
        public string PromotionCodeName { get; set; }

        /// <summary>
        /// 配送方式No
        /// </summary>
        public int ShipTypeSysNo { get; set; }

        /// <summary>
        /// 支付方式No
        /// </summary>
        public int PayTypeSysNo { get; set; }

        /// <summary>
        /// 是否特殊订单
        /// </summary>
        public int SpecialSOType { get; set; }

        /// <summary>
        /// 是否支票支付
        /// </summary>
        [Obsolete("此字段已弃用")]
        public bool IsUseChequesPay { get; set; }

        /// <summary>
        /// 客户是否需要圆通发票
        /// </summary>
        public bool IsRequireShipInvoice { get; set; }

        /// <summary>
        /// 是否并单
        /// </summary>
        public bool IsCombine { get; set; }

        /// <summary>
        /// 是否含有延保服务产品
        /// </summary>
        public bool IsExtendWarrantyOrder { get; set; }

        /// <summary>
        /// 是否已经拆分发票
        /// </summary>
        public bool IsMultiInvoice { get; set; }

        /// <summary>
        /// 是否货到付款
        /// </summary>
        public bool IsPayWhenRecv { get; set; }

        /// <summary>
        /// 商家No
        /// </summary>
        public int MerchantSysNo { get; set; }

        /// <summary>
        /// 仓储类型（NEG：泰隆优选；MET：商家）
        /// </summary>
        public string StockType { get; set; }

        /// <summary>
        /// 配送类型（NEG：泰隆优选；MET：商家）
        /// </summary>
        public string ShippingType { get; set; }

        /// <summary>
        /// 开票类型（NEG：泰隆优选；MET：商家）
        /// </summary>
        public string InvoiceType { get; set; }

        /// <summary>
        /// 业务模式（1-6:正常模式；-1：不正常模式）
        /// </summary>
        public int BussinessMode { get; set; }
    }
}
