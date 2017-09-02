using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.SO
{
    public class SODeliveryAssignTaskQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        public int? ShipType { get; set; }
        public int? DeliveryMan { get; set; }
        public DateTime? DeliveryTime { get; set; }
        public int? DeliveryTimeRange { get; set; }
        public int? OrderType { get; set; }
        public int? OrderSysNo { get; set; }
        public int? Area { get; set; }
        public List<int> SOSysNos { get; set; }
        public int? PayType { get; set; }
        public DateTime? OutStockTimeFrom { get; set; }
        public DateTime? OutStockTimeTo { get; set; }
        public decimal TotalAmt { get; set; }

        public string CompanyCode { get; set; }
    }
}
