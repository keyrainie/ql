using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Service.ExternalSYS.BizProcessor
{
    [VersionExport(typeof(CommissionToCashProcessor))]
    public class CommissionToCashProcessor
    {
        private ICommissionToCashDA commissionToCashDA = ObjectFactory<ICommissionToCashDA>.Instance;

        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="info"></param>
        public void AuditCommisonToCash(CommissionToCashInfo info)
        {
            commissionToCashDA.AuditCommisonToCash(info);

        }
        /// <summary>
        /// 更新实际支付金额
        /// </summary>
        /// <param name="info"></param>
        public void UpdateCommissionToCashPayAmt(CommissionToCashInfo info)
        {
            commissionToCashDA.UpdateCommissionToCashPayAmt(info);
        }
        /// <summary>
        /// 确认支付
        /// </summary>
        /// <param name="info"></param>
        public void ConfirmCommisonToCash(CommissionToCashInfo info)
        {
            commissionToCashDA.ConfirmCommisonToCash(info);
        }
    }
}
