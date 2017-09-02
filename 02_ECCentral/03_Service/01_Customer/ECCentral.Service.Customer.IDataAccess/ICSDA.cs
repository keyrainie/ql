using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Customer.IDataAccess
{
    public interface ICSDA
    {

        CSInfo GetCSByIPPUserSysNo(int ippUserSysNo, CSInfo entity);
        CSInfo InsertCS(CSInfo entity, bool hasCheck);
        CSInfo GetCSBySysNo(int sysNo);
        List<CSInfo> GetCSByLeaderSysNo(int LeaderSysNo);
        int UpdateCS(CSInfo entity);
        int UpdateCSByIPPUserSysNo(CSInfo entity);
        List<CSInfo> GetCSWithDepartmentId(int depid);
        List<CSInfo> GetAllCS(string companyCode);
    }
}
