using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 产品讨论—回复
    /// </summary>
    public class ProductDiscussReply : IIdentity, IWebChannel
    {
        /// <summary>
        /// 产品讨论编号
        /// </summary>
        public int? DiscussSysNo { get; set; }

        /// <summary>
        /// 顾客编号
        /// </summary>
        public int? CustomerSysNo { get; set; }

        /// <summary>
        /// 回复内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 是否添加附加文字    Y=是 N=否
        /// </summary>
        public YNStatus NeedAdditionalText { get; set; }

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
        /// 创建用户
        /// </summary>
        public string InUser { get; set; }

        /// <summary>
        /// 编辑用户
        /// </summary>
        public string EditUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? InDate { get; set; }

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

        /// <summary>
        /// 对应的渠道
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }
    }
}
