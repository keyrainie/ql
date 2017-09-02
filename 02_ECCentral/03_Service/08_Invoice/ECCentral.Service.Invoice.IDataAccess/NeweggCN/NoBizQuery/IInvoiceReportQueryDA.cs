using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.IDataAccess.NoBizQuery
{
    public interface IInvoiceReportQueryDA
    {
        /// <summary>
        /// 发票明细表查询
        /// </summary>
        DataTable InvoiceDetailReportQuery(InvoiceDetailReportQueryFilter filter, out int totalCount);

        /// <summary>
        /// 礼品卡发票明细表查询
        /// </summary>
        DataTable GiftInvoiceDetailReportQuery(GiftInvoiceDetaiReportQueryFilter filter, out int totalCount);

        /// <summary>
        /// 移仓单明细表查询
        /// </summary>
        DataTable ProductShiftDetailReportQuery(ProductShiftDetailReportQueryFilter filter, out int totalCount);

        /// <summary>
        ///  发票打印查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable InvoicePrintAllQuery(InvoicePrintAllQueryFilter filter, out int totalCount);

        /// <summary>
        ///  自印发票系统查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable InvoiceSelfPrintQuery(InvoiceSelfPrintQueryFilter filter, out int totalCount);

        /// <summary>
        ///  自印发票仓库查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        List<CodeNamePair> InvoiceSelfStockQuery();
    }
}