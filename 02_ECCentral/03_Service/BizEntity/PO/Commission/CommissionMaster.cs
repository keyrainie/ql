using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO
{
    /// <summary>
    /// 佣金信息
    /// </summary>
    public class CommissionMaster : IIdentity, ICompany
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        public int? MerchantSysNo { get; set; }

        public string VendorName { get; set; }

        /// <summary>
        /// 公司编号
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 佣金结算单状态
        /// </summary>
        public VendorCommissionMasterStatus? Status { get; set; }

        /// <summary>
        /// 结算状态
        /// </summary>
        public VendorCommissionSettleStatus? SettleStatus { get; set; }

        /// <summary>
        /// 佣金数额
        /// </summary>
        public decimal? TotalAmt { get; set; }

        /// <summary>
        /// 店租费用
        /// </summary>
        public decimal? RentFee { get; set; }

        /// <summary>
        /// 配送费用
        /// </summary>
        public decimal? DeliveryFee { get; set; }

        /// <summary>
        ///  销售提成
        /// </summary>
        public decimal? SalesCommissionFee { get; set; }

        /// <summary>
        /// 订单提成
        /// </summary>
        public decimal? OrderCommissionFee { get; set; }

        /// <summary>
        /// 佣金对应的商品信息
        /// </summary>
        public List<CommissionItem> ItemList { get; set; }

        public DateTime? BeginDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? InDate { get; set; }

        public string Memo { get; set; }
    }
}
