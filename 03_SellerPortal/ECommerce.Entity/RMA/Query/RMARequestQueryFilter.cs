using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility;

namespace ECommerce.Entity.RMA
{
    public class RMARequestQueryFilter : QueryFilter
    {
        public string SOSysNo { get; set; }

        public string OrderDateFrom { get; set; }

        public string OrderDateTo { get; set; }

        public string CustomerID { get; set; }

        public string RequestID { get; set; }

        public string RequestDateFrom { get; set; }

        public string RequestDateTo { get; set; }

        public string Status { get; set; }

        public string ProductSysNo { get; set; }

        public string ReceiverPhone { get; set; }

        public int SellerSysNo { get; set; }
    }
}
