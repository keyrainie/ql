using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.BizEntity.Inventory;

using ECCentral.QueryFilter.Inventory;

namespace ECCentral.Service.Inventory.IDataAccess.NoBizQuery
{
    public interface IShiftRequestQueryDA
    {
        /// <summary>
        /// 查询移仓单
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryShiftRequest(ShiftRequestQueryFilter queryCriteria, out int totalCount);

        DataSet QueryCountData(ShiftRequestQueryFilter queryCriteria);

        /// <summary>
        /// 仓库移仓配置查询
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        DataTable QueryStockShiftConfig(StockShiftConfigFilter filter, out int totalCount);        

        /// <summary>
        /// 查询移仓单创建人列表
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        DataTable QueryShiftRequestCreateUserList(string companyCode, out int totalCount);

        /// <summary>
        /// 获取RMA移仓单 注：和inventory移仓单是不同的表
        /// </summary>
        /// <param name="shiftSysNo"></param>
        /// <returns></returns>
        DataTable GetRMAShift(int shiftSysNo);
    }
}
