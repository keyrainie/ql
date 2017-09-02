using System.Data;
using ECCentral.QueryFilter.ExternalSYS;

namespace ECCentral.Service.ExternalSYS.IDataAccess
{
    public interface IAccruedReportDA
    {
        /// <summary>
        /// 应计返利报表查询（周期）
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="totalCount">返回总记录数</param>
        /// <returns>结果集合</returns>
        DataTable AccruedByPeriod(AccruedQueryFilter filter, out int totalCount);

        /// <summary>
        /// 应计返利报表查询（供应商）
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="totalCount">返回总记录数</param>
        /// <returns>结果集合</returns>
        DataTable AccruedByVendor(AccruedQueryFilter filter, out int totalCount);

        /// <summary>
        /// 应计返利报表查询（合同）
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="totalCount">返回总记录数</param>
        /// <returns>结果集合</returns>
        DataTable AccruedByRule(AccruedQueryFilter filter, out int totalCount);

        /// <summary>
        /// 应计返利报表查询（PM）
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="totalCount">返回总记录数</param>
        /// <returns>结果集合</returns>
        DataTable AccruedByPM(AccruedQueryFilter filter, out int totalCount);
    }
}
