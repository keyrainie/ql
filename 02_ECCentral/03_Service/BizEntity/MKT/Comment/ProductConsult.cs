using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    ///咨询管理
    /// </summary>
    public class ProductConsult : IIdentity, IWebChannel
    {
        /// <summary>
        /// 问题内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 回复数量
        /// </summary>
        public int? ReplyCount { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public int? CustomerSysNo { get; set; }

        /// <summary>
        /// 商品
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 处理人员
        /// </summary>
        public string EditUser { get; set; }

        /// <summary>
        /// 提问的人
        /// </summary>
        public string InUser { get; set; }

        /// <summary>
        /// 邮件地址
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 回复附加语
        /// </summary>
        public YNStatus? NeedAdditionalText { get; set; }

        /// <summary>
        /// 标记类型  D=商品咨询  S=库存配送
        /// </summary>
        public string Type { get; set; }

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
        /// 创建时间
        /// </summary>
        public DateTime? InDate { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 评论厂商回复列表
        /// </summary>
        public List<ECCentral.BizEntity.MKT.ProductConsultReply> VendorReplyList { get; set; }

        /// <summary>
        /// 产品用户评论-邮件日志
        /// </summary>
        public ProductReviewMailLog ProductReviewMailLog { get; set; }

        /// <summary>
        /// 回复
        /// </summary>
        public List<ProductConsultReply> ProductConsultReplyList { get; set; }

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