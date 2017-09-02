using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.GiftCard
{
    public class MyGiftCardQueryInfoFilter
    {
        public int CustomerID { get; set; }
        public string Code { get; set; }
        public DateTime EndTime { get; set; }
        public PageInfo PagingInfo { get; set; }
        public SortingInfo SortingInfo { get; set; }
        public DateTime StartTime { get; set; }
    }
}
