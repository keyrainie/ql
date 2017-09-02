using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;
using ECommerce.Utility;

namespace ECommerce.Entity.ControlPannel
{

    public class UserQueryFilter : QueryFilter
    {
        public int? ManufacturerSysNo { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string SerialNum { get; set; }
        public UserStatus? UserStatus { get; set; }
        public string CompanyCode { get; set; }
    }

    public class RoleQueryFilter : QueryFilter
    {
        public int? ManufacturerSysNo { get; set; }
        public string RoleName { get; set; }
        public RoleStatus? Status { get; set; }
        public string CompanyCode { get; set; }
        public int VendorSysNo { get; set; }
    }
}
