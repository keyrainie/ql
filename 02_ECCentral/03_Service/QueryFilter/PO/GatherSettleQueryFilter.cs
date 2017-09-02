using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.PO;
namespace ECCentral.QueryFilter.PO
{
    public class GatherSettleQueryFilter
    {
        public GatherSettleQueryFilter()
        {
            PageInfo = new PagingInfo();
        }

        public PagingInfo PageInfo { get; set; }

        public string SettleID { get; set; }
        public GatherSettleStatus? Status { get; set; }
        //付款结算公司
        public PaySettleCompany? PaySettleCompany { get; set; }
        public string CreateUser { get; set; }
        public int? StockSysNo { get; set; }
        public int? VendorSysNo { get; set; }
        public string VendorName { get; set; }
        public DateTime? CreateDateFrom { get; set; }
        public DateTime? CreateDateTo { get; set; }

        public DateTime? AuditDateFrom { get; set; }
        public DateTime? AuditDateTo { get; set; }

        public DateTime? SettledDateFrom { get; set; }
        public DateTime? SettledDateTo { get; set; }
        public string Memo { get; set; }
        public int? PMSysno { get; set; }

        /// <summary>
        /// 是否自动结算
        /// </summary>
        public string IsAutoSettle { get; set; }
        public int? AccLogType { get; set; }
        public string CompanyCode { get; set; }

    }
}
