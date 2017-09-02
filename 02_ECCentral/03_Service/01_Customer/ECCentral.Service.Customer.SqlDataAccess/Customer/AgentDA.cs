using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ECCentral.BizEntity.Customer;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Customer.SqlDataAccess
{
    [VersionExport(typeof(IAgentDA))]
    public class AgentDA : IAgentDA
    {
        #region IAgentDA Members

        public virtual AgentInfo CreateAgent(AgentInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertNeweggAgent");
            cmd.SetParameterValue<AgentInfo>(entity);
            cmd.ExecuteNonQuery();
            return entity;
        }

        public virtual AgentInfo UpdateAgent(AgentInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateNeweggAgent");   
            cmd.SetParameterValue<AgentInfo>(entity);
            cmd.ExecuteNonQuery();
            return entity;
        }

        public virtual AgentInfo GetAgentByCustomerSysNo(int customerSysNo)
        {
            AgentInfo agent = new AgentInfo();
            DataCommand cmd = DataCommandManager.GetDataCommand("GetNeweggAgentByCustomerSysNo");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            return cmd.ExecuteEntity<AgentInfo>();
        }

        #endregion
    }
}
