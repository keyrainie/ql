using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.MKT
{
    /// <summary>
    /// 留言管理
    /// </summary>
    public class LeaveWordQueryFilter
    {
        public PagingInfo PageInfo { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 主题
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public int? SOSysNo { get; set; }
        
        /// <summary>
        /// 用户编号
        /// </summary>
        public int? CustomerSysNo { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// CompanyCode
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 更新人编号
        /// </summary>
        public int? UpdateUserSysNo { get; set; }

        /// <summary>
        /// 评论开始时间
        /// </summary>
        public DateTime? CreateTimeFrom { get; set; }

        /// <summary>
        /// 评论结束时间
        /// </summary>
        public DateTime? CreateTimeTo { get; set; }

        /// <summary>
        /// 处理开始时间
        /// </summary>
        public DateTime? UpdateTimeFrom { get; set; }

        /// <summary>
        /// 处理结束时间
        /// </summary>
        public DateTime? UpdateTimeTo { get; set; }

        /// <summary>
        /// 是否是有效case
        /// </summary>
        public bool IsValidCase { get; set; }

        /// <summary>
        /// 所属渠道
        /// </summary>
        public int? ChannelID { get; set; }
        /// <summary>
        /// 处理状态
        /// </summary>
        public ECCentral.BizEntity.MKT.CommentProcessStatus? Status { get; set; }

        /// <summary>
        /// 超期时间状态
        /// </summary>
        public ECCentral.BizEntity.MKT.OverTimeStatus? OverTimeStatus { get; set; }
    }
}
