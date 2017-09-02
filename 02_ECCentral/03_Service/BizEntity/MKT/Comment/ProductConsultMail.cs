using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 产品咨询-邮件日志 暂时没用到
    /// </summary>
    public class ProductConsultMail : IIdentity, IWebChannel
    {
        /// <summary>
        /// 产品咨询编号
        /// </summary>
        public int? ConsultSysNo { get; set; }

        /// <summary>
        /// 邮件内容
        /// </summary>
        public string MailContent { get; set; }

        /// <summary>
        /// 对应的编号
        /// </summary>
        public int? RefSysNo { get; set; }

        /// <summary>
        /// CS备注
        /// </summary>
        public string CSNote { get; set; }

        /// <summary>
        /// 主题
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 回复内容
        /// </summary>
        public string TopicMailContent { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        public int? CustomerSysNo { get; set; }

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