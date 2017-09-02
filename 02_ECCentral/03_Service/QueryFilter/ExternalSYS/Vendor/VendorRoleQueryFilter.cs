using ECCentral.QueryFilter.Common;
using System;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.QueryFilter.ExternalSYS
{
    public class VendorRoleQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public int? RoleSysNo { get; set; }

        public string RoleName { get; set; }
        
        public ValidStatus? Status { get; set; }

        public int? PrivilegeSysNo { get; set; }

        public string PrivilegeName { get; set; }
    }
}
