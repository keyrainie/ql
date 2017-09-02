using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.Restful.RequestMsg
{
    public class CustomerRightReq
    {
        public CustomerRightReq()
        {
            RightList = new List<CustomerRight>();
        }
        public int CustomerSysNo { get; set; }
        public List<CustomerRight> RightList { get; set; }
    }
}
