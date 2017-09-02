using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Customer;

namespace ECCentral.QueryFilter
{
    public class OrderCheckMasterQueryFilter 
    {
        public PagingInfo PagingInfo { get; set; }
        public int? ChannelSysNo { get; set; }
        public string CompanyCode { get; set; }
    }

    public class OrderCheckItemQueryFilter 
    {
        public PagingInfo PagingInfo { get; set; }
        public int? SysNo { get; set; }
        public string ReferenceType { get; set; }
        public string ReferenceContent { get; set; }
        public string Description { get; set; }
        public OrderCheckStatus? Status { get; set; }
        public string ReferenceTypeIn { get; set; }
        public int? C1SysNo { get; set; }
        public int? C2SysNo { get; set; }
        public int? ChannelSysNo { get; set; }
        public string CompanyCode { get; set; }
    }
}
