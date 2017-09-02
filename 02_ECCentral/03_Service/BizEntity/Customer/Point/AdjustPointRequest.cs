using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Customer
{
    /// <summary>
    /// 积分调整申请
    /// </summary>
    public class AdjustPointRequest : IIdentity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }
        /// <summary>
        /// 客户编号
        /// </summary>
        public int? CustomerSysNo { get; set; }

        /// <summary>
        /// 相关单据编号
        /// </summary>
        public int? SOSysNo { get; set; }
        /// <summary>
        /// 积分数（正、负）
        /// </summary>
        public int? Point { get; set; }

        /// <summary>
        /// 原因
        /// </summary>
        public string Memo { get; set; }
        /// <summary>
        /// 调整积分原因类型
        /// </summary>
        public int? PointType { get; set; }
        /// <summary>
        /// 积分过期时间,可以不用设置，如果没有设置系统将自动计算。
        /// </summary>
        public DateTime? PointExpiringDate { get; set; }
        /// <summary>
        /// OperationType与OrderSysNo配合使用，
        /// 当OperationType=1，积分撤销，OperationType=0，添加积分，扣减积分操作；
        /// </summary>
        public AdjustPointOperationType? OperationType { get; set; }

        /// <summary>
        /// 调用方名称，一般写Domain名称
        /// </summary>
        public string Source { get; set; }


    }
}
