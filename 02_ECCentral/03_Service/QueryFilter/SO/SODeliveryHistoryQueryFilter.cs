using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.SO
{
    public class SODeliveryHistoryQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        public int? OrderType { get; set; }
        public string DistrictName { get; set; }
        public int? AreaSysNo { get; set; }
        public DateTime? DeliveryDateFrom { get; set; }
        public DateTime? DeliveryDateTo { get; set; }
        public int? OrderSysNo { get; set; }
        public int? Status { get; set; }
        public int? DeliveryManUserSysNo { get; set; }

        public string CompanyCode { get; set; }
    }
}
