using System;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.ExternalSYS
{
    public class EIMSComprehensiveQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 供应商编号
        /// </summary>
        public int? VendorSysNo { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// 合同编号
        /// </summary>
        public string RuleNo { get; set; }

        /// <summary>
        /// 单据编号
        /// </summary>
        public string InvoiceNo { get; set; }

        /// <summary>
        /// 周期开始时间
        /// </summary>
        public DateTime? CycleStart { get; set; }

        /// <summary>
        /// 周期结束时间
        /// </summary>
        public DateTime? CycleEnd { get; set; }

        /// <summary>
        /// 合同状态
        /// </summary>
        public string RuleStatus { get; set; }

        /// <summary>
        /// 单据状态
        /// </summary>
        public string InvoiceStatus { get; set; }

        /// <summary>
        /// 返利类型
        /// </summary>
        public string EIMSType { get; set; }

        /// <summary>
        /// PM编号
        /// </summary>
        public int? PMUserSysNo { get; set; }

        /// <summary>
        /// 合同审核开始时间
        /// </summary>
        public DateTime? RuleApprovedStart { get; set; }

        /// <summary>
        /// 合同审核结束时间
        /// </summary>
        public DateTime? RuleApprovedEnd { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }
    }
}
