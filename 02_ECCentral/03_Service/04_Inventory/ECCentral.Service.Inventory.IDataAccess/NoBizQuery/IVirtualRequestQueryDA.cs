using System.Data;

using ECCentral.QueryFilter.Inventory;

namespace ECCentral.Service.Inventory.IDataAccess.NoBizQuery
{
    public interface IVirtualRequestQueryDA
    {
        /// <summary>
        /// 查询虚库申请单
        /// </summary>
        /// <returns></returns>
        DataTable QueryVirtualRequest(VirtualRequestQueryFilter queryFilter, out int totalCount);

        /// <summary>
        /// 查询虚库日志
        /// </summary>
        /// <returns></returns>
        DataTable QueryVirtualRequestMemo(VirtualRequestQueryFilter queryFilter, out int totalCount);

        /// <summary>
        /// 加虚库单关闭日志List:
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QueryVirtualRequestCloseLog(VirtualRequestQueryFilter queryFilter, out int totalCount);

        DataTable QueryProducts(VirtualRequestQueryProductsFilter filter, out int totalCount);


        /// <summary>
        /// 查询虚库单库存信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        DataTable QueryVirtualInventoryInfoByStock(VirtualRequestQueryFilter queryFilter);


        /// <summary>
        /// 查询虚库单 - 查询最后虚库变更List：
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        DataTable QueryVirtualInventoryLastVerifiedRequest(VirtualRequestQueryFilter queryFilter);

        /// <summary>
        /// 查询虚库单 - 查询虚库变更List：
        /// </summary>
        /// <param name="queryFilter"></param>
        /// <returns></returns>
        DataTable QueryModifiedVirtualRequest(VirtualRequestQueryFilter queryFilter,out int totalCount);

        /// <summary>
        /// 查询虚库日志创建者列表
        /// </summary>
        /// <returns></returns>
        DataTable QueryVirtualRequestMemoCreateUserList(string companyCode, out int totalCount);

         /// <summary>
        /// 查询虚库申请单创建者列表
        /// </summary>
        /// <returns></returns>
        DataTable QueryVirtualRequestCreateUserList(string companyCode, out int totalCount);
    }
}
