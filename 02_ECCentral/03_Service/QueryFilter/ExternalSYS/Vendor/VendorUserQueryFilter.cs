using ECCentral.QueryFilter.Common;
using System;
using ECCentral.BizEntity.PO;

namespace ECCentral.QueryFilter.ExternalSYS
{
    public class VendorUserQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public int? VendorSysNo { get; set; }
        public string VendorName { get; set; }

        public int? ManufacturerSysNo { get; set; }
        public string AgentLevel { get; set; }

        public int? C1SysNo { get; set; }
        public int? C2SysNo { get; set; }
        public int? C3SysNo { get; set; }

        public DateTime? ExpiredDateFrom { get; set; }
        public DateTime? ExpiredDateTo { get; set; }

        public string Contact { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public VendorStatus? VendorStatus { get; set; }
        public string Rank { get; set; }

        public string SerialNum { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public ECCentral.BizEntity.ExternalSYS.ValidStatus? UserStatus { get; set; }
        public int? RoleSysNo { get; set; }
        public VendorConsignFlag? ConsignType { get; set; }

        public string CompanyCode { get; set; }

        public int? SysNo { get; set; }
    }
}
