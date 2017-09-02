using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.MKT;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface IHotKeywordsQueryDA
    {
        /// <summary>
        /// 查询热门关键字
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        DataTable QueryHotKeywords(HotKeywordsQueryFilter filter, out int totalCount);
    }
}
