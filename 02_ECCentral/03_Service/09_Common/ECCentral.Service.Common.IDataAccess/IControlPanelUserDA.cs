using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.IDataAccess
{
    public interface IControlPanelUserDA
    {
        ControlPanelUser CreateUser(ControlPanelUser request);

        ControlPanelUser UpdateUser(ControlPanelUser request);

        List<ControlPanelUser> GetUserByLoginName(string loginName);

        ControlPanelUser GetUserBySysNo(int _sysNo);

        int GetCPUsersLoginCount(LoginCountRequest request);
    }
}
