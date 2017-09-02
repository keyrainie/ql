using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Customer.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.Customer.SqlDataAccess
{
    [VersionExport(typeof(ICustomerEmailDA))]
    public class CustomerEmailDA : ICustomerEmailDA
    {
        #region ICustomerEmailDA Members

        public virtual string GetEmailContent(int sysNo, string dbName)
        {
            DataCommand cmd = null;
            if (dbName.ToLower() == "maildb")
                cmd = DataCommandManager.GetDataCommand("GetEmailContentInMailDB");
            else
                cmd = DataCommandManager.GetDataCommand("GetEmailContentInIPP3");
            cmd.SetParameterValue("@SysNo", sysNo);
            var ds = cmd.ExecuteDataSet();
            return ds.Tables[0].Rows[0][0].ToString();
        }

        #endregion
    }
}
