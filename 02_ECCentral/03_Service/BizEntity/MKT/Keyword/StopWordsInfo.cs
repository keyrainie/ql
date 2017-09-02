using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 阻止词
    /// </summary>
    public class StopWordsInfo : IIdentity, IWebChannel
    {
        /// <summary>
        /// 阻止词内容
        /// </summary>
        public LanguageContent Keywords { get; set; }

        /// <summary>
        /// 状态      D=无效  A=有效
        /// </summary>
        public ADTStatus Status { get; set; }

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
