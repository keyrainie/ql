using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Payment
{
    public class TLPoint
    {
        public int SysNo { get; set; }
        public int SoSysNO { get; set; }
        public int Point { get; set; }
        public int Type { get; set; }
        public DateTime InDate { get; set; }
        public string InUser { get; set; }
        public DateTime LastEditDate { get; set; }
        public string LastEditUser { get; set; }
    }
}
