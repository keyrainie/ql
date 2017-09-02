using ECCentral.BizEntity.Inventory;
using ECCentral.QueryFilter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.QueryFilter.Inventory
{
    public class ExperienceHallAllocateOrderQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public int? SysNo { get; set; }

        public int? StockSysNo { get; set; }

        public int? ProductSysNo { get; set; }

        public int? ProductName { get; set; }

        public DateTime? InDate { get; set; }

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public AllocateType? AllocateType { get; set; }

        public ExperienceHallStatus? ExperienceHallStatus { get; set; }
    }
}
