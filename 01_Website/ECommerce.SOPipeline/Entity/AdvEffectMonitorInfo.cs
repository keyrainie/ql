using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.SOPipeline
{
    /// <summary>
    /// Advertisement Effect Monitor
    /// </summary>
    public class AdvEffectMonitorInfo
    {
        /// <summary>
        /// 系统自动生成的编号
        /// </summary>
        public int? SysNo { get; set; }
        /// <summary>
        /// CMP = Campaign, 活动类别
        /// </summary>
        public string CMP { get; set; }
        /// <summary>
        /// 操作类型
        /// </summary>
        public string OperationType { get; set; }
        /// <summary>
        /// 生成的单据的顾客编号
        /// </summary>
        public int? CustomerSysNo { get; set; }
        /// <summary>
        /// 生成的单据编号
        /// </summary>
        public int? ReferenceSysNo { get; set; }
        public DateTime? CreateDate { get; set; }
        public string CompanyCode { get; set; }
        public string LanguageCode { get; set; }
        public string StoreCompanyCode { get; set; }
        public string CMP1 { get; set; }
        public string CMP2 { get; set; }
        public string CMP3 { get; set; }
        public string CMP4 { get; set; }
    }
}
