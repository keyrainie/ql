using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface IHomePageSectionDA
    {
        void Create(HomePageSectionInfo entity);

        void Update(HomePageSectionInfo entity);

        HomePageSectionInfo Load(int sysNo);

        bool CheckNameExists(int? excludeSysNo,string domainName,string companyCode,string channelID);
    }
}
