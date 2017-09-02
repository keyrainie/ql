using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.Invoice.Refund;
using ECCentral.Service.EventMessage.Invoice;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.BizProcessor
{
    [VersionExport(typeof(BalanceRefundProcessor))]
    public class BalanceRefundProcessor
    {
        private IBalanceRefundDA m_BalanceRefundDA = ObjectFactory<IBalanceRefundDA>.Instance;

        /// <summary>
        /// 创建顾客余额退款信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual BalanceRefundInfo Create(BalanceRefundInfo entity)
        {
            if (entity.ReturnPrepayAmt <= 0)
            {
                //throw new BizException("退款金额必须大于等于0。");
                ThrowBizException("BalanceRefund_RefundAmoutNeedMoreThan0");
            }
            if (!ExternalDomainBroker.ExistsCustomer(entity.CustomerSysNo.Value))
            {
                //throw new BizException(string.Format("编号为{0}的顾客不存在。", entity.CustomerSysNo));
                ThrowBizException("BalanceRefund_NotExsistCustomer", entity.CustomerSysNo);
            }

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = IsolationLevel.ReadUncommitted;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, options))
            {
                ExternalDomainBroker.AdjustCustomerPerpayAmount(entity.CustomerSysNo.Value, 0, -entity.ReturnPrepayAmt.Value, PrepayType.BalanceReturn, "余额账户转银行退款");

                entity.Status = BalanceRefundStatus.Origin;
                entity = m_BalanceRefundDA.Create(entity);

                //发送创建Message
                EventPublisher.Publish(new CreateBalanceRefundMessage()
                {
                    ReturnPrepaySysNo = entity.SysNo.Value,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });


                ts.Complete();
            }

            return entity;
        }

        /// <summary>
        /// 更新退款信息
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Update(BalanceRefundInfo entity)
        {
            m_BalanceRefundDA.Update(entity);
        }

        /// <summary>
        ///  根据退款编号加载退款信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual BalanceRefundInfo LoadBySysNo(int sysNo)
        {
            var balanceRefundInfo = m_BalanceRefundDA.Load(sysNo);
            if (balanceRefundInfo == null)
            {
                //throw new BizException(string.Format("单据[{0}]不存在", sysNo));
                ThrowBizException("BalanceRefund_BillNotExsist");
            }
            return balanceRefundInfo;
        }

        /// <summary>
        /// 财务审核
        /// </summary>
        /// <param name="auditInfo"></param>
        public virtual void FinConfirm(BalanceRefundAuditInfo auditInfo)
        {
            var balanceRefundInfo = LoadBySysNo(auditInfo.SysNo.Value);

           /* if (balanceRefundInfo.CreateUserSysNo == auditInfo.AuditUserSysNo)
            {
                throw new BizException(string.Format(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.NetPay, "NetPay_InputAndAuditUserCannotSame"), auditInfo.SysNo.Value));
            }*/

            if (balanceRefundInfo.Status != BalanceRefundStatus.CSConfirmed)
            {
                //throw new BizException("只有CS已经审核的单据才能审核.");
                ThrowBizException("BalanceRefund_AuditPassCanAudit");
            }

              TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = IsolationLevel.ReadUncommitted;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, options))
            {
                m_BalanceRefundDA.UpdateStatusForFinConfirm(auditInfo.SysNo.Value, auditInfo.AuditUserSysNo, BalanceRefundStatus.FinConfirmed);

                //发送财务审核完成Message
                EventPublisher.Publish(new BalanceRefundFinConfirmedMessage()
                {
                    ReturnPrepaySysNo = auditInfo.SysNo.Value,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });
                ts.Complete();
            }
        }

        /// <summary>
        /// 客服审核
        /// </summary>
        /// <param name="auditInfo"></param>
        public virtual void CSConfirm(BalanceRefundAuditInfo auditInfo)
        {
            var balanceRefundInfo = LoadBySysNo(auditInfo.SysNo.Value);

            /*if (balanceRefundInfo.CreateUserSysNo == auditInfo.AuditUserSysNo)
            {
                throw new BizException(string.Format(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.NetPay, "NetPay_InputAndAuditUserCannotSame"), auditInfo.SysNo.Value));
            }*/

            if (balanceRefundInfo.Status != BalanceRefundStatus.Origin)
            {
                //throw new BizException("单据状态不是待CS审核状态,CS审核失败.");
                ThrowBizException("BalanceRefund_NotWaitCSAuditStatus");
            }
            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = IsolationLevel.ReadUncommitted;
            options.Timeout = TimeSpan.FromMinutes(2);
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, options))
            {
                m_BalanceRefundDA.UpdateStatusForCSConfirm(auditInfo.SysNo.Value, auditInfo.AuditUserSysNo, BalanceRefundStatus.CSConfirmed);
                EventPublisher.Publish(new BalanceRefundCSConfirmedMessage()
                {
                    ReturnPrepaySysNo = auditInfo.SysNo.Value,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });
                ts.Complete();
            }
        }

        /// <summary>
        /// 取消审核
        /// </summary>
        /// <param name="entity"></param>
        public virtual void CancelConfirm(int sysNo)
        {
            var balanceRefundInfo = LoadBySysNo(sysNo);

            if (balanceRefundInfo.Status != BalanceRefundStatus.FinConfirmed)
            {
               // throw new BizException("只有财务已经审核的单据才能取消审核.");
                ThrowBizException("BalanceRefund_JustAuditPassCanCancel");
            }


            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = IsolationLevel.ReadUncommitted;
            options.Timeout = TimeSpan.FromMinutes(2);
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, options))
            {
                m_BalanceRefundDA.UpdateStatus(sysNo, BalanceRefundStatus.CSConfirmed);
                //发送CS审核通过message
                EventPublisher.Publish(new BalanceRefundCSConfirmedMessage()
                {
                    ReturnPrepaySysNo = sysNo,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo
                });
                ts.Complete();
            }
        }

        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="sysNo"></param>
        public virtual void Abandon(int sysNo)
        {
            var balanceRefundInfo = LoadBySysNo(sysNo);

            if (balanceRefundInfo.Status != BalanceRefundStatus.Origin && balanceRefundInfo.Status != BalanceRefundStatus.CSConfirmed)
            {
                //throw new BizException("只有待审核或客服审核通过的单据才能作废.");
                ThrowBizException("BalanceRefund_JustWaitOrAuditPassCanDeactive");
            }

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = IsolationLevel.ReadUncommitted;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, options))
            {
                ExternalDomainBroker.AdjustCustomerPerpayAmount(balanceRefundInfo.CustomerSysNo.Value,
                    0, balanceRefundInfo.ReturnPrepayAmt.Value, PrepayType.BalanceReturn, "作废余额账户转银行退款加余额");

                m_BalanceRefundDA.UpdateStatus(sysNo, BalanceRefundStatus.Abandon);

                //发送作废Message
                EventPublisher.Publish(new BalanceRefundAbandonedMessage()
                {
                    ReturnPrepaySysNo = sysNo,
                    CurrentUserSysNo = ServiceContext.Current.UserSysNo,
                    LastRefundStatus = (int)balanceRefundInfo.Status
                });

                ts.Complete();
            }
        }

        /// <summary>
        /// 设置凭证号
        /// </summary>
        /// <param name="entity"></param>
        public virtual void SetReferenceID(int sysNo, string referenceID)
        {
            m_BalanceRefundDA.SetReferenceID(sysNo, referenceID);
        }

        public virtual void UpdatePointExpiringDate(int obtainSysNo, DateTime expiredDate)
        {
            int result = m_BalanceRefundDA.UpdatePointExpiringDate(obtainSysNo, expiredDate);
            if (result == 0)
            {
                //throw new BizException(string.Format("编号【{0}】的积分记录不存在或积分不为正数", obtainSysNo));
                ThrowBizException("BalanceRefund_PointRecordNotExsist", obtainSysNo);
            }
        }

        public virtual object AdjustPoint(AdjustPointRequest adjustInfo)
        {
            return m_BalanceRefundDA.Adjust(adjustInfo);
        }

        public virtual object AdjustPointPreCheck(AdjustPointRequest adjustInfo)
        {
            return m_BalanceRefundDA.AdjustPointPreCheck(adjustInfo);
        }

        public virtual object SplitSOPointLog(int customerSysNo, BizEntity.SO.SOBaseInfo master, List<BizEntity.SO.SOBaseInfo> subSoList)
        {
            return m_BalanceRefundDA.SplitSOPointLog(customerSysNo, master, subSoList);
        }

        public virtual object CancelSplitSOPointLog(int customerSysNo, BizEntity.SO.SOBaseInfo master, List<BizEntity.SO.SOBaseInfo> subSoList)
        {
            return m_BalanceRefundDA.CancelSplitSOPointLog(customerSysNo, master, subSoList);
        }

        #region Helper Methods

        private void ThrowBizException(string msgKeyName, params object[] args)
        {
            throw new BizException(GetMessageString(msgKeyName, args));
        }

        private string GetMessageString(string msgKeyName, params object[] args)
        {
            return string.Format(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.BalanceRefund, msgKeyName), args);
        }

        #endregion Helper Methods
    }
}