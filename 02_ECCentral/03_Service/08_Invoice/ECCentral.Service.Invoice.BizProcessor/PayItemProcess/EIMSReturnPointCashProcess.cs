using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Transactions;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.IBizInteract;

namespace ECCentral.Service.Invoice.BizProcessor.PayItemProcess
{
    /// <summary>
    /// 针对于EIMS返点的处理
    /// </summary>
    [VersionExport(typeof(IProcess), new string[] { "EIMSReturnPointCashProcess" })]
    [Export(typeof(ProcessBase))]
    public class EIMSReturnPointCashProcess : ProcessBase
    {
        #region For Pay

        protected override PayItemInfo PreCheckForPay(PayItemInfo entity, out PayableInfo payableInfo)
        {
            var payItemInfo = base.PreCheckForPay(entity, out payableInfo);

            //CRL19990 By Kilin
            //现金支付的必须有银行科目号
            //if (payableInfo.OrderType == PayableOrderType.ReturnPointCashAdjust)
            //{
            //    if (string.IsNullOrEmpty(entity.BankGLAccount))
            //    {
            //        ThrowBizException("PayItem_Pay_PayableBankGLAccountCanNotEmpty");
            //    }
            //    payItemInfo.BankGLAccount = entity.BankGLAccount;
            //}
            return payItemInfo;
        }

        protected override void BeforeProcessForPay(PayItemInfo payItemInfo)
        {
            string UserID = ExternalDomainBroker.GetUserInfoBySysNo(ServiceContext.Current.UserSysNo).UserID;
            //TODO:调用EIMS系统接口，支付付款单。
            var re = ExternaSystemBroker.EIMSPayPayItem(payItemInfo.SysNo.Value, payItemInfo.OrderSysNo.Value, payItemInfo.PayAmt.Value, payItemInfo.CompanyCode, UserID);
            if (!string.IsNullOrEmpty(re))
            {
                throw new ECCentral.BizEntity.BizException(re);
            }
        }

        #endregion For Pay

        #region For Create

        protected override void ProcessReferencePayableInfoForCreate(PayItemInfo entity)
        {
            var now = System.DateTime.Now;

            //TODO:为EIMS返点创建付款单时CompanyCode怎么取得。
            //entity.CompanyCode = "8601";
            List<PayableInfo> payList = PayableBizProcessor.GetListByCriteria(new PayableInfo
            {
                OrderSysNo = entity.OrderSysNo,
                OrderType = entity.OrderType,
                BatchNumber = entity.BatchNumber
            });

            if (payList != null && payList.Count > 0)
            {
                ReferencePayableInfo = payList[0];
                if (ReferencePayableInfo.PayStatus == PayableStatus.Abandon)
                {
                    ReferencePayableInfo.OrderAmt = entity.PayAmt;
                    ReferencePayableInfo.PayStatus = PayableStatus.UnPay;
                    ReferencePayableInfo.EIMSNo = entity.OrderSysNo.Value;
                    ReferencePayableInfo.EstimatedTimeOfPay = now;                  

                    PayableBizProcessor.UpdateStatusAndOrderAmt(ReferencePayableInfo);
                }
                entity.CurrencySysNo = ReferencePayableInfo.CurrencySysNo;
                entity.PaySysNo = ReferencePayableInfo.SysNo.Value;
                entity.OrderType = ReferencePayableInfo.OrderType;
                entity.OrderSysNo = ReferencePayableInfo.OrderSysNo;
            }
            else
            {
                ReferencePayableInfo = new PayableInfo();
                ReferencePayableInfo.OrderSysNo = entity.OrderSysNo.Value;
                ReferencePayableInfo.OrderType = entity.OrderType.Value;
                ReferencePayableInfo.AlreadyPayAmt = 0;
                ReferencePayableInfo.OrderAmt = entity.PayAmt;
                ReferencePayableInfo.CurrencySysNo = 1;// 人民币类型
                ReferencePayableInfo.PayStatus = PayableStatus.UnPay;
                ReferencePayableInfo.InvoiceStatus = PayableInvoiceStatus.Absent;
                ReferencePayableInfo.AuditStatus = PayableAuditStatus.NotAudit;
                ReferencePayableInfo.InvoiceFactStatus = PayableInvoiceFactStatus.Corrent;
                ReferencePayableInfo.EIMSNo = entity.OrderSysNo.Value;
                ReferencePayableInfo.Note = "Auto created by system!";
                ReferencePayableInfo.EstimatedTimeOfPay = now;
                ReferencePayableInfo.CompanyCode = entity.CompanyCode;

                //CRL18977 Added:冲销付款单时，需记录批次号
                ReferencePayableInfo.BatchNumber = entity.BatchNumber;

                //if (ReferencePayableInfo.OrderType == PayableOrderType.SubInvoice) //新建时如果是票扣则ETP不填
                //{
                //    ReferencePayableInfo.EstimatedTimeOfPay = null;
                //}

                ReferencePayableInfo = PayableBizProcessor.Create(ReferencePayableInfo);

                entity.PaySysNo = ReferencePayableInfo.SysNo.Value;
                entity.CurrencySysNo = ReferencePayableInfo.CurrencySysNo;
                entity.OrderType = ReferencePayableInfo.OrderType;
                entity.OrderSysNo = ReferencePayableInfo.OrderSysNo;
            }
        }

        #endregion For Create

        #region For CancelAbandon

        public override PayItemInfo CancelAbandon(PayItemInfo entity)
        {
            PayableInfo payableInfo = null;
            var payItemInfo = base.PreCheckForCancelAbandon(entity, out payableInfo);

            TransactionOptions options = new TransactionOptions();
            options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
            options.Timeout = TransactionManager.DefaultTimeout;
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, options))
            {
                payItemInfo.Status = PayItemStatus.Origin;
                payItemInfo.Note = payItemInfo.Note + entity.Note;
                payItemInfo = PayItemBizProcessor.UpdateStatus(payItemInfo);

                payableInfo.PayStatus = PayableStatus.UnPay;
                PayableBizProcessor.UpdateStatus(payableInfo);

                scope.Complete();
            }
            return payItemInfo;
        }

        protected override void AfterProcessForCancelAbandon(PayItemInfo entity)
        {
            //DO NOTHING
        }

        #endregion For CancelAbandon

        #region CancelPay

        protected override PayItemInfo PreCheckForCancelPay(PayItemInfo entity, out PayableInfo payableInfo)
        {
            var payItemInfo = base.PreCheckForCancelPay(entity, out payableInfo);

            //CRL19990 By Kilin
            //现金，取消支付，清除银行
            //if (payableInfo.OrderType == PayableOrderType.ReturnPointCashAdjust)
            //{
            //    string BankGLAccount = payItemInfo.BankGLAccount;
            //    if (!string.IsNullOrEmpty(BankGLAccount))
            //    {
            //        string note = string.Format("{0}取消支付前银行科目:{1};", payItemInfo.Note ?? string.Empty, BankGLAccount);
            //        if (note.Length <= 200)
            //        {
            //            payItemInfo.Note = note;
            //        }
            //        payItemInfo.BankGLAccount = null;
            //    }
            //}
            return payItemInfo;
        }

        protected override void BeforeProcessForCancelPay(PayItemInfo payItemInfo)
        {
            string UserID = ExternalDomainBroker.GetUserInfoBySysNo(ServiceContext.Current.UserSysNo).UserID;
            //TODO:调用EIMS系统接口，取消支付付款单。
            var re = ExternaSystemBroker.EIMSCancelPayItem(payItemInfo.SysNo.Value, payItemInfo.OrderSysNo.Value, payItemInfo.CompanyCode, UserID);
            if (!string.IsNullOrEmpty(re))
            {
                throw new ECCentral.BizEntity.BizException(re);
            }
        }

        #endregion CancelPay

        #region For Abandon

        protected override PayItemInfo PreCheckForAbandon(PayItemInfo entity)
        {
           

            return base.PreCheckForAbandon(entity);
        }

        #endregion For Abandon

        #region For Lock

        protected override PayItemInfo PreCheckForLock(PayItemInfo entity, out PayableInfo payableInfo)
        {
            throw new ECCentral.BizEntity.BizException(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.PayItem, "PayItem_Lock_InvalidOrderType"));
        }

        #endregion For Lock

        #region For CancelLock

        protected override PayItemInfo PreCheckForCancelLock(PayItemInfo entity, out PayableInfo payableInfo)
        {
            throw new ECCentral.BizEntity.BizException(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.PayItem, "PayItem_UnLock_InvalidOrderType"));
        }

        #endregion For CancelLock
    }
}