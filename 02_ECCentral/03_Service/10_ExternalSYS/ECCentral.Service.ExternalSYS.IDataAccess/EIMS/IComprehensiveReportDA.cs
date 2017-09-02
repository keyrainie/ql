using System.Data;
using ECCentral.QueryFilter.ExternalSYS;

namespace ECCentral.Service.ExternalSYS.IDataAccess
{
    public interface IComprehensiveReportDA
    {
        /// <summary>
        /// EIMS单据查询
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="totalCount">返回总记录数</param>
        /// <returns>结果集合</returns>
        DataTable EIMSInvoiceQuery(EIMSInvoiceQueryFilter filter,out int totalCount);

        /// <summary>
        /// 合同与对应单据查询
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="totalCount">返回总记录数</param>
        /// <returns>结果集合</returns>
        DataTable UnbilledRuleListQuery(UnbilledRuleListQueryFilter filter, out int totalCount);

        /// <summary>
        /// 综合报表查询
        /// </summary>
        /// <param name="filter">查询条件集合</param>
        /// <param name="totalCount">返回总记录数</param>
        /// <returns>结果集合</returns>
        DataTable ComprehensiveQuery(EIMSComprehensiveQueryFilter filter, out int totalCount);
    }
}
