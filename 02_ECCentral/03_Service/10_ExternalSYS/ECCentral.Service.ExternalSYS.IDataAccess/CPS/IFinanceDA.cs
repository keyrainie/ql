using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.QueryFilter.ExternalSYS;

namespace ECCentral.Service.ExternalSYS.IDataAccess
{
    public interface IFinanceDA
    {
        /// <summary>
        /// 得到所有结算单
        /// </summary>
        /// <returns></returns>
      DataTable GetAllFinancee(FinanceQueryFilter query, out int totalCount);
        
        /// <summary>
        /// 更新确认结算金额
        /// </summary>
        /// <param name="info"></param>
      void UpdateCommisonConfirmAmt(FinanceInfo info);
    }
}
