using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Customer;

namespace ECCentral.QueryFilter.Customer
{
    public class RefundRequestQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }
        public DateTime? CreateFrom { get; set; }
        public DateTime? CreateTo { get; set; }
        public DateTime? EditFrom { get; set; }
        public DateTime? EditTo { get; set; }
        public string CustomerId { get; set; }
        public int? SysNo { get; set; }
        public string EditUserName { get; set; }
        public int? SOSysNo { get; set; }
        public RefundRequestType? RequestType { get; set; }
        public string   RefundType { get; set; }
        public RefundRequestStatus? Status { get; set; }
        public string CompanyCode { get; set; }
        public string ChannelID { get; set; }

    }
}
