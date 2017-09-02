using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IUnicomContractPhoneDA))]
    public class UnicomContractPhoneDA : IUnicomContractPhoneDA
    {

        #region IUnicomContractPhone Members

        public void UpdateUnicomContractPhoneNumberStatus(string phone, UnicomContractPhoneNumberStatus status, UserInfo operationUser)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateUnicomContractPhoneNumberStatus");
            cmd.SetParameterValue("@CellPhone", phone);
            cmd.SetParameterValue("@Status", status);
            cmd.SetParameterValue("@EditUser", operationUser.UserName);
            cmd.ExecuteNonQuery();
        }

        #endregion
    }
}
