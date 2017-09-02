using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 商品评论回复
    /// </summary>
    [Serializable]
    public class ProductReviewReplyInfo : EntityBase
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 商品评论编号
        /// </summary>
        public int? ReviewSysNo { get; set; }

        /// <summary>
        /// 顾客编号    0-泰隆优选回复
        /// </summary>
        public int? CustomerSysNo { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 回复内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// O=未处理
        ///E=已阅读
        ///R=已回复
        ///A_1=审核通过系统
        ///A_2=审核通过人工已发布
        ///D_1=审核不通过系统
        ///D_2=审核不通过人工
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the status value.
        /// </summary>
        /// <value>
        /// The status value.
        /// </value>
        public string StatusValue { get; set; }

        /// <summary>
        /// 类型  W=网友回复    N=泰隆优选回复  M=厂商回复
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 是否添加附加文字    Y=是 N=否
        /// </summary>
        public string NeedAdditionalText { get; set; }

        ///// <summary>
        ///// 评论日志
        ///// </summary>
        //public ProductReviewMailLog MailLog { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public string InUser { get; set; }

        /// <summary>
        /// 编辑用户
        /// </summary>
        public string EditUser { get; set; }

    }
}
