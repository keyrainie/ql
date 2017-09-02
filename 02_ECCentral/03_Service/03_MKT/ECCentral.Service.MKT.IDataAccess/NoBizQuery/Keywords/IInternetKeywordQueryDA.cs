using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.MKT;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface IInternetKeywordQueryDA
    {

        /// <summary>
        /// 查询外网搜素
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryKeyword(InternetKeywordQueryFilter queryCriteria, out int totalCount);
    }
}
