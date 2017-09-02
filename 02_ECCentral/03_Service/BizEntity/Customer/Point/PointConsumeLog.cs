using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Customer
{
    /// <summary>
    /// 积分消费记录
    /// </summary>
    public class PointConsumeLog:IIdentity
    {
        /// <summary>
        /// 消费记录的系统编号
        /// </summary>
        public int ? SysNo { get; set; }

       /// <summary>
       /// 顾客系统编号
       /// </summary>
        public int ? CustomerSysNo { get; set; }

        /// <summary>
        /// 关联的获得记录编号
        /// </summary>
        public int ? RelatedObtainNo { get; set; }

        /// <summary>
        /// 积分数量
        /// </summary>
        public int ? Point { get; set; }

        /// <summary>
        /// 订单系统编号
        /// </summary>
        public int ? SOSysNo { get; set; }
        /// <summary>
        /// 消费类型
        /// </summary>
        public int ? ConsumeType { get; set; }

        /// <summary>
        /// 消费记录
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public int ? IsShow { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public PointStatus? Status { get; set; }
    }
}
