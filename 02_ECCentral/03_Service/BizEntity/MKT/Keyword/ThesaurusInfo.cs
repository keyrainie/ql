using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 同义词
    /// </summary>
    public class ThesaurusInfo : IIdentity, IWebChannel
    {
        /// <summary>
        /// 同义词类型   单向词 双向词
        /// </summary>
        public ThesaurusWordsType? Type { get; set; }

        /// <summary>
        /// 同义词内容
        /// </summary>
        public LanguageContent ThesaurusWords { get; set; }

        /// <summary>
        /// 状态      D=无效  A=有效
        /// </summary>
        public ADTStatus? Status { get; set; }

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
