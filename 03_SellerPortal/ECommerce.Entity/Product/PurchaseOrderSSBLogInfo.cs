using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Product
{

    /// <summary>
    /// 采购单SSB日志信息
    /// </summary>
    public class PurchaseOrderSSBLogInfo
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 采购单系统编号
        /// </summary>
        public int? POSysNo { get; set; }

        /// <summary>
        /// 日志内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public string ActionType { get; set; }

        /// <summary>
        /// 创建人系统编号
        /// </summary>
        public int? InUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? Indate { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public string ErrMSg { get; set; }

        /// <summary>
        /// 异常信息时间
        /// </summary>
        public DateTime? ErrMSgTime { get; set; }

        /// <summary>
        /// 发送异常邮件
        /// </summary>
        public string SendErrMail { get; set; }

    }
}
