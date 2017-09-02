using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.QueryFilter.MKT
{
    /// <summary>
    /// 新闻与公告查询
    /// </summary>
    public class NewsInfoQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 新闻类型
        /// </summary>
        public int? NewsType { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 副标题
        /// </summary>
        public string Subtitle { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string InUser { get; set; }

        /// <summary>
        /// 创建时间开始于
        /// </summary>
        public DateTime? InDateFrom { get; set; }

        /// <summary>
        /// 创建时间结束于
        /// </summary>
        public DateTime? InDateFromTo { get; set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        public bool? IsSetTop { get; set; }

        /// <summary>
        /// 是否飘红
        /// </summary>
        public bool? IsRed { get; set; }

        /// <summary>
        /// 是否展示评论
        /// </summary>
        public int? IsShow { get; set; }

        /// <summary>
        /// 主要投放区域
        /// </summary>
        public string SelectedArea { get; set; }

        /// <summary>
        /// 显示状态
        /// </summary>
        public NewsStatus? Status { get; set; }

        public string CompanyCode { get; set; }
        /// <summary>
        /// 所属渠道
        /// </summary>
        public string ChannelID { get; set; }

        private int? _ReferenceSysNo;

        /// <summary>
        /// 大区
        /// </summary>
        public int? ReferenceSysNo
        {
            get;
            set;
        }
    }
}
