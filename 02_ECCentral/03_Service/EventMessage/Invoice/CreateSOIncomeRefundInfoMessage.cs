using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.Invoice
{
    /// <summary>
    /// 创建销售退款单
    /// </summary>
   public class CreateSOIncomeRefundInfoMessage:ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get { return "ECC_SOIncomeRefundInfo_Create"; }
        }

        public int SOIncomeRefundSysNo { get; set; }

      
        public int CurrentUserSysNo { get; set; }
    }
}
