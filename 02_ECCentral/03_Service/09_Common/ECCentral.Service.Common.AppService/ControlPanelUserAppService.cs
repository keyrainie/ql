using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.BizProcessor;

namespace ECCentral.Service.Common.AppService
{
    [VersionExport(typeof(ControlPanelUserAppService))]
    public class ControlPanelUserAppService
    {

        public virtual ControlPanelUser GetControlPanelUserByLoginName(string loginName)
        {
            return ObjectFactory<ControlPanelUserProcessor>.Instance.GetControlPanelUserByLoginName(loginName);
        }

        public virtual ControlPanelUser Create(ControlPanelUser request)
        {
            return ObjectFactory<ControlPanelUserProcessor>.Instance.Create(request);
        }

        public virtual ControlPanelUser Update(ControlPanelUser request)
        {
            return ObjectFactory<ControlPanelUserProcessor>.Instance.Update(request);
        }

        public ControlPanelUser GetUserBySysNo(int _sysNo)
        {
            return ObjectFactory<ControlPanelUserProcessor>.Instance.GetUserBySysNo(_sysNo);
        }

        public int GetCPUsersLoginCount(LoginCountRequest request)
        {
            return ObjectFactory<ControlPanelUserProcessor>.Instance.GetCPUsersLoginCount(request);
        }

        public virtual ControlPanelUser GetCPUsersLoginUser(string loginName)
        {
            return ObjectFactory<ControlPanelUserProcessor>.Instance.GetCPUsersLoginUser(loginName);
        }
    }
}
