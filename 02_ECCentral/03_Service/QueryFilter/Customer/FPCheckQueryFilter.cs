using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Customer;

namespace ECCentral.QueryFilter.Customer
{
    /// <summary>
    /// FP查询过滤参数
    /// </summary>
    public class FPCheckQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }
        public string ChannelID { get; set; }
        public string CompanyCode { get; set; }

    }
    /// <summary>
    /// FP审核中串货订单查询条件
    /// </summary>
    public class CHQueryFilter
    {
        public string ProductID { get; set; }
        public int? CategorySysNo { get; set; }
        public FPCheckItemStatus? Status { get; set; }
        public string ChannelID { get; set; }
        public bool IsSearchCategory { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
