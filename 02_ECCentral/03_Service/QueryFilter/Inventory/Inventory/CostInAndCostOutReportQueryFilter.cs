using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Inventory
{
    public class CostInAndCostOutReportQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public string ProductID { get; set; }

        public string ProductName { get; set; }

        public int? Category1SysNo { get; set; }

        public int? Category2SysNo { get; set; }

        public int? Category3SysNo { get; set; }

        public List<int> WarehouseSysNoList { get; set; }

        public List<int> BrandSysNoList { get; set; }

        public List<int> VendorSysNoList { get; set; }

        public DateTime? DateTimeFrom { get; set; }

        public DateTime? DateTimeTo { get; set; }
    }
}
