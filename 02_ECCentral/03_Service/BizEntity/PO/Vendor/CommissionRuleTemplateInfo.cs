using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.PO.Vendor
{
    /// <summary>
    /// 佣金规则模板
    /// </summary>
    public class CommissionRuleTemplateInfo
    {
        public int? SysNo { get; set; }
        public string SalesRule { get; set; }
        public CommissionRuleStatus? Status { get; set; }
        public int? BrandSysNo { get; set; }
        public int? C1SysNo { get; set; }
        public int? C2SysNo { get; set; }
        public int? C3SysNo { get; set; }
        public int? InUserSysNo { get; set; }
        public string InUserName { get; set; }
        public DateTime? InDate { get; set; }

        public int? EditUserSysNo { get; set; }
        public string EditUserName { get; set; }

        public DateTime? EditDate { get; set; }
        public VendorStagedSaleRuleEntity SaleRuleEntity { get; set; }

        /// <summary>
        /// 店租
        /// </summary>
        public decimal? RentFee { get; set; }
        /// <summary>
        /// 订单提成
        /// </summary>
        public decimal? OrderCommissionAmt { get; set; }
        /// <summary>
        /// 运费
        /// </summary>
        public decimal? DeliveryFee { get; set; }
        public List<CategoryReq> C3SysNos { get; set; }
        public List<int> BrandSysNos { get; set; }

        public class CategoryReq
        {
            public int C1 { get; set; }
            public int C2 { get; set; }
            public int C3 { get; set; }
        }
    }
}
