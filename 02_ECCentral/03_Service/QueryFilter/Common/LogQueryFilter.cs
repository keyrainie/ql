using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.QueryFilter.Common
{
   public class LogQueryFilter
    {
        public int? TicketSysNo { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool CancelOutStore { get; set; }
        public bool ISSOLog { get; set; }

        public PagingInfo PagingInfo { get; set; }
    }
}
