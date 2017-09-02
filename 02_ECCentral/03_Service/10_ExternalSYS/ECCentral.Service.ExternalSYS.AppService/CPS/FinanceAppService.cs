using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.ExternalSYS.BizProcessor;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Service.ExternalSYS.AppService
{
     [VersionExport(typeof(FinanceAppService))]
   public class FinanceAppService
    {
         private FinanceProcessor processor = ObjectFactory<FinanceProcessor>.Instance;
           /// <summary>
        /// 更新确认结算金额
        /// </summary>
        /// <param name="info"></param>
         public void UpdateCommisonConfirmAmt(FinanceInfo info)
         {
             processor.UpdateCommisonConfirmAmt(info);
         }
    }
}
