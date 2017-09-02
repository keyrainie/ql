using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.RMA
{
    public class SellerPortalAutoRMALog
    {       
        public int? SysNo { get; set; }
      
        public int? SOSysNo { get; set; }
       
        public string RequestStatus { get; set; }
      
        public DateTime? RequestTime { get; set; }
       
        public string RefundStatus { get; set; }
       
        public DateTime? RefundTime { get; set; }
      
        public string InUser { get; set; }
     
        public DateTime? InDate { get; set; }
    }
}