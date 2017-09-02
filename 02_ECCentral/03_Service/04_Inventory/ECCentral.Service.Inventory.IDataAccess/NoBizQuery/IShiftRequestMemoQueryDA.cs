using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using ECCentral.QueryFilter.Inventory;

namespace ECCentral.Service.Inventory.IDataAccess.NoBizQuery
{
    public interface IShiftRequestMemoQueryDA
    {
        /// <summary>
        /// 查询移仓单跟进日志
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryShiftRequestMemo(ShiftRequestMemoQueryFilter queryCriteria, out int totalCount);
    }
}
