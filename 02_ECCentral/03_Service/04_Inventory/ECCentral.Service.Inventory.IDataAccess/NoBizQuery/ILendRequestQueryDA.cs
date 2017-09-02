using System.Data;

using ECCentral.QueryFilter.Inventory;

namespace ECCentral.Service.Inventory.IDataAccess.NoBizQuery
{
    public interface ILendRequestQueryDA
    {
        /// <summary>
        /// 查询借货单
        /// </summary>
        /// <returns></returns>
        DataTable QueryLendRequest(LendRequestQueryFilter queryCriteria, out int totalCount);
        
        /// <summary>
        /// 按PM统计
        /// </summary>
        /// <param name="queryCriteria"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable ExportAllByPM(LendRequestQueryFilter queryCriteria, out int totalCount);

        /// <summary>
        /// 根据状态获取对应状态下的成本
        /// </summary>
        /// <param name="queryCriteria">查询条件</param>
        /// <returns></returns>
        DataTable QueryLendCostbyStatus(LendRequestQueryFilter queryCriteria);
    }
}
