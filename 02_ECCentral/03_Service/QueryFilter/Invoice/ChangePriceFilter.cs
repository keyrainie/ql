using ECCentral.BizEntity.Invoice;
using ECCentral.QueryFilter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.QueryFilter.Invoice
{
    public class ChangePriceFilter
    {
        public PagingInfo PagingInfo { get; set; }

        public string SysNo { get; set; }

        public int? ProductsysNo { get; set; }

        public string ProductName { get; set; }

        public string Memo { get; set; }

        public DateTime? BeginDate { get; set; }

        public DateTime? EndDate { get; set; }

        public RequestPriceStatus? Status { get; set; }

        public int? C1SysNo { get; set; }

        public int? C2SysNo { get; set; }

        public int? C3SysNo { get; set; }
    }
}
