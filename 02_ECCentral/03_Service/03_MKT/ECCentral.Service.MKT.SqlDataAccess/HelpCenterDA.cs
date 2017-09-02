using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity.MKT;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(IHelpCenterDA))]
    public class HelpCenterDA:IHelpCenterDA
    {
        

        public void Create(HelpTopic entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("HelpCenter_Insert");
            cmd.SetParameterValue(entity);

            cmd.ExecuteNonQuery();
        }

        public void Update(HelpTopic entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("HelpCenter_Update");
            cmd.SetParameterValue(entity);

            cmd.ExecuteNonQuery();
        }

        public HelpTopic Load(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("HelpCenter_Load");
            cmd.SetParameterValue("@SysNo", sysNo);

            return cmd.ExecuteEntity<HelpTopic>();
        }

        public bool CheckTitleExists(int excludeSysNo, string title, string companyCode, string channelID)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("HelpCenter_CheckTitleExists");
            cmd.SetParameterValue("@ExcludeSysNo", excludeSysNo);
            cmd.SetParameterValue("@Title", title);
            cmd.SetParameterValue("@CompanyCode", companyCode);

            return cmd.ExecuteScalar() != null;
        }
    }
}
