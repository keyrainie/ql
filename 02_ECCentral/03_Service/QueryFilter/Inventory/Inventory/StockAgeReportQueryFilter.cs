using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Inventory
{
    public class StockAgeReportQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public DateTime StatisticDate { get; set; }

        public int? C1SysNo { get; set; }

        public int? C2SysNo { get; set; }

        public int? C3SysNo { get; set; }

        public int? ProductSysNo { get; set; }

        public int? StockSysNo { get; set; }

        public int? VendorSysNo { get; set; }

        public List<string> StockAgeTypeList { get; set; }
    }
}
