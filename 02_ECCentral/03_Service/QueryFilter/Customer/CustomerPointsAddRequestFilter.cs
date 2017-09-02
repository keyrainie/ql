using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Customer;

namespace ECCentral.QueryFilter.Customer
{
    public class CustomerPointsAddRequestFilter
    {
        public PagingInfo PageInfo { get; set; }

        public int? SystemNumber { get; set; }
        public DateTime? CreateDateFrom { get; set; }
        public DateTime? CreateDateTo { get; set; }
        public string CustomerID { get; set; }
        public string SOSysNo { get; set; }
        public string OwnByDepartment { get; set; }
        public string  NeweggAccount { get; set; }
        public string CompanyCode { get; set; }
        public string ChannelID { get; set; }
        public CustomerPointsAddRequestStatus? Status { get; set; }

        public string NeweggAccountDesc { get; set; }
        public string OwnByDepartmentDesc { get; set; }
        public string OwnByReasonDesc { get; set; }
    }
}
