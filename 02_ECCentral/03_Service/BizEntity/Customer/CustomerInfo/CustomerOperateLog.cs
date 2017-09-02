using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Customer
{
    /// <summary>
    /// 顾客操作日志
    /// </summary>
    public class CustomerOperateLog
    {
        /// <summary>
        /// 关于哪个客户的操作信息
        /// </summary>
        public int? CustomerSysNo { get; set; }
        /// <summary>
        /// 事件类型
        /// </summary>
        public string EventType { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public int? SOSysNo { get; set; }

    }
}
