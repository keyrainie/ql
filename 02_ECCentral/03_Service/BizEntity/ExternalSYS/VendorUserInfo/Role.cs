using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.ExternalSYS
{
    public class Role: ICompany, IIdentity
    {
        public string RoleName { get; set; }
        
        public string Status { get; set; }
        
        public string InUser { get; set; }
        
        public DateTime? InDate { get; set; }
        
        public string EditUser { get; set; }
       
        public DateTime? EditDate { get; set; }

        public List<int> PrivilegeSysNoList { get; set; }

        public string CompanyCode { get; set; }

        public int? SysNo {get;set;}

        public int? VendorSysNo { get; set; }
    }
}
