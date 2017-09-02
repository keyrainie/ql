using System.Collections.Generic;
using System;
using ECCentral.WPMessage.BizEntity;

namespace ECCentral.Service.WPMessage.Restful.RequestMsg
{
    public class UpdateWPMessageCategoryRoleReq
    {
        public int CategorySysNo { get; set; }

        public List<int> RoleSysNoList { get; set; }
    }

    public class UpdateWPMessageCategoryRoleByRoleSysNoReq
    {
        public int RoleSysNo { get; set; }

        public List<int> CategorySysNoList { get; set; }
    }

}
