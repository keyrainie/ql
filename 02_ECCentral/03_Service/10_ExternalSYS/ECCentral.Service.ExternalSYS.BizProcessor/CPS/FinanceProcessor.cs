using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Service.ExternalSYS.BizProcessor
{
    [VersionExport(typeof(FinanceProcessor))]
    public class FinanceProcessor
    {
        private IFinanceDA financeDA = ObjectFactory<IFinanceDA>.Instance;
        /// <summary>
        /// 更新确认结算金额
        /// </summary>
        /// <param name="info"></param>
        public void UpdateCommisonConfirmAmt(FinanceInfo info)
        {
            financeDA.UpdateCommisonConfirmAmt(info);
        }
    }
}
