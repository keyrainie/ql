using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.PO;

namespace ECCentral.QueryFilter.PO
{
    public class SettleQueryFilter
    {

        public SettleQueryFilter()
        {
            PageInfo = new PagingInfo();
        }

        public PagingInfo PageInfo { get; set; }

        public int? VendorSysNo { get; set; }
        public string VendorName { get; set; }

        public int? SettleSysNo { get; set; }

        public POSettleStatus? Status { get; set; }

        public DateTime? CreateTime { get; set; }

        public DateTime? AuditTime { get; set; }

        /// <summary>
        /// 公司编码
        /// </summary>
        public string CompanyCode { get; set; }
    }
}
