using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.QueryFilter.IM
{
    public class GiftCardFabricationFilter
    {
        public ECCentral.QueryFilter.Common.PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }

        public int? POSysNo { get; set; }

        public string Title { get; set; }

        public string CompanyCode { get; set; }

        /// <summary>
        ///到期时间开始于
        /// </summary>
        public DateTime? InDateFrom { get; set; }

        /// <summary>
        /// 到期时间结束于
        /// </summary>
        public DateTime? InDateTo { get; set; }
    }
}
