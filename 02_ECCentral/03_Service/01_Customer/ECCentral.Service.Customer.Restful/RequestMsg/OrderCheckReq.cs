using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.Restful.RequestMsg
{
    public class OrderCheckMasterReq
    {
        public OrderCheckMaster orderCheckMaster { get; set; }
        public List<int> SysNoList { get; set; }
    }

    public class BatchCreatOrderCheckItemReq
    {
        public List<OrderCheckItem> orderCheckItemList { get; set; }
        public string ReferenceType { get; set; }
    }
}
