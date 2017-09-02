using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.RMA;
using System.Data;

namespace ECCentral.Service.RMA.IDataAccess
{
    public interface IRefundBalanceDA
    {
        RefundBalanceInfo LoadNewRefundBalanceByRefundSysNo(int refundSysNo);

        RefundBalanceInfo CreateRefundBalance(RefundBalanceInfo entity);

        void UpdateRefundBalance(RefundBalanceInfo entity);

        RefundItemInfo GetRefundTotalAmount(RefundBalanceInfo entity);

        RefundBalanceInfo GetRefundBalanceBySysNo(int sysNo);
    }
}
