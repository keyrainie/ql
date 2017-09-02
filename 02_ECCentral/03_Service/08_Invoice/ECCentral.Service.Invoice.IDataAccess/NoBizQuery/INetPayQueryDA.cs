using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Invoice;

namespace ECCentral.Service.Invoice.IDataAccess.NoBizQuery
{
    public interface INetPayQueryDA
    {
        /// <summary>
        /// 查询netpay列表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataTable Query(NetPayQueryFilter query, out int totalCount);
    }
}