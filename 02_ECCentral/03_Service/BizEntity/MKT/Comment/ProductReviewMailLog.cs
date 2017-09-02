using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 产品评论,咨询，讨论-邮件日志
    /// </summary>
    public class ProductReviewMailLog : IIdentity, IWebChannel,ILanguage
    {
        /// <summary>
        /// 产品评论编号
        /// </summary>
        public int? RefSysNo { get; set; }

        /// <summary>
        /// 日志类型 R为评论  C为咨询
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// CS备注
        /// </summary>
        public LanguageContent CSNote { get; set; }

        /// <summary>
        /// 用户咨询提问
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 邮件内容
        /// </summary>
        public LanguageContent TopicMailContent { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 多语言
        /// </summary>
        public string LanguageCode { get; set; }

        /// <summary>
        /// 对应的渠道
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }
    }
}
