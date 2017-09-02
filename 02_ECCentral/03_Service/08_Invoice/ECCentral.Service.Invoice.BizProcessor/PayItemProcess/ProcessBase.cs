using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.BizProcessor.PayItemProcess
{
    [VersionExport(typeof(IProcess))]
    public abstract class ProcessBase : IProcess
    {
        #region Properties

        protected PayableProcessor PayableBizProcessor
        {
            get
            {
                return ObjectFactory<PayableProcessor>.Instance;
            }
        }

        protected PayItemProcessor PayItemBizProcessor
        {
            get
            {
                return ObjectFactory<PayItemProcessor>.Instance;
            }
        }

        #endregion Properties

        #region For Pay

        protected virtual PayItemInfo PreCheckForPay(PayItemInfo entity, out PayableInfo payableInfo)
        {
            var payItemInfo = PayItemBizProcessor.LoadBySysNo(entity.SysNo.Value);
            if (payItemInfo.Status != PayItemStatus.Origin)
            {
                ThrowBizException("PayItem_Pay_OnlyOriginCanPay");
            }

            payableInfo = PayableBizProcessor.LoadBySysNo(payItemInfo.PaySysNo.Value);
            //ECC移除应付款汇总表，没有相应的财务审核功能，移除此逻辑 by freegod 20130531
            //if (payableInfo.AuditStatus != PayableAuditStatus.Audited)
            //{
            //    ThrowBizException("PayItem_Pay_PayableAuditStatusNotMatchAudited");
            //}

            if (payableInfo.PayStatus == PayableStatus.FullPay)
            {
                ThrowBizException("PayItem_Pay_CannotPayForFullPay");
            }
            if (payableInfo.PayStatus == PayableStatus.Abandon)
            {
                ThrowBizException("PayItem_Pay_CannotPayForAbandon");
            }
            payItemInfo.OrderSysNo = payableInfo.OrderSysNo;
            payItemInfo.BatchNumber = payableInfo.BatchNumber;

            return payItemInfo;
        }

        /// <summary>
        /// 支付付款单
        /// </summary>
        /// <param name="payItemInfo">付款单</param>
        /// <param name="payableInfo">付款单对应的应付款</param>
        /// <param name="isForcePay">是否强制付款</param>
        /// <returns>支付后的付款单</returns>
        public virtual PayItemInfo Pay(PayItemInfo entity, bool isForcePay)
        {
            PayableInfo payableInfo = null;
            var payItemInfo = PreCheckForPay(entity, out payableInfo);

            BeforeProcessForPay(payItemInfo);

            //执行业务操作
            TransactionOptions option = new TransactionOptions();
            option.IsolationLevel = IsolationLevel.ReadUncommitted;
            option.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, option))
            {
                decimal alreadyPay = payItemInfo.PayAmt.Value + payableInfo.AlreadyPayAmt.Value;
                decimal remains = payableInfo.OrderAmt.Value - alreadyPay;

                if (remains == 0)
                {
                    payableInfo.PayStatus = PayableStatus.FullPay;
                    //对于一个PO单对应的付款状态为FullPay的应付款，如果这个PO单对应的应付款有存在付款状态为Origin的付款单，
                    //系统自动将这些付款单的付款状态从Origin置为Abandon
                    //if (payableInfo.OrderType != PayableOrderType.SubInvoice)
                    {
                        PayItemBizProcessor.SetAbandonOfFullPay(payItemInfo);
                    }
                }
                else if ((payableInfo.OrderAmt > 0 && remains > 0) ||
                          (payableInfo.OrderAmt < 0 && remains < 0))
                {
                    payableInfo.PayStatus = PayableStatus.PartlyPay;
                }
                else
                {
                    if (isForcePay)
                    {
                        payableInfo.PayStatus = PayableStatus.FullPay;
                        //对于一个PO单对应的付款状态为FullPay的应付款，如果这个PO单对应的应付款有存在付款状态为Origin的付款单，
                        //系统自动将这些付款单的付款状态从Origin置为Abandon
                        //if (payableInfo.OrderType != PayableOrderType.SubInvoice)
                        {
                            PayItemBizProcessor.SetAbandonOfFullPay(payItemInfo);
                        }
                    }
                    else
                    {
                        ThrowBizException("PayItem_Pay_CannotPayForMore");
                    }
                }
                payableInfo.AlreadyPayAmt += payItemInfo.PayAmt;
                PayableBizProcessor.UpdateStatusAndAlreadyPayAmt(payableInfo);

                payItemInfo.Status = PayItemStatus.Paid;
                payItemInfo.BankGLAccount = entity.BankGLAccount;
                payItemInfo = PayItemBizProcessor.UpdateStatus(payItemInfo);

                scope.Complete();
            }

            return payItemInfo;
        }

        /// <summary>
        ///  这个方法会调用其他Domain的Service，不能放在TransactionScope里面，否则会有分布式事务
        ///  用于处理返点信息
        /// </summary>
        /// <param name="item"></param>
        protected abstract void BeforeProcessForPay(PayItemInfo payItemInfo);

        #endregion For Pay

        #region For Create

        protected virtual void PreCheckForCreate(PayItemInfo payItemInfo)
        {
            if (payItemInfo.OrderType == null)
            {
                throw new ArgumentNullException("item.OrderType");
            }
            if (payItemInfo.OrderSysNo == null)
            {
                throw new ArgumentNullException("item.OrderSysNo");
            }
        }

        public virtual PayItemInfo Create(PayItemInfo entity)
        {
            PreCheckForCreate(entity);

            PayItemInfo payItemInfo = null;

            TransactionOptions option = new TransactionOptions();       
            option.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, option))
            {
                ProcessReferencePayableInfoForCreate(entity);
                if (entity.PaySysNo == 0)
                {
                    return entity;
                }
                entity.Status = PayItemStatus.Origin;
                entity.AvailableAmt = entity.PayAmt;

                payItemInfo = ObjectFactory<IPayItemDA>.Instance.Create(entity);

                scope.Complete();
            }

            //CRL18099:如果供商已锁定，自动锁定
            if (payItemInfo.SysNo > 0
                && PayItemBizProcessor.IsHoldByVendor(payItemInfo)
                && (payItemInfo.OrderType == PayableOrderType.PO || payItemInfo.OrderType == PayableOrderType.VendorSettleOrder || payItemInfo.OrderType == PayableOrderType.CollectionSettlement))
            {
                payItemInfo.EditUserSysNo = PayItemBizProcessor.GetSystemUserSysNo();
                PayItemBizProcessor.Lock(payItemInfo);
            }

            //CRL19793 如果PM 已锁定，自动锁定
            if (payItemInfo.SysNo > 0
                && PayItemBizProcessor.IsHoldByVendorPM(payItemInfo)
                && (payItemInfo.OrderType == PayableOrderType.PO || payItemInfo.OrderType == PayableOrderType.VendorSettleOrder))
            {
                payItemInfo.EditUserSysNo = PayItemBizProcessor.GetSystemUserSysNo();
                PayItemBizProcessor.Lock(payItemInfo);
            }

            //付款金额为零，自动支付
            if (payItemInfo.OrderType.HasValue
               && payItemInfo.SysNo > 0 && payItemInfo.PayAmt == 0)
            {
                PayItemBizProcessor.AutoPay(payItemInfo);
            }

            return payItemInfo;
        }

        /// <summary>
        /// 关联的应付款
        /// </summary>
        protected PayableInfo ReferencePayableInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 处理关联的应付款
        /// </summary>
        /// <param name="payItemInfo"></param>
        protected abstract void ProcessReferencePayableInfoForCreate(PayItemInfo payItemInfo);

        #endregion For Create

        #region For CancelAbandon

        protected virtual PayItemInfo PreCheckForCancelAbandon(PayItemInfo entity, out PayableInfo payableInfo)
        {
            var payItemInfo = PayItemBizProcessor.LoadBySysNo(entity.SysNo.Value);

            if (payItemInfo.Status != PayItemStatus.Abandon)
            {
                ThrowBizException("PayItem_CancelAbandon_StatusNotMatchAbandon");
            }

            payableInfo = PayableBizProcessor.LoadBySysNo(payItemInfo.PaySysNo.Value);
            if (payableInfo.PayStatus == PayableStatus.FullPay)
            {
                ThrowBizException("PayItem_CancelAbandon_FullPayCannotCancel");
            }

            payableInfo.PayItemList.Where(x => x.SysNo == entity.SysNo).First().Status = PayItemStatus.Paid;
            if (payableInfo.PayItemList.Where(x => x.Status != PayItemStatus.Abandon).Sum(x => x.PayAmt) > payableInfo.OrderAmt)
            {
                ThrowBizException("PayItem_CancelAbandon_PayItemTotalPayAmtCannotMoreThanPayOrderAmt");
            }

            return payItemInfo;
        }

        public virtual PayItemInfo CancelAbandon(PayItemInfo entity)
        {
            PayableInfo payableInfo = null;
            var payItemInfo = PreCheckForCancelAbandon(entity, out payableInfo);

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                payItemInfo.Status = PayItemStatus.Origin;
                payItemInfo = PayItemBizProcessor.UpdateStatus(payItemInfo);
                if (payableInfo.PayStatus == PayableStatus.Abandon)
                {
                    payableInfo.PayStatus = PayableStatus.UnPay;
                    payableInfo.AlreadyPayAmt = 0;
                    PayableBizProcessor.UpdateStatus(payableInfo);
                }
                scope.Complete();
            }

            AfterProcessForCancelAbandon(payItemInfo);

            return payItemInfo;
        }

        protected abstract void AfterProcessForCancelAbandon(PayItemInfo payItemInfo);

        #endregion For CancelAbandon

        #region For CancelPay

        protected virtual PayItemInfo PreCheckForCancelPay(PayItemInfo entity, out PayableInfo payableInfo)
        {
            var payItemInfo = PayItemBizProcessor.LoadBySysNo(entity.SysNo.Value);
            if (payItemInfo.Status != PayItemStatus.Paid)
            {
                ThrowBizException("PayItem_CancelPay_OnlyPaidCanCancelPay");
            }
            //payItemInfo.PayUserSysNo = entity.PayUserSysNo;
            payableInfo = PayableBizProcessor.LoadBySysNo(payItemInfo.PaySysNo.Value);
            payItemInfo.OrderSysNo = payableInfo.OrderSysNo;
            payItemInfo.BatchNumber = payableInfo.BatchNumber;

            if (payableInfo.PayStatus == PayableStatus.UnPay || payableInfo.PayStatus == PayableStatus.Abandon)
            {
                ThrowBizException("PayItem_CancelPay_CannotCancelPay");
            }
            return payItemInfo;
        }

        public virtual PayItemInfo CancelPay(PayItemInfo entity)
        {
            PayableInfo payableInfo = null;
            var payItemInfo = PreCheckForCancelPay(entity, out payableInfo);

            BeforeProcessForCancelPay(payItemInfo);

            TransactionOptions option = new TransactionOptions();
            option.IsolationLevel = IsolationLevel.ReadUncommitted;
            option.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, option))
            {
                decimal? alreadyPay = payableInfo.AlreadyPayAmt - payItemInfo.PayAmt;
                decimal? remains = payableInfo.OrderAmt - alreadyPay;

                if (alreadyPay == 0)
                {
                    payableInfo.PayStatus = PayableStatus.UnPay;
                }
                else if (remains == 0)
                {
                    payableInfo.PayStatus = PayableStatus.FullPay;
                }
                else if ((payableInfo.OrderAmt > 0 && remains > 0) ||
                    (payableInfo.OrderAmt < 0 && remains < 0))
                {
                    payableInfo.PayStatus = PayableStatus.PartlyPay;
                }
                else
                {
                    payableInfo.PayStatus = PayableStatus.PartlyPay;
                }
                payableInfo.AlreadyPayAmt -= payItemInfo.PayAmt;

                PayableBizProcessor.UpdateStatusAndAlreadyPayAmt(payableInfo);

                payItemInfo.Status = PayItemStatus.Origin;
                payItemInfo.BankGLAccount = string.Empty;
                payItemInfo = PayItemBizProcessor.UpdateStatus(payItemInfo);

                scope.Complete();
            }

            return payItemInfo;
        }

        /// <summary>
        /// 这个方法会调用其他Domain的Service，不能放在TransactionScope里面，否则会有分布式事务
        /// 用于处理返点信息
        /// </summary>
        /// <param name="item"></param>
        protected abstract void BeforeProcessForCancelPay(PayItemInfo payItemInfo);

        #endregion For CancelPay

        #region For Abandon

        protected virtual PayItemInfo PreCheckForAbandon(PayItemInfo entity)
        {
            var payItemInfo = PayItemBizProcessor.LoadBySysNo(entity.SysNo.Value);
            if (payItemInfo.Status != PayItemStatus.Origin)
            {
                ThrowBizException("PayItem_Abandon_StatusNotMatchOrigin");
            }
            return payItemInfo;
        }

        public virtual PayItemInfo Abandon(PayItemInfo entity)
        {
            var payItemInfo = PreCheckForAbandon(entity);

            TransactionOptions option = new TransactionOptions();
            option.IsolationLevel = IsolationLevel.ReadUncommitted;
            option.Timeout = TransactionManager.DefaultTimeout;
            var payList = PayableBizProcessor.GetListByCriteria(new PayableInfo
            {
                SysNo = payItemInfo.PaySysNo
            });
            bool isLastPayitem = PayItemBizProcessor.IsLastUnAbandon(payItemInfo);
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, option))
            {
                payItemInfo.Status = PayItemStatus.Abandon;
                payItemInfo.Note = payItemInfo.Note + entity.Note;
                PayItemBizProcessor.UpdateStatus(payItemInfo);
                //如果该应付款对应的所有付款单都是无效的， 就作废应付款
                if (isLastPayitem)
                {
                    foreach (var pay in payList)
                    {
                        pay.PayStatus = PayableStatus.Abandon;
                        PayableBizProcessor.UpdateStatus(pay);
                    }
                }
                scope.Complete();
            }
            return payItemInfo;
        }

        #endregion For Abandon

        #region For Lock

        protected virtual PayItemInfo PreCheckForLock(PayItemInfo entity, out PayableInfo payableInfo)
        {
            var payItemInfo = PayItemBizProcessor.LoadBySysNo(entity.SysNo.Value);
            if (payItemInfo.Status != PayItemStatus.Origin)
            {
                ThrowBizException("PayItem_Lock_StatusNotMatchOrigin");
            }

            payableInfo = PayableBizProcessor.LoadBySysNo(payItemInfo.PaySysNo.Value);
            //应付款是否为已作废或者已支付
            if (payableInfo.PayStatus == PayableStatus.Abandon)
            {
                ThrowBizException("PayItem_Lock_AbandonStatusCannotLock");
            }
            else if (payableInfo.PayStatus == PayableStatus.FullPay)
            {
                ThrowBizException("PayItem_Lock_FullPayStatusCannotLock");
            }
            return payItemInfo;
        }

        public PayItemInfo Lock(PayItemInfo entity)
        {
            PayableInfo payableInfo = null;
            var payItemInfo = PreCheckForLock(entity, out payableInfo);

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                payItemInfo.Status = PayItemStatus.Locked;
                //item.EditUser = entity.EditUser;
                payItemInfo = PayItemBizProcessor.UpdateStatusAndEditUser(payItemInfo);

                scope.Complete();
            }
            return payItemInfo;
        }

        #endregion For Lock

        #region For CancelLock

        protected virtual PayItemInfo PreCheckForCancelLock(PayItemInfo entity, out PayableInfo payableInfo)
        {
            var payItemInfo = PayItemBizProcessor.LoadBySysNo(entity.SysNo.Value);
            //付款状态是否为已锁定
            if (payItemInfo.Status != PayItemStatus.Locked)
            {
                ThrowBizException("PayItem_UnLock_StatusNotMatchLocked");
            }

            //校验应付款的供应商是否已锁定
            if (PayItemBizProcessor.IsHoldByVendor(payItemInfo))
            {
                ThrowBizException("PayItem_UnLock_CannotUnLockByVendor");
            }

            if (PayItemBizProcessor.IsHoldByVendorPM(payItemInfo))
            {
                ThrowBizException("PayItem_UnLock_CannotUnLockByPM");
            }

            payableInfo = PayableBizProcessor.LoadBySysNo(payItemInfo.PaySysNo.Value);
            //应付款是否为已作废或者已支付
            if (payableInfo.PayStatus == PayableStatus.Abandon)
            {
                ThrowBizException("PayItem_UnLock_AbandonStatusCannotUnLock");
            }
            else if (payableInfo.PayStatus == PayableStatus.FullPay)
            {
                ThrowBizException("PayItem_UnLock_FullPayStatusCannotUnLock");
            }
            return payItemInfo;
        }

        public virtual PayItemInfo CancelLock(PayItemInfo entity)
        {
            PayableInfo payableInfo = null;
            var payItemInfo = PreCheckForCancelLock(entity, out payableInfo);

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                payItemInfo.Status = PayItemStatus.Origin;
                //item.EditUser = entity.EditUser;
                payItemInfo = PayItemBizProcessor.UpdateStatusAndEditUser(payItemInfo);

                scope.Complete();
            }
            return payItemInfo;
        }

        #endregion For CancelLock

        #region Helper Methods

        protected void ThrowBizException(string msgKeyName, params object[] args)
        {
            string msg = string.Format(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.PayItem, msgKeyName), args);
            throw new BizException(msg);
        }

        #endregion Helper Methods
    }
}
