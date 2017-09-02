using ECCentral.QueryFilter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.QueryFilter.PO
{
    public class SettleOrderCreateQueryFilter
    {
        public SettleOrderCreateQueryFilter()
        {
            PageInfo = new PagingInfo();
        }

        public List<int> OrderSysNoList { get; set; }

        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 商家编号
        /// </summary>
        public int? VendorSysNo { get; set; }

        /// <summary>
        /// 进货单
        /// </summary>
        public bool? POPositive { get; set; }

        /// <summary>
        /// 返厂单
        /// </summary>
        public bool? PONegative { get; set; }

        /// <summary>
        /// 进价变价单
        /// </summary>
        public bool? ChangePrice { get; set; }

        /// <summary>
        /// 单据生成时间From
        /// </summary>
        public DateTime? CreateDateFrom { get; set; }

        /// <summary>
        /// 单据生成时间To
        /// </summary>
        public DateTime? CreateDateTo { get; set; }

        public string CompanyCode { get; set; }
    }
}
