using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    ///产品讨论
    /// </summary>
    public class ProductDiscussDetail : IIdentity, IWebChannel
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
        /// 回复次数
        /// </summary>
        public int? ReplyCount { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 查看次数
        /// </summary>
        public int? ViewCount { get; set; }

        /// <summary>
        /// 讨论回复
        /// </summary>
        public List<ProductDiscussReply> ProductDiscussReplyList { get; set; }

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
