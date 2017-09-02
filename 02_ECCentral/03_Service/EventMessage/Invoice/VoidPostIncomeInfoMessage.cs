using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.EventMessage.Invoice
{
    /// <summary>
    /// 作废电汇邮局收款单
    /// </summary>
   public class VoidPostIncomeInfoMessage:ECCentral.Service.Utility.EventMessage
    {
        public override string Subject
        {
            get { return "ECC_PostIncomeInfo_Void"; }
        }

        /// <summary>
        /// 电汇邮局收款单编号
        /// </summary>
        public int PostIncomeInfoSysNo { get; set; }


        public int CurrentUserSysNo { get; set; }
    }
}
