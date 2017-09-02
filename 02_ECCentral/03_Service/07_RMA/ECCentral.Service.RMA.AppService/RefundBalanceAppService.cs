using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.RMA.BizProcessor;
using System.Data;

namespace ECCentral.Service.RMA.AppService
{
    [VersionExport(typeof(RefundBalanceAppService))]
    public class RefundBalanceAppService
    {
        #region Load
        /// <summary>
        /// 根据退款调整单系统编号获取退款单信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual RefundBalanceInfo LoadRefundBalanceBySysNo(int sysNo)
        {
            return ObjectFactory<RefundBalanceProcessor>.Instance.LoadRefundBalanceBySysNo(sysNo);
        }
        /// <summary>
        /// 根据退款单系统编号获取新退款调整单基本信息
        /// </summary>
        /// <param name="OrgRefundSysNo"></param>
        /// <returns></returns>
        public virtual RefundBalanceInfo LoadNewRefundBalanceByRefundSysNo(int refundSysNo)
        {
            return ObjectFactory<RefundBalanceProcessor>.Instance.LoadNewRefundBalanceByRefundSysNo(refundSysNo);
        }

        /// <summary>
        /// 根据退款单号获取Item列表（用作退款调整单的调整商品）
        /// </summary>
        /// <param name="OrgRefundSysNo"></param>
        /// <returns></returns>
        public virtual List<RefundItemInfo> LoadRefundItemListByRefundSysNo(int refundSysNo)
        {
            return ObjectFactory<RefundBalanceProcessor>.Instance.LoadRefundItemListByRefundSysNo(refundSysNo);
        }

        #endregion

        /// <summary>
        /// 创建RefundBalance
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual void Create(RefundBalanceInfo entity)
        {
             ObjectFactory<RefundBalanceProcessor>.Instance.CreateRefundBalance(entity);
        }
        /// <summary>
        /// 提交审核
        /// </summary>
        /// <param name="entity"></param>
        public virtual void SubmitAudit(RefundBalanceInfo entity)
        {
            ObjectFactory<RefundBalanceProcessor>.Instance.SubmitAudit(entity);
        }
        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Refund(RefundBalanceInfo entity)
        {
            ObjectFactory<RefundBalanceProcessor>.Instance.Refund(entity);
        }
        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Abandon(int sysNo)
        {
            ObjectFactory<RefundBalanceProcessor>.Instance.Abandon(sysNo);
        }
        
    }
}
