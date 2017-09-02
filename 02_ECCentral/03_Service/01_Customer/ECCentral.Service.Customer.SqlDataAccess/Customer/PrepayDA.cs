using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Customer.SqlDataAccess
{
    [VersionExport(typeof(IPrepayDA))]
    public class PrepayDA : IPrepayDA
    {
        public virtual void UpdatePrepay(int customerSysNo, decimal prepay)
        {
            DataCommand cmd  = DataCommandManager.GetDataCommand("UpdatePrepay");
            cmd.SetParameterValue("@SysNo", customerSysNo);
            cmd.SetParameterValue("@ValidPrepayAmt", prepay);
            cmd.ExecuteNonQuery();
        }


        public virtual void CreatePrepayLog(CustomerPrepayLog log)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("InsertPrepayLog");
            cmd.SetParameterValue<CustomerPrepayLog>(log);
            cmd.ExecuteNonQuery();
        }

    }
}
