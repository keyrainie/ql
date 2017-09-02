using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Common
{
    /// <summary>
    /// 主要用于Job调用Email服务时的实体映射到MailMessage
    /// </summary>
    public class MailInfo
    {
        /// <summary>
        /// 是否是Html类型的邮件
        /// </summary>
        public bool IsHtmlType { get; set; }

        /// <summary>
        /// 邮件的重要性 （0=普通，1=一般，2=重要)
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 发件人邮件地址
        /// </summary>
        public string FromName { get; set; }

        /// <summary>
        /// 发件人名称
        /// </summary>
        public string DisplaySenderName { get; set; }

        /// <summary>
        /// 收件人地址
        /// </summary>
        public string ToName { get; set; }

        /// <summary>
        /// 抄送人地址
        /// </summary>
        public string CCName { get; set; }

        /// <summary>
        /// 按送人地址
        /// </summary>
        public string BCCName { get; set; }

        /// <summary>
        /// 回复名称
        /// </summary>
        public string ReplyName { get; set; }

        /// <summary>
        /// 邮件主题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 邮件内容
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 附件
        /// </summary>
        public List<string> Attachments { get; set; }

        /// <summary>
        /// 自增长序号
        /// </summary>
        public int SysNo { get; set; }


        #region 仅做标识用，不保存数据库

        /// <summary>
        /// 是否异步发送
        /// </summary>
        public bool IsAsync { get; set; }

        /// <summary>
        /// 是否为内部邮件
        /// </summary>
        public bool IsInternal { get; set; }

        #endregion

        public MailInfo()
        {
            IsAsync = true;
            IsInternal = true;
        }
    }
}
