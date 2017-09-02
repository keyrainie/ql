using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;
using ECommerce.Utility;

namespace ECommerce.Entity.SO
{
    public class AOQueryFilter : QueryFilter
    {
        public int? SOSysNo { get; set; }
        public DateTime? OrderDateBegin { get; set; }
        public DateTime? OrderDateEnd { get; set; }
        public string CustomerID { get; set; }
        public int? PayTypeSysNo { get; set; }
        public RMARefundStatus? Status { get; set; }
        public RefundPayType? RefundPayType { get; set; }
        public int? MerchantSysNo { get; set; }
    }
}
