using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.Customer
{
    public class EmailQueryFilter
    {
        public string ChannelID { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public EmailSendStatus? Status { get; set; }
        public string Source { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
