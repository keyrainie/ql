using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.QueryFilter.Inventory
{
    public class AdjustRequestQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public int? SysNo { get; set; }

        public string RequestID { get; set; }

        // 有 ProductSysNo 这个参数没有必要用了。
        //public string ProductID { get; set; }

        public int? ProductSysNo { get; set; }        

        public int? StockSysNo { get; set; }

        public AdjustRequestStatus? RequestStatus { get; set; }

        public DateTime? CreateDateFrom { get; set; }

        public DateTime? CreateDateTo { get; set; }                        

        public bool? IsNegative { get; set; }

        public AdjustRequestProperty? AdjustProperty { get; set; }

        public RequestConsignFlag? ConsignFlag { get; set; }

        public string CompanyCode { get; set; }

        public BizEntity.Common.PMQueryType? PMQueryRightType { get; set; }

        public int? UserSysNo { get; set; }
    }
}
