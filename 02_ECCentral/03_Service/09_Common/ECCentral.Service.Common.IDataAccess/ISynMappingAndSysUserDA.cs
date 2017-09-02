using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.IDataAccess
{
    public interface ISynMappingAndSysUserDA
    {
        int GetExistUserSysNo(ControlPanelUser controlPanelUser);

        int GetExistUserSysNoInOldData(ControlPanelUser controlPanelUser, int mappingUserSysNo);

        int GenerateUserSysNo();

        void SynUserMapping(ControlPanelUser controlPanelUser, int mappingUserSysNo, int generateUserSysNo);

        void SynSysUser(ControlPanelUser controlPanelUser, int sysUserNo, int generateUserSysNo);
    }
}
