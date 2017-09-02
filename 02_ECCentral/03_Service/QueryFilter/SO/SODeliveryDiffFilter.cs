using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.SO;

namespace ECCentral.QueryFilter.SO
{
    public class SODeliveryDiffFilter
    {

        public PagingInfo PageInfo { get; set; }

        public int? SOSysNo { get; set; }

        public SOStatus? SOStatus { get; set; }

        public DateTime? DeliveryDateTimeFrom { get; set; }

        public DateTime? DeliveryDateTimeTo { get; set; }

        public int? FreightMen { get; set; }

        public int? DeliveryAreaNo { get; set; }

        public string CompanyCode { get; set; }
    }
}
