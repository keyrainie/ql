using System.Data;
using ECCentral.QueryFilter.ExternalSYS;

namespace ECCentral.Service.ExternalSYS.IDataAccess
{
    public interface IReceivedReportDA
    {
        /// <summary>
        /// 年度收款报表查询
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="totalCount">返回总记录数</param>
        /// <returns>结果集合</returns>
        DataTable ReceiveByYearQuery(ReceivedReportQueryFilter filter, out int totalCount);

        /// <summary>
        /// 供应商对账单查询
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="totalCount">返回总记录数</param>
        /// <returns>结果集合</returns>
        DataTable ReceiveByVendorQuery(ReceivedReportQueryFilter filter, out int totalCount);

        /// <summary>
        /// 应收账单（单据）查询
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="totalCount">返回总记录数</param>
        /// <returns>结果集合</returns>
        DataTable ARReceiveQuery(ReceivedReportQueryFilter filter, out int totalCount);

        /// <summary>
        /// 应收账单明细
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="totalCount">返回总记录数</param>
        /// <returns>结果集合</returns>
        DataTable ARReceiveDetialsQuery(ReceivedReportQueryFilter filter, out int totalCount);
    }
}
