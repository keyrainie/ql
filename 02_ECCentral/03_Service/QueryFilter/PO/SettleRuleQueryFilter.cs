using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.PO;

namespace ECCentral.QueryFilter.PO
{
    public class SettleRuleQueryFilter
    {
        public SettleRuleQueryFilter()
        {
            PageInfo = new PagingInfo();
        }

        public PagingInfo PageInfo { get; set; }


        public int? SysNo { get; set; }

        /// <summary>
        /// 规则编码
        /// </summary>
        public string SettleRuleCode { get; set; }

        /// <summary>
        /// 规则名称
        /// </summary>
        public string SettleRuleName { get; set; }

        public DateTime? CreateDateFrom { get; set; }

        public DateTime? CreateDateTo { get; set; }

        public int? ProductSysNo { get; set; }

        public int? VendorSysNo { get; set; }

        /// <summary>
        ///  状态
        /// </summary>
        public ConsignSettleRuleStatus? Status { get; set; }

        public string CompanyCode { get; set; }
    }
}
