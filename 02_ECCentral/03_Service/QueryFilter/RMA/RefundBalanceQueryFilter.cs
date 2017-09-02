using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.RMA
{
    public class RefundBalanceQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }
        /// <summary>
        /// 退款单号
        /// </summary>
        public int? RefundSysNo { get; set; }
    }
}
