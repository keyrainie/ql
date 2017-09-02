using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Customer.SqlDataAccess
{
    [VersionExport(typeof(IRankDA))]
    public class RankDA : IRankDA
    {
        #region IRankDA Members

        public virtual void SetRank(int customerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCustomerRank");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            cmd.ExecuteNonQuery();
        }

        public virtual void SetRank(int customerSysNo, BizEntity.Customer.CustomerRank rank)
        {
            throw new NotImplementedException();
        }

        public virtual void SetVIPRank(int customerSysNo)
        {
            throw new NotImplementedException();
        }

        public virtual void SetVIPRank(int CustomerSysNo, BizEntity.Customer.VIPRank rank)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateVipRank");
            cmd.SetParameterValue("@SysNo", CustomerSysNo);
            cmd.SetParameterValue("@VIPRank", rank);
            cmd.ExecuteNonQuery();
        }

        #endregion
    }
}
