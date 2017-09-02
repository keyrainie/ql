using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Data;

namespace ECCentral.Service.Common.SqlDataAccess
{
    [VersionExport(typeof(ISynMappingAndSysUserDA))]
    public class SynMappingAndSysUserDA:ISynMappingAndSysUserDA
    {
        public int GetExistUserSysNo(ControlPanelUser controlPanelUser)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetExistUserSysNo");
            command.SetParameterValue("@SourceUserName", controlPanelUser.LoginName);
            command.SetParameterValue("@SourceDirectoryKey", controlPanelUser.SourceDirectory);
            command.SetParameterValue("@CompanyCode", controlPanelUser.CompanyCode);
            object result = command.ExecuteScalar();
            if (result != null)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                return 0;
            }
        }

        public int GetExistUserSysNoInOldData(ControlPanelUser controlPanelUser, int mappingUserSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetExistUserSysNoInOldData");
            command.SetParameterValue("@UserID", controlPanelUser.LoginName);
            command.SetParameterValue("@UserSysNo", mappingUserSysNo);
            object result = command.ExecuteScalar();
            if (result != null)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                return 0;
            }
        }

        public int GenerateUserSysNo()
        {
            DataCommand newCmd = DataCommandManager.GetDataCommand("GenerateUserSysNo");
            object newNum = newCmd.ExecuteScalar();
            DataCommand oldCmd = DataCommandManager.GetDataCommand("GenerateUserSysNoInOldData");
            object oldNum = oldCmd.ExecuteScalar();
            int num1 = newNum == null ? 1 : Convert.ToInt32(newNum);
            int num2 = oldNum == null ? 1 : Convert.ToInt32(oldNum);
            return Math.Max(num1, num2);
        }

        public void SynUserMapping(ControlPanelUser controlPanelUser, int userSysNo, int generateUserSysNo)
        {
            DataCommand cmd;
            if (userSysNo == 0)
            {
                cmd = DataCommandManager.GetDataCommand("InsertNewUser");
                cmd.SetParameterValue("@GenerateUserSysNo", generateUserSysNo);
            }
            else
            {
                cmd = DataCommandManager.GetDataCommand("UpdateExistUser");
                cmd.SetParameterValue("@IPPUserSysNo", userSysNo);
            }

            cmd.SetParameterValue("@ACLogicUserName", controlPanelUser.DisplayName);
            cmd.SetParameterValue("@ACPhysicalUserName", controlPanelUser.LoginName);
            cmd.SetParameterValue("@ACSourceDirectoryKey", controlPanelUser.SourceDirectory);
            cmd.SetParameterValue("@CompanyCode", controlPanelUser.CompanyCode);
            cmd.SetParameterValue("@DepartmentCode", controlPanelUser.DepartmentCode);
            cmd.SetParameterValue("@DepartmentName", controlPanelUser.DepartmentName);
            cmd.SetParameterValue("@EmailAddress", controlPanelUser.EmailAddress);
            cmd.SetParameterValue("@Status", MappingStatus(controlPanelUser.Status));
            cmd.SetParameterValue("@PhoneNumber", controlPanelUser.PhoneNumber);

            cmd.ExecuteNonQuery();
        }

        public void SynSysUser(ControlPanelUser controlPanelUser, int userSysNo, int generateUserSysNo)
        {
            DataCommand cmd;

            if (userSysNo == 0)
            {
                cmd = DataCommandManager.GetDataCommand("InsertNewUserInOldData");
                cmd.SetParameterValue("@GenerateUserSysNo", generateUserSysNo);
                cmd.SetParameterValue("@UserID", controlPanelUser.LoginName);
            }
            else
            {
                cmd = DataCommandManager.GetDataCommand("UpdateExistUserInOldData");
                cmd.SetParameterValue("@SysNo", userSysNo);
            }
            cmd.SetParameterValue("@UserName", controlPanelUser.DisplayName);
            cmd.SetParameterValue("@Email", controlPanelUser.EmailAddress);
            cmd.SetParameterValue("@Phone", controlPanelUser.PhoneNumber);
            cmd.SetParameterValue("@Status", MappingStatus(controlPanelUser.Status));

            cmd.ExecuteNonQuery();
        }

        private string MappingStatus(ControlPanelUserStatus? status)
        {
            if (status == ControlPanelUserStatus.A)
                return "0";
            else if (status == ControlPanelUserStatus.D)
                return "-1";
            return status.ToString();
        }
    }
}
