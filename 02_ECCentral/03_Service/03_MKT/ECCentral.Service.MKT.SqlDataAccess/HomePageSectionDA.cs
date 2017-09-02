using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.SqlDataAccess
{
    [VersionExport(typeof(IHomePageSectionDA))]
    public class HomePageSectionDA : IHomePageSectionDA
    {
        public void Create(HomePageSectionInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Domain_CreateDomain");
            cmd.SetParameterValue(entity);
            cmd.ExecuteNonQuery();
            entity.SysNo = (int)cmd.GetParameterValue("@SysNo");
        }

        public void Update(HomePageSectionInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Domain_UpdateDomain");
            cmd.SetParameterValue(entity);
            cmd.ExecuteNonQuery();
        }

        public HomePageSectionInfo Load(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Domain_Load");
            cmd.SetParameterValue("@SysNo", sysNo);
            return cmd.ExecuteEntity<HomePageSectionInfo>();
        }

        public bool CheckNameExists(int? excludeSysNo, string domainName, string companyCode, string channelID)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Domain_CheckNameExists");
            cmd.SetParameterValue("@SysNo", excludeSysNo??0);
            cmd.SetParameterValue("@DomainName", domainName);
            cmd.SetParameterValue("@CompanyCode", companyCode);
            //TODO:添加渠道参数
            return cmd.ExecuteScalar<int>() != 0;
        }
    }
}
