using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.MKT;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface ISEOQueryDA
    {
        /// <summary>
        /// SEO查询
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QuerySEO(SEOQueryFilter filter, out int totalCount);
    }
}
