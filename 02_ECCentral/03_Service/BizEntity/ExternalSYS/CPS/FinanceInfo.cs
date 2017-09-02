using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.ExternalSYS
{
   public class FinanceInfo
    {
       /// <summary>
       /// 系统编号
       /// </summary>
       public int SysNo { get; set; }

       /// <summary>
       /// 确认结算金额
       /// </summary>
       public decimal ConfirmCommissionAmt { get; set; }

       /// <summary>
       /// 佣金金额
       /// </summary>
       public decimal CommissionAmt { get; set; }

       public UserInfo User { get; set; }
    }
}
