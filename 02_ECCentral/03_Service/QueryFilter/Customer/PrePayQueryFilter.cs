using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Customer;

namespace ECCentral.QueryFilter.Customer
{
    public class PrePayQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }
        public DateTime? CreateDateFrom { get; set; }
        public DateTime? CreateDateTo { get; set; }
        public int? OrderSysNo { get; set; }
        public int? PrePayType { get; set; }
        public int? CustomerSysNo { get; set; }
        public PrepayStatus? Status { get; set; }
        public string CompanyCode { get; set; }
        public string ChannelID { get; set; }
    }
}
