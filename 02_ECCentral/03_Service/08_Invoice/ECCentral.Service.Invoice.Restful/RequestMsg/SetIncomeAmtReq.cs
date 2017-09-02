using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Service.Invoice.Restful.RequestMsg
{
    /// <summary>
    /// 设置收款单实收金额Request
    /// </summary>
    public class SetIncomeAmtReq
    {
        /// <summary>
        /// 收款单系统编号
        /// </summary>
        public int SysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal IncomeAmt
        {
            get;
            set;
        }
    }
}