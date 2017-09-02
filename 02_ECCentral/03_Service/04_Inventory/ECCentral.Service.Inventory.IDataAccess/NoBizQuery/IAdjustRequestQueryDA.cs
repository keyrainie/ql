using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.Inventory;

namespace ECCentral.Service.Inventory.IDataAccess.NoBizQuery
{
    public interface IAdjustRequestQueryDA
    {
        /// <summary>
        /// 查询损益单
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryAdjustRequest(AdjustRequestQueryFilter queryCriteria, out int totalCount);
    }
}
