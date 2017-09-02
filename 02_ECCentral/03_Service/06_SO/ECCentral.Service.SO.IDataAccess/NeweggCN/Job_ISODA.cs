using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.SO.IDataAccess
{
    public partial interface ISODA
    {
        #region Job 联通合约机相关
        /// <summary>
        /// 取得待审核的联通结算订单编号
        /// </summary>
        /// <returns></returns>
        List<int> GetStatusIsOriginalBuyMobileSettlementSOSysNo();

        /// <summary>
        /// 取得状态为已完成的联通合约机订单编号
        /// </summary>
        /// <returns></returns>
        List<int> GetStatusIsCompleteUnicomFreeBuySOSysNo();
        #endregion
    }
}
