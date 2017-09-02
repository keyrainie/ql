using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.MKT;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface IDefaultKeywordsQueryDA
    {
        /// <summary>
        /// 查询默认关键字
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        DataTable QueryDefaultKeywords(DefaultKeywordsQueryFilter filter, out int totalCount);
    }
}
