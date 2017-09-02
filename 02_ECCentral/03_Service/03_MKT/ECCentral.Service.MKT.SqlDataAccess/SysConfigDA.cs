using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(ISysConfigDA))]
    public class SysConfigDA : ISysConfigDA
    {
        #region ISysConfigDA Members

        public void Update(string key, string value, string channelID)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateSysConfig");
            cmd.SetParameterValue("@Key", key);
            cmd.SetParameterValue("@Value", value);
            cmd.SetParameterValueAsCurrentUserSysNo("@UpdateUserSysNo");
            cmd.ExecuteNonQuery();
        }

        #endregion
    }
}
