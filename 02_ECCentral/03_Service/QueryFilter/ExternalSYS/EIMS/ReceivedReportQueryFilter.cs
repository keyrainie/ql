using System;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.ExternalSYS
{
    public class ReceivedReportQueryFilter
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
        /// 年度
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// 费用类型
        /// </summary>
        public string EIMSType { get; set; }

        /// <summary>
        /// PM编号
        /// </summary>
        public int? PMUserSysNo { get; set; }

        /// <summary>
        /// 过期天数
        /// </summary>
        public int? ExpiredDays { get; set; }

        /// <summary>
        /// 产品一级类别
        /// </summary>
        public int? ProductCategory1 { get; set; }

        /// <summary>
        /// 产品二级类别
        /// </summary>
        public int? ProductCategory2 { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }
    }
}
