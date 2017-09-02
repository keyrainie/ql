using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Member
{
    public class PointConsumeInfo
    {
        public PointType ConsumeType { get; set; }

        public DateTime CreateDate { get; set; }

        public int CustomerSysNo { get; set; }

        public string Memo { get; set; }

        public int Point { get; set; }

        public int SOSysNo { get; set; }

        public int SysNo { get; set; }
    }
}
