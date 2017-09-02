using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
namespace ECCentral.QueryFilter.PO
{
    public class PurchaseOrderBasketQueryFilter
    {

        public PurchaseOrderBasketQueryFilter()
        {
            PageInfo = new PagingInfo();
        }
        public PagingInfo PageInfo { get; set; }

        public int? SysNo { get; set; }
        public string UserName { get; set; }
        public string PMGroupName { get; set; }
        public int? Status { get; set; }
        public int? PMUserSysNo { get; set; }
        public int? StockSysNo { get; set; }
        public string CreateUserSysNo { get; set; }
        public string CompanyCode { get; set; }
        public bool? IsManagerPM { get; set; }
    }
}
