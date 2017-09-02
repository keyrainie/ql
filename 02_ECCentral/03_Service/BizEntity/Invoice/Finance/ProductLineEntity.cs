using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Invoice
{
    public class ProductLineEntity
    {        
        public int ProductLineSysNo { get; set; }
       
        public int C2SysNo { get; set; }
       
        public int? BrandSysNo { get; set; }

        public int PMUserSysNo { get; set; }
       
        public int MerchandiserSysNo { get; set; }
       
        public string BackupPMSysNoList { get; set; }
    }
}
