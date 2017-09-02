using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.SO
{
    public class WHSOOutStockQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        public DateTime? DeliveryDateTime { get; set; }
        public String DeliveryTimeRange { get; set; }
        public int? StockSysNo { get; set; }
        public DateTime? AuditDateTimeFrom { get; set; }
        public DateTime? AuditDateTimeTo { get; set; }
        public int? ShipTypeSysNo { get; set; }
        public int? ReceiveAreaSysNo { get; set; }
        public int? ISVAT { get; set; }
        public int? SpecialSOType { get; set; }
        public int? EnoughFlag { get; set; }
        public int? ShipTypeCondition { get; set; }

        public string CompanyCode { get; set; }
    }
}
