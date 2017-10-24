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
    [VersionExport(typeof(IControlPanelSocietyDA))]
    public class ControlPanelSocietyDA : IControlPanelSocietyDA
    {
        public ControlPanelSociety CreateSociety(ControlPanelSociety request)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateSociety");
            cmd.SetParameterValue<ControlPanelSociety>(request);
            return cmd.ExecuteEntity<ControlPanelSociety>();
        }

        public ControlPanelSociety UpdateSociety(ControlPanelSociety request)
        {
            request.EditDate = DateTime.Now;
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateSociety");
            cmd.SetParameterValue<ControlPanelSociety>(request);
            return cmd.ExecuteEntity<ControlPanelSociety>();
        }

        public List<ControlPanelSociety> GetSocietyByLoginName(string loginName)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QuerySocietyByLoginName");
            cmd.SetParameterValue("@LoginName", loginName);
            return cmd.ExecuteEntityList<ControlPanelSociety>();
        }


        public ControlPanelSociety GetSocietyBySysNo(int _sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetControlPanelSocietyBySysNo");
            cmd.SetParameterValue("@SysNo", _sysNo);
            ControlPanelSociety item = cmd.ExecuteEntity<ControlPanelSociety>();
            return item;
        }

        public int GetCPSocietysLoginCount(LoginCountRequest request)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetCPSocietysLoginCount");
            cmd.SetParameterValue("@Action", request.Action);
            cmd.SetParameterValue("@OrganizationName", request.SystemNo);
            cmd.SetParameterValue("@Password", request.InUser);
            return cmd.ExecuteScalar<int>();
        }
        public List<ComBoxData> GetSocietyProvince_ComBox(string _sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetSocietyProvince_ComBox");
            List<ComBoxData> item = cmd.ExecuteEntityList<ComBoxData>();
            return item;
            //return cmd.ExecuteScalar<List<ComBoxData>>();
        }
        public List<ComBoxData> GetSocietyCommissionType_ComBox(string _sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetSocietyCommissionType_ComBox");
            //ControlPanelSociety item = cmd.ExecuteEntity<ControlPanelSociety>();
            List<ComBoxData> item = cmd.ExecuteEntityList<ComBoxData>();
            return item;
            //return cmd.ExecuteScalar<List<ComBoxData>>();
        }
    }
}
