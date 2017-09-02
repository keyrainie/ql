using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Member
{
    public class ExperienceQueryInfoFilter
    {
        public int CustomerID { get; set; }
        public PageInfo PagingInfo { get; set; }
        public SortingInfo SortingInfo { get; set; }
    }
}
