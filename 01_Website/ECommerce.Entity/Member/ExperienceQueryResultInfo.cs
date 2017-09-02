using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Member
{
    public class ExperienceQueryResultInfo
    {
        public QueryResult<ExperienceInfo> LogList { get; set; }

        public decimal TotalExperience { get; set; }
    }
}
