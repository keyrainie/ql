using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 公告及促销评论表
    /// </summary>
    public class NewsAdvReply : IIdentity, IWebChannel
    {
        /// <summary>
        /// 编号 
        /// </summary>
        public int SystemNumber { get; set; }

        /// <summary>
        /// 顾客ID
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// 顾客SysNo
        /// </summary>
        public int? CustomerSysNo { get; set; }

        /// <summary>
        /// 前台回复内容
        /// </summary>
        public string ReplyContent { get; set; }

        /// <summary>
        /// 评论类型
        /// </summary>
        public string ReferenceType { get; set; }

        /// <summary>
        /// 对应的编号
        /// </summary>
        public int ReferenceSysNo { get; set; }

        /// <summary>
        /// 前台展示状态
        /// </summary>
        public NewsAdvReplyStatus Status { get; set; }

        /// <summary>
        /// 顾客IP地址
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// 相关处理人员
        /// </summary>
        public int? LastEditUserSysNo { get; set; }

        /// <summary>
        /// 后台回复内容
        /// </summary>
        public string AnswerContent { get; set; }

        /// <summary>
        /// 回复是否有回复
        /// </summary>
        public YNStatus ReplyHasReplied { get; set; }

        /// <summary>
        /// 图片地址集合
        /// </summary>
        public string[] ImageUrl { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

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
