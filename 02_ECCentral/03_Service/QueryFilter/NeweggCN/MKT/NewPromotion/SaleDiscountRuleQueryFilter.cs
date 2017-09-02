using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.MKT
{
    public class SaleDiscountRuleQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        public string ActivityName { get; set; }

        /// <summary>
        /// 状态：0-无效，1-有效
        /// </summary>
        public int? Status { get; set; }

        public DateTime? BeginDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? C1SysNo { get; set; }
        public int? C2SysNo { get; set; }
        public int? C3SysNo { get; set; }
        public int? BrandSysNo { get; set; }

        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        public string ProductID { get; set; }

        public string CompanyCode { get; set; }

        public string ChannelID { get; set; }

        /// <summary>
        /// 所属商家
        /// </summary>
        public int? VendorSysNo { get; set; }
    }
}
