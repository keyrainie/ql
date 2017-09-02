using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Member
{
    public class ExperienceInfo
    {
        public decimal Amount { get; set; }

        public DateTime CreateTime { get; set; }

        public int ExperienceType { get; set; }

        public string Memo { get; set; }
    }
}
