using System.Data;

using ECCentral.QueryFilter.Inventory;

namespace ECCentral.Service.Inventory.IDataAccess.NoBizQuery
{
    public interface IStockQueryDA
    {
        /// <summary>
        /// 查询渠道仓库
        /// </summary>
        /// <returns></returns>
        DataTable QueryStock(StockQueryFilter queryCriteria, out int totalCount);
    }
}
