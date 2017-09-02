using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.MKT;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Utility;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface IHomePageSectionQueryDA
    {
        DataTable Query(HomePageSectionQueryFilter filter, out int totalCount);

        List<CodeNamePair> GetDomainCodeNames(string companyCode, string channelID);
    }
}
