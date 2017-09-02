using System.Collections.Generic;

namespace IPPOversea.Invoicemgmt.AutoSettledCommission.Biz
{
    /// <summary>
    /// 验证结果
    /// </summary>
    public class ValidateResult
    {
        public ValidateResult()
        {
            this.MessageDetailsList = new List<string>();
        }
        /// <summary>
        /// 是否通过验证
        /// </summary>
        public bool IsPass { get; set; }

        /// <summary>
        /// 验证消息
        /// </summary>
        public string ValidateMessage { get; set; }

        /// <summary>
        /// 消息明细
        /// </summary>
        public List<string> MessageDetailsList { get; set; }
    }
}
