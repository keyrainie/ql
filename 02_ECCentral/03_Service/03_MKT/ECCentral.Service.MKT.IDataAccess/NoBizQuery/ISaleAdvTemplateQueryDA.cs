using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.MKT;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.Utility;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface ISaleAdvTemplateQueryDA
    {
        List<WebPage> GetActiveCodeNames(string companyCode, string channelID);

        DataTable Query(SaleAdvTemplateQueryFilter filter, out int totalCount);

        List<WebPage> GetNowActiveCodeNames();
    }
}
