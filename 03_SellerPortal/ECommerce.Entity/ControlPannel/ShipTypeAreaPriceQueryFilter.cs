using ECommerce.Enums;
using ECommerce.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.ControlPannel
{
    public class ShipTypeAreaPriceQueryFilter : QueryFilter
    {
        public int MerchantSysNo { get; set; }
        public int? ShipTypeSysNo { get; set; }
        public int? ProvinceSysNo { get; set; }
        public int? CitySysNo { get; set; }
        public int? DistrictSysNo { get; set; }
    }
}
