using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Inventory
{
    public class StockQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public string StockSysNo { get; set; }

        public string StockID { get; set; }

        public string StockName { get; set; }

        public int? StockStatus { get; set; }
       
        public int? WarehouseSysNo { get; set; }

        public string WebChannelID { get; set; }

        public int? WebChannelSysNo { get; set; }

        public string CompanyCode { get; set; }

    }

    public class StockQuerySimpleFilter
    {

        public string WebChannelID { get; set; }

        public int? MerchantSysNo { get; set; }
    }
}
