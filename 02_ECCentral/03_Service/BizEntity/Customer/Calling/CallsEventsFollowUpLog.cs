using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.Customer
{
    /// <summary>
    /// 来电跟进日志
    /// </summary>
    public class CallsEventsFollowUpLog : IIdentity
    {
        /// <summary>
        /// 跟进日志系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 来电事件编号
        /// </summary>
        public int? CallsEventsSysNo { get; set; }

        /// <summary>
        /// 处理状态
        /// </summary>
        public CallsEventsStatus? Status { get; set; }

        /// <summary>
        /// 情况描述
        /// </summary>
        public string Question { get; set; }

        /// <summary>
        /// 记录类型
        /// </summary>
        public CustomerCallReason? CallReason { get; set; }

        /// <summary>
        /// 记录来源
        /// </summary>
        public string RecordOrigion { get; set; }

        /// <summary>
        /// 原由ReasonCode系统编号
        /// </summary>
        public int? ReasonCodeSysNo { get; set; }
 
    }
}
