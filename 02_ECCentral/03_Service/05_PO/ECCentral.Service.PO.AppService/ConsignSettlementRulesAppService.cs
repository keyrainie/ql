using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.PO;
using ECCentral.Service.PO.BizProcessor;

namespace ECCentral.Service.PO.AppService
{
    [VersionExport(typeof(ConsignSettlementRulesAppService))]
    public class ConsignSettlementRulesAppService
    {
        #region [Fields]
        private ConsignSettlementRulesProcessor m_ConsignSettlementRulesProcessor;

        public ConsignSettlementRulesProcessor ConsignSettlementRulesProcessor
        {
            get
            {
                if (null == m_ConsignSettlementRulesProcessor)
                {
                    m_ConsignSettlementRulesProcessor = ObjectFactory<ConsignSettlementRulesProcessor>.Instance;
                }
                return m_ConsignSettlementRulesProcessor;
            }
        }
        #endregion

        public virtual ConsignSettlementRulesInfo CreateConsignRule(ConsignSettlementRulesInfo info)
        {
            return ConsignSettlementRulesProcessor.CreateConsignSettlementRule(info);
        }

        public virtual ConsignSettlementRulesInfo UpdateConsignRule(ConsignSettlementRulesInfo info)
        {
            return ConsignSettlementRulesProcessor.UpdateConsignSettlementRule(info);
        }

        public virtual ConsignSettlementRulesInfo AuditConsignRule(ConsignSettlementRulesInfo info)
        {
            return ConsignSettlementRulesProcessor.AuditConsignSettlementRule(info);
        }
        public virtual ConsignSettlementRulesInfo StopConsignRule(string settleRuleCode)
        {
            return ConsignSettlementRulesProcessor.StopConsignSettlementRule(settleRuleCode);
        }
        public virtual ConsignSettlementRulesInfo AbandonConsignRule(string settleRuleCode)
        {
            return ConsignSettlementRulesProcessor.AbandonConsignSettlementRule(settleRuleCode);
        }

    }
}
