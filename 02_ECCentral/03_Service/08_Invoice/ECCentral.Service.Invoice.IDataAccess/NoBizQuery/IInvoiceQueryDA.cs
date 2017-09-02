using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Invoice;

namespace ECCentral.Service.Invoice.IDataAccess.NoBizQuery
{
    public interface IInvoiceQueryDA
    {
        /// <summary>
        /// 销售-分公司查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataSet Qeury(InvoiceQueryFilter filter, out int totalCount);

        /// <summary>
        /// 对账单查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable ReconciliationQuery(ReconciliationQueryFilter filter, out int totalCount);
    }
}