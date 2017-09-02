using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Invoice;

namespace ECCentral.Service.Invoice.IDataAccess.NoBizQuery
{
    public interface ISOIncomeQueryDA
    {
        DataSet Query(SOIncomeQueryFilter filter, out int totalCount);

        DataSet QuerySO(SOQueryFilter filter, out int totalCount);

        DataSet QueryROExport(SOIncomeQueryFilter filter);

        DataSet ExportQuerySO(SOQueryFilter filter);

        DataTable QuerySaleReceivables(SaleReceivablesQueryFilter request, out int totalCount);

        /// <summary>
        /// 运费报表
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable QuerySOFreightStatDetai(SOFreightStatDetailQueryFilter filter, out int totalCount);
    }
}