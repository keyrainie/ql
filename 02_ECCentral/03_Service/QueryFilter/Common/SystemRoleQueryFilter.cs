using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.QueryFilter.Common
{
    public class SystemRoleQueryFilter
    {
        public PagingInfo PagingInfo
        {
            get;
            set;
        }

        public string ApplicationID
        {
            get;
            set;
        }
        public string RoleName
        {
            get;
            set;
        }

        public AuthCenterStatus? RoleStatus
        { get; set; }
    }
}
