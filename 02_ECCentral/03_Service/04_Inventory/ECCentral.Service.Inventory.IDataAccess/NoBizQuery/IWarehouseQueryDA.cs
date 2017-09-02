using System.Data;

using ECCentral.QueryFilter.Inventory;
namespace ECCentral.Service.Inventory.IDataAccess.NoBizQuery
{
    public interface IWarehouseQueryDA
    {
        /// <summary>
        /// 查询仓库
        /// </summary>
        /// <returns></returns>
        DataTable QueryWarehouse(WarehouseQueryFilter queryCriteria, out int totalCount);

    }
}
