using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.QueryFilter.Common
{
    public class ControlPanelUserQueryFilter
    {
        public string LoginName { get; set; }
        
        public string DisplayName { get; set; }
        
        public string DepartmentCode { get; set; }

        public int? OrganizationID { get; set; }

        public ControlPanelUserStatus? Status { get; set; }

        public PagingInfo PagingInfo { get; set; }
    }
}
