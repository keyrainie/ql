using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.PO
{
    public class PurchaseOrderBatchNumberQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        public int? ProductSysNo { get; set; }

        public int? StockSysNo { get; set; }
    }
}
