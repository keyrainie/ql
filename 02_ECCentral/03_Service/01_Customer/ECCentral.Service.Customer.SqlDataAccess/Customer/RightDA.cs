using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Customer;
using System.Data;

namespace ECCentral.Service.Customer.SqlDataAccess
{
    [VersionExport(typeof(IRightDA))]
    public class RightDA : IRightDA
    {
        #region IRightDA Members

        public virtual List<CustomerRight> LoadAllCustomerRight(int customerSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetALLCustomerRight");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            return cmd.ExecuteEntityList<CustomerRight>();
        }

        public virtual bool CreateCustomerRight(int customerSysNo, int right)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertCustomerRight");
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            cmd.SetParameterValue("@Right", right);
            cmd.SetParameterValueAsCurrentUserAcct("@CreateUserName");
            cmd.ExecuteNonQuery();
            return true;
        }

        public virtual bool UpdateCustomerRight(int customerSysNo, int right)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateCustomerRight");
            cmd.SetParameterValue("@Right", right);
            cmd.SetParameterValue("@CustomerSysNo", customerSysNo);
            cmd.SetParameterValueAsCurrentUserAcct("@EidtUserName");
            cmd.ExecuteNonQuery();
            return true;
        }

        #endregion
    }
}
