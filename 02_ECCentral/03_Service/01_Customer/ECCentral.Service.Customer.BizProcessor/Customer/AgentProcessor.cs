using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.BizProcessor
{
    [VersionExport(typeof(AgentProcessor))]
    public class AgentProcessor
    {
        private IAgentDA agentDA = ObjectFactory<IAgentDA>.Instance;
        public virtual AgentInfo UpdateAgent(AgentInfo entity)
        {
            entity = agentDA.UpdateAgent(entity);
            return entity;
        }
        public virtual AgentInfo CreateAgent(AgentInfo entity)
        {
            AgentInfo query = agentDA.GetAgentByCustomerSysNo(entity.CustomerSysNo.Value);
            if (query != null && query.CustomerSysNo == entity.CustomerSysNo)
            {
                entity = UpdateAgent(entity);
                return entity;
            }
            else if (entity.AgentType != null)
            {
                entity.Status = "A";
                entity = agentDA.CreateAgent(entity);
            }
            return entity;
        }



        public virtual AgentInfo GetAgentByCustomerSysNo(int customerSysNo)
        {
            AgentInfo result = new AgentInfo();
            result = agentDA.GetAgentByCustomerSysNo(customerSysNo);
            return result;
        }
    }
}
