using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.Invoice.EBank
{
    /// <summary>
    /// 退款返回结果
    /// </summary>
    public class RefundResult
    {
        public RefundResult()
        {
            Result = false;
            Message = string.Empty;
            IsSync = false;
        }

        /// <summary>
        /// 退款流水号
        /// </summary>
        public string ExternalKey { get; set; }

        public bool Result { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// 是否同步退款
        /// </summary>
        public bool IsSync { get; set; }

    }
}
