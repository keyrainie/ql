using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
namespace ECCentral.Service.Common.IDataAccess
{
    public interface IControlPanelSocietyDA
    {
        ControlPanelSociety CreateSociety(ControlPanelSociety request);

        ControlPanelSociety UpdateSociety(ControlPanelSociety request);

        List<ControlPanelSociety> GetSocietyByLoginName(string loginName);

        ControlPanelSociety GetSocietyBySysNo(int _sysNo);

        int GetCPSocietysLoginCount(LoginCountRequest request);

        List<ComBoxData> GetSocietyProvince_ComBox(string request);

        List<ComBoxData> GetSocietyCommissionType_ComBox(string request);
    }
}
