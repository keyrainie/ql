using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Member
{
    public class PrepayQueryInfoFilter : QueryFilterBase
    {
        public int CustomerID { get; set; }
        public PrepayLogType QueryLogType { get; set; }
        public PrepayQueryTimeType QueryTimeType { get; set; }
        public SortingInfo SortingInfo { get; set; }
    }
}
