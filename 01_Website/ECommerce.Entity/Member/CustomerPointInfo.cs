using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Member
{
    public class CustomerPointInfo
    {
        public DateTime CreateDate { get; set; }

        public string Memo { get; set; }

        public int PointAmount { get; set; }

        public PointType PointLogType { get; set; }

        public int PointSysno { get; set; }

        public decimal SOAmount { get; set; }

        public int SOSysNo { get; set; }

        public int SubTotal { get; set; }
    }
}
