using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Inventory;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.PO;

namespace ECCentral.QueryFilter.Inventory
{
    public class StockShiftConfigFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public int? OutStockSysNo { get; set; }

        public int? InStockSysNo { get; set; }

        public int? SPLInterval { get; set; }

        public int? ShipInterval { get; set; }

        public string ShiftType { get; set; }

        public string CompanyCode { get; set; }
    }
}
