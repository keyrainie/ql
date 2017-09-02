using ECommerce.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity;

namespace ECommerce.Entity.Order
{
    public class SOQueryInfo
    {
        public SOSearchType SearchType { get; set; }

        public int CustomerID { get; set; }

        public PageInfo PagingInfo { get; set; }

        public SortingInfo SortingInfo { get; set; }

        public string ProductName { get; set; }

        public string SOID { get; set; }

        public DateTime OrderDate { get; set; }

        public SOStatus? Status { get; set; }

        public SOType SOType { get; set; }

        public SOPaymentStatus SOPaymentStatus { get; set; }

        public string SearchKey { get; set; }
        
    }
}
