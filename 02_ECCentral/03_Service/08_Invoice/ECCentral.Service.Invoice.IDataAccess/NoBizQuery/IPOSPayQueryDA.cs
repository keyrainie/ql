using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Invoice;

namespace ECCentral.Service.Invoice.IDataAccess.NoBizQuery
{
    public interface IPOSPayQueryDA
    {
        /// <summary>
        /// 查询POS支付记录
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataSet QueryPOSPayConfirmList(POSPayQueryFilter query, out int totalCount);
    }
}