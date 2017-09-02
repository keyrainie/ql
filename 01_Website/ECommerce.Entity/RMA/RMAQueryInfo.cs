using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.RMA
{
    public class RMAQueryInfo
    {
        public string CustomerID { get; set; }

        public PageInfo PagingInfo { get; set; }

        public SortingInfo SortingInfo { get; set; }

        public string ProductName { get; set; }

        public string SOID { get; set; }

        public List<string> SOIDList { get; set; }

        public string RequestID { get; set; }

        public int? CustomerSysNo { get; set; }

        public bool? IsVIP { get; set; }

        public bool? IsSubmit { get; set; }

        public RMARequestStatus? Status { get; set; }

        public int? SysNo { get; set; }
        public int? SOSysNo { get; set; }
    }
}
