using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 默认关键字
    /// </summary>
    public class DefaultKeywordsInfo : IIdentity, IWebChannel
    {
        /// <summary>
        /// 默认关键字
        /// </summary>
        public LanguageContent Keywords { get; set; }

        /// <summary>
        /// 状态      D=无效  A=有效
        /// </summary>
        public ADStatus? Status { get; set; }

        /// <summary>
        /// 页面类型    提供选择PageID
        /// </summary>
        public int? PageType { get; set; }

        /// <summary>
        /// 页面ID
        /// </summary>
        public int? PageID { get; set; }

        /// <summary>
        /// 排除的页面ID
        /// </summary>
        public string ExceptPageID { get; set; }
        
        /// <summary>
        /// 扩展生效
        /// </summary>
        public bool? Extend { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? BeginDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 对应的渠道
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }
    }
}
