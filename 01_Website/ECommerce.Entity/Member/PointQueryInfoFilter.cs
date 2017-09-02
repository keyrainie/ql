using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Member
{
    public class PointQueryInfoFilter
    {
        public int CustomerID { get; set; }
        public DateTime EndTime { get; set; }
        public PageInfo PagingInfo { get; set; }
        public List<int> PointTypeIntList { get; set; }
        public List<PointType> PointTypeList { get; set; }
        public SortingInfo SortingInfo { get; set; }
        public DateTime StartTime { get; set; }
    }
}
