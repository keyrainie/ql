using System;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.ExternalSYS
{
    public class UnbilledRuleListQueryFilter
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
        /// 返利类型
        /// </summary>
        public string EIMSType { get; set; }

        /// <summary>
        /// PM编号
        /// </summary>
        public string PMUserSysNo { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode{get;set;}
    }
}
