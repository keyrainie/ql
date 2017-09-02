using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.EventMessage.SO;

namespace ECCentral.Service.EventMessage.Invoice
{
    /// <summary>
    /// 订单的Netpay审核完成发送Message
    /// </summary>
    [Serializable]
    public class InvoiceNetpayAuditedMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_Netpay_Audited";
            }
        }
        /// <summary>
        /// 订单系统编号
        /// </summary>
        public int SoSysNo { get; set; }
        /// <summary>
        /// 商家系统编号
        /// </summary>
        public int MerchantSysNo { get; set; }
        /// <summary>
        /// 订单类型
        /// </summary>
        public int SOType { get; set; }

        public int ReferenceSysNo { get; set; }

        public int AuditUserSysNo { get; set; }
        public string AuditUserName { get; set; }
        public int NetpaySysNo { get; set; }

        /// <summary>
        /// 获取或设置订单拆分类型
        /// </summary>
        public int SplitType { get; set; }
    }
}
