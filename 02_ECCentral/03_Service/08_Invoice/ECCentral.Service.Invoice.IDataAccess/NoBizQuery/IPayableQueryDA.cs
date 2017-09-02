using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.QueryFilter.Invoice;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.IDataAccess.NoBizQuery
{
    public interface IPayableQueryDA
    {
        DataTable QueryPayable(PayableQueryFilter query, out int totalCountout, out DataTable dtStatistical);

        /// <summary>
        /// 取得所有账期类型
        /// </summary>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        List<CodeNamePair> GetAllVendorPayTerms(string companyCode);
    }
}