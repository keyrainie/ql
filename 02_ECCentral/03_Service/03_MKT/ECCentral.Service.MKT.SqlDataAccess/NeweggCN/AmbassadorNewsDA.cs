using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.MKT.SqlDataAccess.NeweggCN
{
    [VersionExport(typeof(IAmbassadorNewsDA))]
    public class AmbassadorNewsDA : IAmbassadorNewsDA
    {

        #region IAmbassadorNewsDA Members

        public void BatchUpdateAmbassadorNewsStatus(BizEntity.MKT.AmbassadorNewsBatchInfo batchInfo)
        {

            StringBuilder message = new StringBuilder();
            foreach (int sysNo in batchInfo.AmbassadorNewsSysNos)
            {
                message.Append(sysNo.ToString() + ",");
            }
            DataCommand dc = DataCommandManager.GetDataCommand("BatchUpdateAmbassadorNewsStatus");
            dc.SetParameterValue("@SysNos", message.ToString().TrimEnd(','));
            dc.SetParameterValue("@Status", batchInfo.Status);
            dc.SetParameterValue("@CompanyCode", batchInfo.CompanyCode);

            dc.ExecuteNonQuery();
        }

        #endregion
    }
}
