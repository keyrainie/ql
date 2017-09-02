using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 用户留言表
    /// </summary>
    public class LeaveWordsItem : IIdentity, IWebChannel
    {
        /// <summary>
        /// 主题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public int? SOSysNo { get; set; }

        /// <summary>
        /// 更新用户编号
        /// </summary>
        public int? UpdateUserSysNo { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        public int? CustomerSysNo { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 用户Email
        /// </summary>
        public string CustomerEmail { get; set; }

        /// <summary>
        /// 回复内容[邮件]
        /// </summary>
        public string ReplyContent { get; set; }

        /// <summary>
        /// CS备注
        /// </summary>
        public string CSNote { get; set; }

        /// <summary>
        /// 用户留言内容 
        /// </summary>
        public string LeaveWords { get; set; }

        /// <summary>
        /// 客服邮件回复内容
        /// </summary>
        public string MailReplyContent { get; set; }

        /// <summary>
        /// 评论时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 处理状态
        /// </summary>
        public ECCentral.BizEntity.MKT.CommentProcessStatus? Status { get; set; }

        /// <summary>
        /// 超期时间状态
        /// </summary>
        public ECCentral.BizEntity.MKT.OverTimeStatus? OverTimeStatus { get; set; }

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
