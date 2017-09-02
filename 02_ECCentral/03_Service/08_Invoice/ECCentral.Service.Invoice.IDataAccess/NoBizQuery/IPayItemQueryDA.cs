using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Invoice;

namespace ECCentral.Service.Invoice.IDataAccess.NoBizQuery
{
    public interface IPayItemQueryDA
    {
        /// <summary>
        /// 付款单查询界面的复杂查询
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        DataSet Query(PayItemQueryFilter filter, out int totalCount);

        /// <summary>
        /// 根据付款单系统编号对付款单进行检查查询
        /// </summary>
        /// <param name="paySysNo"></param>
        /// <returns></returns>
        DataTable SimpleQuery(int paySysNo);
    }
}