using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;
using ECommerce.Entity.Store.Vendor;

namespace ECommerce.Entity.ControlPannel
{
    public class UserInfo : EntityBase
    {
        public UserInfo()
        {
            Roles = new List<UsersRoleInfo>();
        }
        public string VendorName { get; set; }
        public string Rank { get; set; }
        public int? VendorSysNo { get; set; }
        public int? UserNum { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Pwd { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Department { get; set; }
        public string Note { get; set; }
        public UserStatus Status { get; set; }
        public string InUser { get; set; }
        public string EditUser { get; set; }
        public int? SysNo { get; set; }
        public string InputPwd { get; set; }
        public VendorStockType VendorStockType { get; set; }

        public List<UsersRoleInfo> Roles { get; set; }
    }

    public class UsersRoleInfo
    {
        public int? UserSysNo { get; set; }
        public int? RoleSysNo { get; set; }
        public int? VendorSysNo { get; set; }
        public string RoleName { get; set; }
        public List<PrivilegeInfo> Privileges { get; set; }
    }

}
