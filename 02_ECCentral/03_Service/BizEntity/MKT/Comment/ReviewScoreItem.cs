using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 评分项定义
    /// </summary>
    public class ReviewScoreItem : IIdentity, IWebChannel
    {
        /// <summary>
        /// 所属渠道
        /// </summary>
        public int? ChannelID { get; set; }

        /// <summary>
        /// 一级类别编号
        /// </summary>
        public int? C1SysNo { get; set; }

        /// <summary>
        /// 二级类别编号
        /// </summary>
        public int? C2SysNo { get; set; }

        /// <summary>
        /// 三级类别编号
        /// </summary>
        public int? C3SysNo { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public LanguageContent Name { get; set; }

        /// <summary>
        /// 状态    A：有效 D:无效
        /// </summary>
        public ADStatus? Status { get; set; }

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
