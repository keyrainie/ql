using System;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.SO;

namespace ECCentral.QueryFilter.SO
{
    public class SOPendingQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public DateTime? CreateTimeFrom { get; set; }

        public DateTime? CreateTimeTo { get; set; }

        public SOPendingStatus? Status { get; set; }

        public int? WarehouseNumber { get; set; }

        public int? UpdateUserSysNo { get; set; }

        public int? SOSysNo
        {
            get;
            set;
        }

        public int? SysNo
        {
            get;
            set;
        }

        public string CompanyCode { get; set; }
    }
}
