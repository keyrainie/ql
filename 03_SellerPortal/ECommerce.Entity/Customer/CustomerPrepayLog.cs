using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Customer
{
    /// <summary>
    /// 客户余额日志
    /// </summary>
    public class CustomerPrepayLog
    {
        /// <summary>
        /// 余额日志的系统编号
        /// </summary>
        public int? SysNo { get; set; }
        /// <summary>
        /// 用户系统编号
        /// </summary>
        public int? CustomerSysNo { get; set; }
        /// <summary>
        /// 调整类型
        /// </summary>
        public PrepayType? PrepayType { get; set; }
        /// <summary>
        /// 订单系统编号
        /// </summary>
        public int? SOSysNo { get; set; }
        /// <summary>
        /// 调整金额,如果是消费的话值为负数，如果是获得（包括消费返还）值为正数
        /// </summary>
        public decimal? AdjustAmount { get; set; }
        // public string Memo { get; set; }
        /// <summary>
        /// 调整备注
        /// </summary>
        public string Note { get; set; }

    }
}
