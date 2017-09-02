using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.ExternalSYS
{
    public class PrivilegeEntity
    {
        public int SysNo { get; set; }
      
        public int? ParentSysNo { get; set; }
      
        public string PrivilegeName { get; set; }
       
        public int? OrderNo { get; set; }
       
        public string Memo { get; set; }

    }
}
