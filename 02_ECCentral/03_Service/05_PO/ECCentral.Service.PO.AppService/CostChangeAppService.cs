using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.BizProcessor;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Common;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.Inventory;
using System.Data;
using ECCentral.BizEntity;
using System.Threading;

namespace ECCentral.Service.PO.AppService
{
    [VersionExport(typeof(CostChangeAppService))]
    public class CostChangeAppService
    {
        #region [Fields]
        private CostChangeProcessor m_CostChangeProcessor;

        public CostChangeProcessor CostChangeProcessor
        {
            get
            {
                if (null == m_CostChangeProcessor)
                {
                    m_CostChangeProcessor = ObjectFactory<CostChangeProcessor>.Instance;
                }
                return m_CostChangeProcessor;
            }
        }
        #endregion

        public CostChangeInfo LoadCostChangeInfo(int ccSysNo)
        {
            return CostChangeProcessor.LoadCostChangeInfo(ccSysNo);
        }

        public CostChangeInfo CreateCostChange(CostChangeInfo info)
        {
            return CostChangeProcessor.CreateCostChange(info);
        }

        public CostChangeInfo UpdateCostChange(CostChangeInfo info)
        {
            return CostChangeProcessor.UpdateCostChange(info);
        }

        public CostChangeInfo SubmitAuditCostChange(CostChangeInfo info)
        {
            return CostChangeProcessor.SubmitAuditCostChange(info);
        }

        public CostChangeInfo CancelSubmitAuditPOCostChange(CostChangeInfo info)
        {
            return CostChangeProcessor.CancelSubmitAuditPOCostChange(info);
        }

        public CostChangeInfo RefuseCostChange(CostChangeInfo info)
        {
            return CostChangeProcessor.RefuseCostChange(info);
        }

        public CostChangeInfo AbandonCostChange(CostChangeInfo info)
        {
            return CostChangeProcessor.AbandonCostChange(info);
        }

        public CostChangeInfo AuditCostChange(CostChangeInfo info)
        {
            return CostChangeProcessor.AuditCostChange(info);
        }
    }
}
