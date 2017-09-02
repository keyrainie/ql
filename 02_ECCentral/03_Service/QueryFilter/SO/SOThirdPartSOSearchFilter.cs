using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.SO
{
   public class SOThirdPartSOSearchFilter
    {
       public  PagingInfo  PagingInfo { get; set; }
       public  string    OrderID { get; set;}
       public  string      SOSysNo { get; set; }
       public  string    Type { get; set; }
       public  string    StatusSyncResult { get; set;}
       public  string    CreateResult { get; set; }
       public  string    Memo { get; set;}
       //创建日期(从)
       public DateTime? ShippedOutTimeFrom { get; set; }
       //创建日期(至)
       public DateTime? ShippedOutTimeTo { get; set; }
       public  string    CompanyCode { get; set; }
   }
}
