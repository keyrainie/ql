using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.IDataAccess
{
    public interface IAgentDA
    {
        AgentInfo CreateAgent(AgentInfo entity);

        AgentInfo UpdateAgent(AgentInfo entity);

        AgentInfo GetAgentByCustomerSysNo(int customerSysNo);
    }
}
