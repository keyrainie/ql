using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
   public class UnifiedImage:IIdentity
    {
       public int? SysNo { get; set; }

       public string ImageName { get; set;}

       public string ImageUrl { get; set; }

       public DateTime? CreateDate { get; set; }

    }
}
