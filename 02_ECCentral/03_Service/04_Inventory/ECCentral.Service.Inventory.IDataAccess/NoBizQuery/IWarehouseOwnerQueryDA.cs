using System.Data;

using ECCentral.QueryFilter.Inventory;

namespace ECCentral.Service.Inventory.IDataAccess.NoBizQuery
{
    public interface IWarehouseOwnerQueryDA
    {        
        /// <summary>
        /// 查询仓库所有者
        /// </summary>
        /// <returns></returns>
        DataTable QueryWarehouseOwner(WarehouseOwnerQueryFilter queryCriteria, out int totalCount);
    }
}
