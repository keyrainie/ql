using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess
{
     [VersionExport(typeof(IRmaPolicyLogDA))]
   public class RmaPolicyLogDA:IRmaPolicyLogDA
    {
        #region IRmaPolicyLogDA Members

        public void CreateRMAPolicyLog(RmaPolicyInfo info,RmaLogActionType actionType)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateRMAPolicyLog");
            cmd.SetParameterValue("@RMAPolicyName", info.RMAPolicyName);
            cmd.SetParameterValue("@RMAPolicySysNo", info.SysNo);
            cmd.SetParameterValue("@Priority", info.Priority);
            cmd.SetParameterValue("@Type", info.RmaType);
            cmd.SetParameterValue("@Status", info.Status);
            cmd.SetParameterValue("@ECDisplayName", info.ECDisplayName);
            cmd.SetParameterValue("@ECDisplayDesc", info.ECDisplayDesc);
            cmd.SetParameterValue("@ECDisplayMoreDesc", info.ECDisplayMoreDesc);
            cmd.SetParameterValue("@ReturnDate", info.ReturnDate);
            cmd.SetParameterValue("@ChangeDate", info.ChangeDate);
            cmd.SetParameterValue("@IsOnlineRequst", info.IsOnlineRequest);
            cmd.SetParameterValue("@InUser", info.User.UserDisplayName);
            cmd.SetParameterValue("@CompanyCode", info.CompanyCode);
            cmd.SetParameterValue("@StoreCompanyCode", info.CompanyCode);
            cmd.SetParameterValue("@LanguageCode", info.LanguageCode);
            cmd.SetParameterValue("@OperationType", actionType);
            cmd.ExecuteNonQuery();
        }

        #endregion
    }
}
