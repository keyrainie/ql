using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Invoice;

namespace ECCentral.Service.Invoice.IDataAccess
{
    public interface IFinancialReportDA
    {
        /// <summary>
        /// Table[0]: Result,
        /// Table[1]: Statistics
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataSet IncomeCostReportQuery(IncomeCostReportQueryFilter filter, out int totalCount);

        /// <summary>
        /// Table[0]: Result,
        /// Table[1]: Statistics
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataSet SalesStatisticsReportQuery(SalesStatisticsReportQueryFilter filter, out int totalCount);

        DataSet CouponUseedReportQuery(CouponUsedReportFilter filter, out int totalCount);
    }
}
