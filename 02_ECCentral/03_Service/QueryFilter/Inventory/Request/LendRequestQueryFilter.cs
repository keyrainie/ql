using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.QueryFilter.Inventory
{
    public class LendRequestQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public int? SysNo { get; set; }

        public string RequestID { get; set; }

        public string ProductID { get; set; }

        public int? ProductSysNo { get; set; }

        public int? LendUserSysNo { get; set; }

        public int? PMUserSysNo { get; set; }

        public int? StockSysNo { get; set; }

        public LendRequestStatus? RequestStatus { get; set; }

        public DateTime? CreateDateFrom { get; set; }

        public DateTime? CreateDateTo { get; set; }

        public string CompanyCode { get; set; }
        /// <summary>
        /// PM权限
        /// </summary>
        public BizEntity.Common.PMQueryType? PMQueryRightType { get; set; }

        public int? UserSysNo { get; set; }
    }
}
