using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.MKT;
using System.Data;

namespace ECCentral.Service.MKT.IDataAccess.NoBizQuery
{
    public interface IPollItemQueryDA
    {
        /// <summary>
        /// 查询投票列表
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryPollList(PollQueryFilter filter, out int totalCount);
    }
}
