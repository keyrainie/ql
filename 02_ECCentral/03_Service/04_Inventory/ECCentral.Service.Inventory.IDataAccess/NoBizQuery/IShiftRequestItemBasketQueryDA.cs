using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.QueryFilter.Inventory;

namespace ECCentral.Service.Inventory.IDataAccess.NoBizQuery
{
    public interface IShiftRequestItemBasketQueryDA
    {
        /// <summary>
        /// 查询移仓蓝List
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryShiftRequestItemBasketList(ShiftRequestItemBasketQueryFilter queryFilter, out int totalCount);
    }
}
