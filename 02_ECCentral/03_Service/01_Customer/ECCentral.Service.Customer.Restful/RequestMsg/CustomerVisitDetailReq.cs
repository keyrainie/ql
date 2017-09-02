using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.Restful.RequestMsg
{
    public class CustomerVisitDetailReq
    {
        public VisitCustomer VisitInfo { get; set; }
        public List<VisitLog> VisitLogs { get; set; }
        public List<VisitLog> MaintenanceLogs { get; set; }

    }
}
