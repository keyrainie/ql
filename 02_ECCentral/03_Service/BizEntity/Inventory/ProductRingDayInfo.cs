using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Inventory
{
    public class ProductRingDayInfo
    {
        public int? SysNo { get; set; }
        public int? BrandSysNo { get; set; }
        public int? C3SysNo { get; set; }
        public int? RingDay { get; set; }
        public string Email { get; set; }
        public int? InUser { get; set; }
        public DateTime? InDate { get; set; }
        public int? EditUser { get; set; }
        public DateTime? EditDate { get; set; }
    }
}
