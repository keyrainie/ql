using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.BizEntity.ExternalSYS.CPS;

namespace ECCentral.Service.ExternalSYS.IDataAccess.CPS
{
    public interface ICommissionSettlementItemDA
    {
        /// <summary>
        /// 获取拥有订单的用户列表
        /// </summary>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        List<int> GetHavingOrderUserList(DateTime beginDate, DateTime endDate, FinanceStatus status);

        /// <summary>
        /// 根据用户编号获取结算单明细列表
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <param name="beginDate"></param>
        /// <param name="endDate"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        List<CommissionSettlementItemInfo> GetCommissionSettlementItemInfoListByUserSysNo(int userSysNo,
                                                                                          DateTime beginDate,
                                                                                          DateTime endDate,
                                                                                          FinanceStatus status);

        /// <summary>
        /// 更新结算单明细
        /// Status CommissionAmt CommissionSettlementSysNo
        /// </summary>
        /// <param name="commissionSettlementItemInfo"></param>
        void Update(CommissionSettlementItemInfo commissionSettlementItemInfo);
    }
}
