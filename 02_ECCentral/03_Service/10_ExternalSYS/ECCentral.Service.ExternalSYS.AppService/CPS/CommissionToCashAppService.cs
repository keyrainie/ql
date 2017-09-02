using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.ExternalSYS.BizProcessor;
using ECCentral.BizEntity.ExternalSYS;

namespace ECCentral.Service.ExternalSYS.AppService
{
    [VersionExport(typeof(CommissionToCashAppService))]
    public class CommissionToCashAppService
    {
        private CommissionToCashProcessor processor = ObjectFactory<CommissionToCashProcessor>.Instance;
        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="info"></param>
        public void AuditCommisonToCash(CommissionToCashInfo info)
        {
            processor.AuditCommisonToCash(info);

        }
        /// <summary>
        /// 更新实际支付金额
        /// </summary>
        /// <param name="info"></param>
        public void UpdateCommissionToCashPayAmt(CommissionToCashInfo info)
        {
            processor.UpdateCommissionToCashPayAmt(info);
        }
        /// <summary>
        /// 确认支付
        /// </summary>
        /// <param name="info"></param>
        public void ConfirmCommisonToCash(CommissionToCashInfo info)
        {
            processor.ConfirmCommisonToCash(info);
        }
    }
}
