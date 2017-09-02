using System;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.ExternalSYS
{
    public class EIMSEventMemoQueryFilter
    {
        public PagingInfo PagingInfo{get;set;}

        /// <summary>
        /// 供应商编号
        /// </summary>
        public int? VendorSysNo { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? BeginDate { get; set; }
        
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }
        
        /// <summary>
        /// 结算类型更改说明
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }
    }
}
