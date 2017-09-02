using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 产品评论
    /// </summary>
    public class ProductReview : IIdentity, IWebChannel
    {
        /// <summary>
        /// 产品编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 商品
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 顾客编号
        /// </summary>
        public int? CustomerSysNo { get; set; }

        /// <summary>
        /// 订单编号    提交CS处理需要SONo
        /// </summary>
        public int? SOSysno { get; set; }

        /// <summary>
        /// 评论标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 优点
        /// </summary>
        public string Prons { get; set; }

        /// <summary>
        /// 缺点
        /// </summary>
        public string Cons { get; set; }

        /// <summary>
        /// 服务质量
        /// </summary>
        public string Service { get; set; }

        /// <summary>
        /// 是否上传图片
        /// </summary>
        public YNStatus? Image { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        public YNStatus? IsTop { get; set; }

        /// <summary>
        /// 是否精华
        /// </summary>
        public YNStatus? IsDigest { get; set; }

        /// <summary>
        /// 是否置底
        /// </summary>
        public YNStatus? IsBottom { get; set; }

        /// <summary>
        /// 是否有用候选
        /// </summary>
        public NYNStatus? MostUseFulCandidate { get; set; }

        /// <summary>
        /// 是否有最有用
        /// </summary>
        public NYNStatus? MostUseful { get; set; }

        /// <summary>
        /// 是否首页热评  是否显示在主页
        /// </summary>
        public YNStatus? IsIndexHotReview { get; set; }

        /// <summary>
        /// 是否服务热评  是否显示在主页
        /// </summary>
        public YNStatus? IsServiceHotReview { get; set; }

        /// <summary>
        ///评论状态 
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// CS处理状态
        /// </summary>
        public ReviewProcessStatus? ComplainStatus { get; set; }

        /// <summary>
        /// 用户点击有用次数    用户评价
        /// </summary>
        public int? UsefulCount { get; set; }

        /// <summary>
        /// 用户点击无用次数
        /// </summary>
        public int? UnusefulCount { get; set; }

        /// <summary>
        /// 评论厂商回复列表
        /// </summary>
        public List<ECCentral.BizEntity.MKT.ProductReviewReply> VendorReplyList { get; set; }

        /// <summary>
        /// 产品用户评论--回复
        /// </summary>
        public List<ProductReviewReply> ProductReviewReplyList { get; set; }

        /// <summary>
        /// 产品用户评论-邮件日志
        /// </summary>
        public ProductReviewMailLog ProductReviewMailLog { get; set; }

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

        [DataMember]
        public decimal Score { get; set; }
         [DataMember]
        public int Score1 { get; set; }
         [DataMember]
        public int Score2 { get; set; }
         [DataMember]
        public int Score3 { get; set; }
         [DataMember]
        public int Score4 { get; set; }
    }
}
