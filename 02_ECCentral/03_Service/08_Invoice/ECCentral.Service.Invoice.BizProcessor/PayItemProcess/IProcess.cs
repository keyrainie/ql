using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.BizProcessor.PayItemProcess
{
    public interface IProcess
    {
        /// <summary>
        /// 创建
        /// </summary>
        PayItemInfo Create(PayItemInfo entity);

        /// <summary>
        /// 作废
        /// </summary>
        PayItemInfo Abandon(PayItemInfo entity);

        /// <summary>
        /// 取消作废
        /// </summary>
        PayItemInfo CancelAbandon(PayItemInfo entity);

        /// <summary>
        /// 支付
        /// </summary>
        PayItemInfo Pay(PayItemInfo entity, bool isForcePay);

        /// <summary>
        /// 取消支付
        /// </summary>
        PayItemInfo CancelPay(PayItemInfo entity);

        /// <summary>
        /// 锁定
        /// </summary>
        PayItemInfo Lock(PayItemInfo entity);

        /// <summary>
        /// 取消锁定
        /// </summary>
        PayItemInfo CancelLock(PayItemInfo entity);
    }
}