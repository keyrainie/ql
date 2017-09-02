using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess
{
     [VersionExport(typeof(IRmaPolicyDA))]
   public class RmaPolicyDA : IRmaPolicyDA
    {

        #region IRmaPolicyDA Members

        public void CreateRmaPolicy(RmaPolicyInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateRmaPolicy");
            cmd.SetParameterValue("@RMAPolicyName",info.RMAPolicyName);
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
            cmd.ExecuteNonQuery();
            info.SysNo = (int)cmd.GetParameterValue("@SysNo");
        }

        public void UpdateRmaPolicy(RmaPolicyInfo info)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateRmaPolicy");
            cmd.SetParameterValue("@RMAPolicyName", info.RMAPolicyName);
            cmd.SetParameterValue("@Priority", info.Priority);
            cmd.SetParameterValue("@Type", info.RmaType);
            cmd.SetParameterValue("@Status", info.Status);
            cmd.SetParameterValue("@ECDisplayName", info.ECDisplayName);
            cmd.SetParameterValue("@ECDisplayDesc", info.ECDisplayDesc);
            cmd.SetParameterValue("@ECDisplayMoreDesc", info.ECDisplayMoreDesc);
            cmd.SetParameterValue("@ReturnDate", info.ReturnDate);
            cmd.SetParameterValue("@ChangeDate", info.ChangeDate);
            cmd.SetParameterValue("@IsOnlineRequst", info.IsOnlineRequest);
            cmd.SetParameterValue("@SysNo", info.SysNo);
            cmd.SetParameterValue("@EditUser", info.User.UserDisplayName);
            cmd.ExecuteNonQuery();
        }

        public void DeActiveRmaPolicy(int sysNo,UserInfo user)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ChangeRmaPolicyStatus");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.SetParameterValue("@EditUser", user.UserDisplayName);
            cmd.SetParameterValue("@Status", RmaPolicyStatus.DeActive);
            cmd.ExecuteNonQuery();

        }

        public void ActiveRmaPolicy(int sysNo,UserInfo user)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("ChangeRmaPolicyStatus");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.SetParameterValue("@EditUser", user.UserDisplayName);
            cmd.SetParameterValue("@Status", RmaPolicyStatus.Active);
            cmd.ExecuteNonQuery();
        }

        #endregion

     

        #region IRmaPolicyDA Members

        /// <summary>
        /// 检查是否已存在标准类型（Type='p'）并且状态为有效(status='A')
        /// </summary>
        /// <returns></returns>
        public bool IsExistsRmaPolicy(int sysNo = 0)
        {
             DataCommand cmd = DataCommandManager.GetDataCommand("IsExistsRmaPolicy");
             cmd.SetParameterValue("@SysNO", sysNo);
            cmd.ExecuteNonQuery();
            return (int)cmd.GetParameterValue("@flag") < 0;
        }

        #endregion


    }
}
