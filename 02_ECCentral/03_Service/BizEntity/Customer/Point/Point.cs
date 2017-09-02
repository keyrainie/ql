using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Customer
{
    /// <summary>
    /// 顾客积分信息
    /// </summary>
    public class Point : IIdentity
    {
        /// <summary>
        /// 积分系统编号
        /// </summary>
        public int? SysNo { get; set; }
        /// <summary>
        /// 客户系统编号
        /// </summary>
        public int? CustomerSysNo { get; set; }
        /// <summary>
        /// 积分数量
        /// </summary>
        public int? PointAmt { get; set; }
        /// <summary>
        /// 可用积分数
        /// </summary>
        public int? AvailablePoint { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime? ExpireDate { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public PointStatus? Status { get; set; }
        /// <summary>
        /// 积分获得日志
        /// </summary>
        public PointObtainLog ObtainLog { get; set; }
        /// <summary>
        /// 该获得明细对应的积分消费日志 
        /// </summary>
        public List<PointConsumeLog> ConsumeLogList { get; set; }
    }
}
