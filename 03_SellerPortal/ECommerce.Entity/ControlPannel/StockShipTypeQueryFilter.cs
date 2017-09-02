using ECommerce.Enums;
using ECommerce.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.ControlPannel
{
    public class StockShipTypeQueryFilter : QueryFilter
    {
        public int? StockSysNo { get; set; }
        public int SellerSysNo { get; set; }
        public int? ShipTypeSysNo { get; set; }
    }
}
