using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;
using ECommerce.Utility;

namespace ECommerce.Entity.Invoice
{
    public class SettleQueryFilter : QueryFilter
    {
        public int? SysNo { get; set; }
        public int MerchantSysNo { get; set; }
        public SettleOrderType? Type { get; set; }
        public DateTime? IssuingBeginDateTime { get; set; }
        public DateTime? IssuingEndDateTime { get; set; }
        public DateTime? SettledBeginDateTime { get; set; }
        public DateTime? SettledEndDateTime { get; set; }

    }
}
