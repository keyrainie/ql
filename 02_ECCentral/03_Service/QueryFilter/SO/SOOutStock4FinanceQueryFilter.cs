using System;
using ECCentral.BizEntity.SO;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Common;

namespace ECCentral.QueryFilter.SO
{
    public class SOOutStock4FinanceQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        public DateTime? DeliveryDateTime { get; set; }

        public int? DeliveryTimeRange { get; set; }

        public int? StockSysNo { get; set; }

        public int? ReceiveAreaSysNo { get; set; }

        public BooleanType? ISVAT { get; set; }

        public SOIsSpecialOrder? SpecialSOType { get; set; }

        public EnoughFlag? EnoughFlag { get; set; }

        public string CompanyCode { get; set; }

        public int? ShipTypeSysNo { get; set; }

        public ConditionType ShipTypeCondition { get; set; }
    }
}
