using System;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.ExternalSYS
{
    public class EIMSInvoiceQueryFilter
    {
        public PagingInfo PagingInfo{get;set;}

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
        /// 返利类型
        /// </summary>
        public string EIMSType { get; set; }

        /// <summary>
        /// 收款类型
        /// </summary>
        public string ReceivedType { get; set; }

        /// <summary>
        /// 单据审核开始时间
        /// </summary>
        public DateTime? InvoiceApprovedStart { get; set; }

        /// <summary>
        /// 单据审核结束时间
        /// </summary>
        public DateTime? InvocieApprovedEnd { get; set; }

        /// <summary>
        /// 单据状态
        /// </summary>
        public string InvoiceStatus { get; set; }

        /// <summary>
        /// 按一级分类汇总
        /// </summary>
        public bool? IsC1Summary { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }
    }
}
