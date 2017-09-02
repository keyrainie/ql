using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Order
{
    public class SOLog : EntityBase
    {
        public int SysNo { get; set; }
        public DateTime OptTime { get; set; }
        public int OptUserSysNo { get; set; }
        public string OptIP { get; set; }
        public int OptType { get; set; }
        public int SOSysNo { get; set; }
        public string Note { get; set; }
        public string OptTimeString
        {
            get
            {
                return OptTime.ToString("yyyy年MM月dd日");
            }
        }
        public string TrackingNumber { set; get; }
    }
}
