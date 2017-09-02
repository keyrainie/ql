using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.ControlPannel
{
    public class PrivilegeInfo : EntityBase
    {
        public int SysNo { get; set; }

        public int? ParentSysNo { get; set; }

        public string PrivilegeName { get; set; }

        public int? OrderNo { get; set; }

        public string Memo { get; set; }

        public List<PrivilegeInfo> Children { get; set; }
    }
}
