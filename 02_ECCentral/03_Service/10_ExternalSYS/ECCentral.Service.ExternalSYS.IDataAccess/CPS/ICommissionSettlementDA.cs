using System.Collections.Generic;
using ECCentral.BizEntity.ExternalSYS.CPS;

namespace ECCentral.Service.ExternalSYS.IDataAccess.CPS
{
    public interface ICommissionSettlementDA
    {
        /// <summary>
        /// 插入佣金结算单
        /// </summary>
        /// <param name="commissionSettlementInfo"></param>
        /// <returns></returns>
        void Insert(CommissionSettlementInfo commissionSettlementInfo);

        /// <summary>
        /// 根据结算单状态获取无兑现单的佣金结算单待处理用户列表
        /// </summary>
        /// <returns></returns>
        List<int> GetPendingCommissionSettlementUserList();

        /// <summary>
        /// 根据用户获取无兑现单的佣金结算单
        /// </summary>
        /// <returns></returns>
        List<CommissionSettlementInfo> GetPendingCommissionSettlementByUserSysNo(int userSysNo);

        /// <summary>
        /// 根据用户获取已结算单是为付款的金额
        /// </summary>
        /// <returns></returns>
        List<CommissionSettlementInfo> GetUnRequestCommissionSettlementList(int userSysNo);

        /// <summary>
        /// 更新佣金结算单
        /// CommissionAmt ConfirmCommissionAmt
        /// </summary>
        /// <param name="commissionSettlementInfo"></param>
        void Update(CommissionSettlementInfo commissionSettlementInfo);

        /// <summary>
        /// 更新佣金结算单
        /// CashRecordSysNo
        /// </summary>
        /// <param name="commissionSettlementSysNo"></param>
        /// <param name="cashRecordSysNo"></param>
        void UpdateCashRecord(int commissionSettlementSysNo, int cashRecordSysNo);
    }
}
