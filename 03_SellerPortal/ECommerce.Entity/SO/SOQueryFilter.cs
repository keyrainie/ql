using System;
using ECommerce.Enums;
using ECommerce.Utility;

namespace ECommerce.Entity.SO
{
    public class SOQueryFilter : QueryFilter
    {
        public int? SOSysNo { get; set; }
        public DateTime? OrderDateBegin { get; set; }
        public DateTime? OrderDateEnd { get; set; }
        public string CustomerID { get; set; }
        public int? ProductSysNo { get; set; }
        public string ReceivePhone { get; set; }
        public SOStatus? SOStatus { get; set; }
        //public SOPaymentStatus? PaymentStatus { get; set; }
        //public SOIncomeStatus? SOIncomeStatus { get; set; }
        public ConsolidatedPaymentStatus? ConsolidatedPaymentStatus { get; set; }
        public int? PayTypeSysNo { get; set; }
        public int? ShipTypeSysNo { get; set; }
        public int? MerchantSysNo { get; set; }
    }
}
