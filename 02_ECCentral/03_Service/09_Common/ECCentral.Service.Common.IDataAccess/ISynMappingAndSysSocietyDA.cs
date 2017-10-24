using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
namespace ECCentral.Service.Common.IDataAccess
{
    public interface ISynMappingAndSysSocietyDA
    {
        int GetExistSocietySysNo(ControlPanelSociety controlPanelSociety);

        int GetExistSocietySysNoInOldData(ControlPanelSociety controlPanelSociety, int mappingSocietySysNo);

        int GenerateSocietySysNo();

        void SynSocietyMapping(ControlPanelSociety controlPanelSociety, int mappingSocietySysNo, int generateSocietySysNo);

        void SynSysSociety(ControlPanelSociety controlPanelSociety, int sysSocietyNo, int generateSocietySysNo);
    }
}
