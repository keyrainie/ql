using System;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.SO;

namespace ECCentral.QueryFilter.SO
{
    public class OPCQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public string CustomerID { get; set; }

        public WMSAction? ActionType { get; set; }

        public OPCStatus? Status { get; set; }

        public DateTime? CreateTimeFrom { get; set; }

        public DateTime? CreateTimeTo { get; set; }

        public int? SONumber { get; set; }

        public string CompanyCode { get; set; }
    }
}
