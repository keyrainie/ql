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
    [VersionExport(typeof(IControlPanelUserDA))]
    public class ControlPanelUserDA:IControlPanelUserDA
    {
        public ControlPanelUser CreateUser(ControlPanelUser request)
        {
            request.InDate = DateTime.Now;
            request.EditDate = request.InDate;
            request.EditUser = request.InUser;
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateUser");
            cmd.SetParameterValue<ControlPanelUser>(request);
            return cmd.ExecuteEntity<ControlPanelUser>();
        }

        public ControlPanelUser UpdateUser(ControlPanelUser request)
        {
            request.EditDate = DateTime.Now;
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateUser");
            cmd.SetParameterValue<ControlPanelUser>(request);
            return cmd.ExecuteEntity<ControlPanelUser>();
        }

        public List<ControlPanelUser> GetUserByLoginName(string loginName)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryUserByLoginName");
            cmd.SetParameterValue("@LoginName", loginName);
            return cmd.ExecuteEntityList<ControlPanelUser>();
        }


        public ControlPanelUser GetUserBySysNo(int _sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetControlPanelUserBySysNo");
            cmd.SetParameterValue("@SysNo", _sysNo);
            ControlPanelUser item = cmd.ExecuteEntity<ControlPanelUser>();
            return item;
        }

        public int GetCPUsersLoginCount(LoginCountRequest request)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCPUsersLoginCount");
            cmd.SetParameterValue("@Action", request.Action);
            cmd.SetParameterValue("@SystemNo", request.SystemNo);
            cmd.SetParameterValue("@InUser", request.InUser);
            return cmd.ExecuteScalar<int>();
        }
    }
}
