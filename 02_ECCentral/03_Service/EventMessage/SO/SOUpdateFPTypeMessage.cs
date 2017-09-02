using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;

namespace ECCentral.Service.EventMessage.SO
{
    /// <summary>
    /// 修改订单欺诈类型后发送Message
    /// </summary>
    public class SOUpdateFPTypeMessage : ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get
            {
                return "ECC_SO_FPChecked";
            }
        }
        /// <summary>
        /// 订单系统编号
        /// </summary>
        public int SOSysNo { get; set; }
        /// <summary>
        /// 商家系统编号
        /// </summary>
        public int MerchantSysNo { get; set; }
        /// <summary>
        /// 订单欺诈类型
        /// </summary>
        public SOFPType FPType { get; set; }
        /// <summary>
        /// 订单设置为此欺诈类型的原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 欺诈检查用户编号
        /// </summary>
        public int FPCheckUserSysNo { get; set; }

        public string FPCheckUserName { get; set; }

        /// <summary>
        /// 获取或设置订单拆分类型
        /// </summary>
        public SOSplitType SplitType { get; set; }
    }
}
