using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.RMA
{
    /// <summary>
    /// RMA催讨日志
    /// </summary>
    public class RegisterDunLog
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 单件编号
        /// </summary>
        public int? RegisterSysNo { get; set; }

        /// <summary>
        /// 反馈类型
        /// </summary>
        public string Feedback { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 反馈人系统编号
        /// </summary>
        public int? UserSysNo { get; set; }

        /// <summary>
        /// 反馈人姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 反馈时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
    }
}
