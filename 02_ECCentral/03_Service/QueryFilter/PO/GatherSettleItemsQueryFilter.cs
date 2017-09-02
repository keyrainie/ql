using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.PO;

namespace ECCentral.QueryFilter.PO
{
    public class GatherSettleItemsQueryFilter
    {
        public GatherSettleItemsQueryFilter()
        {
            PageInfo = new PagingInfo();
        }

        public PagingInfo PageInfo { get; set; }

        public int? SettleID { get; set; }

        public int? VendorSysNo { get; set; }

        public int? StockSysNo { get; set; }

        public decimal? TotalAmt { get; set; }

        public SettleStatus? Status { get; set; }
        //付款结算公司
        public PaySettleCompany? PaySettleCompany { get; set; }
        public DateTime? CreateDate { get; set; }

        public string ReferenceSysNo { get; set; }

        public DateTime? CreateDateFrom { get; set; }

        public DateTime? CreateDateTo { get; set; }

        public DateTime? OutStockRefundDateFrom { get; set; }

        public DateTime? OutStockRefundDateTo { get; set; }

        public string CompanyCode { get; set; }
    }
}
