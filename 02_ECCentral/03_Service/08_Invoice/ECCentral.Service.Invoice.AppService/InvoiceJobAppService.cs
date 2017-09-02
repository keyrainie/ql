using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Customer;
using ECCentral.Service.Invoice.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.AppService
{
    [VersionExport(typeof(InvoiceJobAppService))]
    public class InvoiceJobAppService
    {
        public void AdjustPointPreCheck(AdjustPointRequest request)
        {
            ObjectFactory<BalanceRefundProcessor>.Instance.AdjustPointPreCheck(request);
        }

        public void AdjustPoint(AdjustPointRequest request)
        {
            ObjectFactory<BalanceRefundProcessor>.Instance.AdjustPoint(request);
        }

        /// <summary>
        /// 同步对账单
        /// </summary>
        /// <param name="billType">
        /// 交易类型
        /// 1 交易对账
        /// 2 实扣税费对账
        /// 3 保证金对账
        /// 4 外币账户对账
        /// </param>
        /// <param name="date">日期，格式：yyyyMMdd</param>
        /// <returns></returns>
        public bool SyncTradeBill(string billType, string date)
        {
            return ObjectFactory<InvoiceProcessor>.Instance.SyncTradeBill(billType, date);
        }
    }
}
