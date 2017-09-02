using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.ControlPannel
{
    public class RoleInfo : EntityBase
    {
        public RoleInfo()
        {
            PrivilegeSysNoList = new List<int>();
        }
        public int SysNo { get; set; }

        public string RoleName { get; set; }

        public RoleStatus Status { get; set; }

        public List<int> PrivilegeSysNoList { get; set; }
    }

}
