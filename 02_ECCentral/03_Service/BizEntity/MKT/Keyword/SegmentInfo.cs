using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;


namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 中文词库
    /// </summary>
    public class SegmentInfo : IIdentity, IWebChannel
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public LanguageContent Keywords { get; set; }

        /// <summary>
        /// 状态  
        /// </summary>
        public KeywordsStatus? Status { get; set; }

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

        /// <summary>
        /// 对应的创建用户
        /// </summary>
        public String InUser { get; set; }

        /// <summary>
        /// 当前用户
        /// </summary>
        public String CurrentUser { get; set; }
    }
}
