using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 产品咨询回复
    /// </summary>
    [Serializable]
    public class ProductMatchedTradingReplyInfo : EntityBase
    {
        /// <summary>
        /// 购物咨询编号
        /// </summary>
        public int? MatchedTradingSysNo { get; set; }

        /// <summary>
        /// 顾客编号
        /// </summary>
        public int? CustomerSysNo { get; set; }

        /// <summary>
        /// 不知道是什么意思
        /// </summary>
        public int? RefSysNo { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 提问主题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 回复内容
        /// </summary>
        public string ReplyContent { get; set; }

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
        public string StatusValue { get; set; }

        ///// <summary>
        ///// 是否添加附加文字    Y=是 N=否    
        ///// </summary>
        //public YNStatus? NeedAdditionalText { get; set; }

        ///// <summary>
        ///// 是否置顶
        ///// </summary>
        //public YNStatus? IsTop { get; set; }

        /// <summary>
        /// 回复人类型
        /// </summary>
        public string ReplyType { get; set; }

        ///// <summary>
        ///// 评论邮件日志
        ///// </summary>
        //public ProductReviewMailLog MailLog { get; set; }

        /// <summary>
        /// 回复人类型
        /// W=网友回复
        ///N=泰隆优选回复
        ///M=厂商回复
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public string InUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? InDate { get; set; }

        /// <summary>
        /// 编辑用户
        /// </summary>
        public string EditUser { get; set; }

        /// <summary>
        /// 编辑时间
        /// </summary>
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        ///// <summary>
        ///// 对应的渠道
        ///// </summary>
        //public Common.WebChannel WebChannel { get; set; }
    }
}
