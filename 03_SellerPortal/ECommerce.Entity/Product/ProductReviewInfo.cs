using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 商品评论
    /// </summary>
    [Serializable]
    public class ProductReviewInfo : EntityBase
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 商品
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 商品ID
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
        public string Image { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        public string IsTop { get; set; }

        /// <summary>
        /// 是否精华
        /// </summary>
        public string IsDigest { get; set; }

        /// <summary>
        /// 是否置底
        /// </summary>
        public string IsBottom { get; set; }

        /// <summary>
        /// 是否有用候选
        /// </summary>
        public string MostUseFulCandidate { get; set; }

        /// <summary>
        /// 是否有最有用
        /// </summary>
        public string MostUseful { get; set; }

        /// <summary>
        /// 是否首页热评  是否显示在主页
        /// </summary>
        public string IsIndexHotReview { get; set; }

        /// <summary>
        /// 是否服务热评  是否显示在主页
        /// </summary>
        public string IsServiceHotReview { get; set; }

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
        /// 产品评论厂商回复回复列表
        /// </summary>
        public List<ProductReviewReplyInfo> VendorReplyList { get; set; }

        /// <summary>
        /// 产品用户评论回复列表
        /// </summary>
        public List<ProductReviewReplyInfo> ProductReviewReplyList { get; set; }

        ///// <summary>
        ///// 产品用户评论-邮件日志
        ///// </summary>
        //public ProductReviewMailLog ProductReviewMailLog { get; set; }

        public decimal Score { get; set; }
        public int Score1 { get; set; }
        public int Score2 { get; set; }
        public int Score3 { get; set; }
        public int Score4 { get; set; }

        public List<string> ImageList { get; set; }
    }
}
