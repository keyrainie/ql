using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.BizProcessor;

namespace ECCentral.Service.PO.AppService
{
    [VersionExport(typeof(GatherSettlementAppService))]
    public class GatherSettlementAppService
    {
        #region [Fields]
        private GatherSettlementProcessor m_GatherSettlementProcessor;

        public GatherSettlementProcessor GatherSettlementProcessor
        {
            get
            {
                if (null == m_GatherSettlementProcessor)
                {
                    m_GatherSettlementProcessor = ObjectFactory<GatherSettlementProcessor>.Instance;
                }
                return m_GatherSettlementProcessor;
            }

        }
        #endregion


        public GatherSettlementInfo LoadGatherSettlementInfo(int gatherSysNo)
        {
            GatherSettlementInfo gatherInfo = new GatherSettlementInfo() { SysNo = gatherSysNo };
            return GatherSettlementProcessor.LoadGetherSettlement(gatherInfo);
        }

        public GatherSettlementInfo CreateGatherSettlementInfo(GatherSettlementInfo info)
        {
            return GatherSettlementProcessor.CreateGatherSettlement(info);
        }

        public GatherSettlementInfo UpdateGatherSettlementInfo(GatherSettlementInfo info)
        {
            return GatherSettlementProcessor.UpdateGatherSettlement(info);
        }

        public GatherSettlementInfo AbandonGatherSettlementInfo(GatherSettlementInfo info)
        {
            return GatherSettlementProcessor.AbandonGatherSettlement(info);
        }

        public GatherSettlementInfo AuditGatherSettlementInfo(GatherSettlementInfo info)
        {
            return GatherSettlementProcessor.AuditGatherSettlement(info);
        }

        public GatherSettlementInfo CancelAuditGatherSettlementInfo(GatherSettlementInfo info)
        {
            return GatherSettlementProcessor.CancelAuditGatherSettlement(info);
        }

        public GatherSettlementInfo SettleGatherSettlementInfo(GatherSettlementInfo info)
        {
            return GatherSettlementProcessor.SettleGatherSettlement(info);
        }

        public GatherSettlementInfo CancelSettleGatherSettlementInfo(GatherSettlementInfo info)
        {
            return GatherSettlementProcessor.CancelSettleGatherSettlement(info);
        }

    }
}
