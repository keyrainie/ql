using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.PO;
namespace ECCentral.QueryFilter.PO
{
    public class ConsignToAccountLogQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        public int? ProductSysNo { get; set; }
        public string ProductName { get; set; }
        public int? StockSysNo { get; set; }
        public int? VendorSysNo { get; set; }
        public string VendorName { get; set; }
        public ConsignToAccountLogStatus? Status { get; set; }
        public ConsignToAccountType? ConsignToAccType { get; set; }
        public DateTime? CreateTimeFrom { get; set; }
        public DateTime? CreateTimeTo { get; set; }
        public SettleType? SettleType { get; set; }
        public int? AccLogType { get; set; }
        public List<int> SysNoList { get; set; }
        public string CompanyCode { get; set; }

        public ConsignToAccountLogQueryFilter()
        {
            PageInfo = new PagingInfo();
        }

    }
}
