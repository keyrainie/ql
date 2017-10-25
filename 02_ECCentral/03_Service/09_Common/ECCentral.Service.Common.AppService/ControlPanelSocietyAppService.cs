using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Common.BizProcessor;

namespace ECCentral.Service.Common.AppService
{
    [VersionExport(typeof(ControlPanelSocietyAppService))]
    public class ControlPanelSocietyAppService
    {
        public virtual ControlPanelSociety GetControlPanelSocietyByLoginName(string loginName)
        {
            return ObjectFactory<ControlPanelSocietyProcessor>.Instance.GetControlPanelSocietyByLoginName(loginName);
        }

        public virtual ControlPanelSociety Create(ControlPanelSociety request)
        {
            return ObjectFactory<ControlPanelSocietyProcessor>.Instance.Create(request);
        }

        public virtual ControlPanelSociety Update(ControlPanelSociety request)
        {
            return ObjectFactory<ControlPanelSocietyProcessor>.Instance.Update(request);
        }

        public ControlPanelSociety GetSocietyBySysNo(int _sysNo)
        {
            return ObjectFactory<ControlPanelSocietyProcessor>.Instance.GetSocietyBySysNo(_sysNo);
        }

        public int GetCPSocietysLoginCount(LoginCountRequest request)
        {
            return ObjectFactory<ControlPanelSocietyProcessor>.Instance.GetCPSocietysLoginCount(request);
        }

        public virtual ControlPanelSociety GetCPSocietysLoginSociety(string loginName)
        {
            return ObjectFactory<ControlPanelSocietyProcessor>.Instance.GetCPSocietysLoginSociety(loginName);
        }

        public virtual List<ComBoxData> GetSocietyProvince_ComBox(string loginName)
        {
            return ObjectFactory<ControlPanelSocietyProcessor>.Instance.GetSocietyProvince_ComBox(loginName);
        }
        public virtual List<ComBoxData> GetSocietyCommissionType_ComBox(string loginName)
        {
            return ObjectFactory<ControlPanelSocietyProcessor>.Instance.GetSocietyCommissionType_ComBox(loginName);
        }
    }
}
