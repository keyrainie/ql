using ECCentral.BizEntity.Invoice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO.Settlement
{
    public class SettleInfo
    {
        public int? SysNo { get; set; }

        public int? VendorSysNo { get; set; }

        public string VendorName { get; set; }

        public int? StockSysNo { get; set; }

        public decimal? TotalAmt { get; set; }

        public DateTime? CreateTime { get; set; }

        public int? CreateUserSysNo { get; set; }

        public DateTime? AuditTime { get; set; }

        public int? AuditUserSysNo { get; set; }

        public DateTime? EditTime { get; set; }

        public int? EditUserSysNo { get; set; }

        public POSettleStatus? Status { get; set; }

        public string CompanyCode { get; set; }

        /// <summary>
        /// 应付税金
        /// </summary>
        public decimal RateAmount { get; set; }

        /// <summary>
        /// 应付价税合计
        /// </summary>
        public decimal RateTotal { get; set; }

        /// <summary>
        /// 税金(其他)
        /// </summary>
        public decimal RateAmountOther { get; set; }

        /// <summary>
        /// 税金(13%)
        /// </summary>
        public decimal RateCost13 { get; set; }

        /// <summary>
        /// 税金(17%)
        /// </summary>
        public decimal RateCost17 { get; set; }

        /// <summary>
        /// 价款(17%)
        /// </summary>
        public decimal Cost17 { get; set; }

        /// <summary>
        /// 价款(13%)
        /// </summary>
        public decimal Cost13 { get; set; }

        /// <summary>
        /// 价款(其它)
        /// </summary>
        public decimal CostOther { get; set; }

        public List<SettleItemInfo> SettleItemInfos { get; set; }
    }

    public class SettleItemInfo
    {
        public int? SysNo { get; set; }

        public int? SettleSysNo { get; set; }

        public decimal? TaxRate { get; set; }

        public decimal? PayAmt { get; set; }

        public int? OrderSysNo { get; set; }

        public int? OrderType { get; set; }

        public PayableOrderType? FinancePayOrderType { get; set; }

        //结算单编号
        public int? FinancePaySysNo { get; set; }

        public string OrderTypeStr { get; set; }

        public string CompanyCode { get; set; }


        /// <summary>
        /// 应付价款
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// 应付税金
        /// </summary>
        public decimal RateAmount { get; set; }

        /// <summary>
        /// 应付价税合计
        /// </summary>
        public decimal RateCost { get; set; }

        /// <summary>
        /// 税金(13%)
        /// </summary>
        public decimal Rate13 { get; set; }

        /// <summary>
        /// 价款(13%)
        /// </summary>
        public decimal Cost13 { get; set; }

        /// <summary>
        /// 税金(17%)
        /// </summary>
        public decimal Rate17 { get; set; }

        /// <summary>
        /// 价款(17%)
        /// </summary>
        public decimal Cost17 { get; set; }

        /// <summary>
        /// 价款(其它)
        /// </summary>
        public decimal CostOther { get; set; }

        /// <summary>
        /// 税金(其他)
        /// </summary>
        public decimal RateOther { get; set; }

    }


}
