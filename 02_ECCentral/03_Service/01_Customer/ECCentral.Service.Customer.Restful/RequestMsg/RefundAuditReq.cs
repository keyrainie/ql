using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.Restful.RequestMsg
{
    public class RefundAuditReq
    {
        public List<int> RefundRequestList { get; set; }
        public RefundRequestStatus Status { get; set; }
        public string Memo { get; set; }
    }
}
