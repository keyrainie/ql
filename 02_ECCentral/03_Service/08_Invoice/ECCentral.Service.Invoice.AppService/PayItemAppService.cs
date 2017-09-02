using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Invoice.BizProcessor;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.AppService
{
    /// <summary>
    /// 付款单应用层服务
    /// </summary>
    [VersionExport(typeof(PayItemAppService))]
    public class PayItemAppService
    {
        /// <summary>
        /// 创建付款单
        /// </summary>
        /// <param name="entity">待创建的付款单</param>
        /// <returns></returns>
        public virtual PayItemInfo Create(PayItemInfo entity)
        {
            if (entity.OrderType == null)
            {
                throw new ArgumentNullException("entity.OrderType");
            }
            if (entity.OrderSysNo == null)
            {
                throw new ArgumentNullException("entity.OrderSysNo");
            }

            var payItemInfo = ObjectFactory<PayItemProcessor>.Instance.Create(entity);
            //记录操作日志
            if (payItemInfo.SysNo.HasValue)
            {
                ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                     GetMessageString("PayItem_Log_Create", ServiceContext.Current.UserSysNo, payItemInfo.SysNo)
                    , BizLogType.Finance_Pay_Item_Add
                    , payItemInfo.SysNo.Value
                    , payItemInfo.CompanyCode);
            }

            return payItemInfo;
        }

        /// <summary>
        /// 更新付款单
        /// </summary>
        /// <param name="entity">待更新的付款单</param>
        public virtual void Update(PayItemInfo entity)
        {
            if (entity.SysNo == null)
            {
                throw new ArgumentNullException("entity.SysNo");
            }
            var payItemInfo = ObjectFactory<PayItemProcessor>.Instance.Update(entity);
            //记录操作日志
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                 GetMessageString("PayItem_Log_Update", ServiceContext.Current.UserSysNo, payItemInfo.SysNo)
                , BizLogType.Finance_Pay_Update
                , payItemInfo.SysNo.Value
                , payItemInfo.CompanyCode);
        }

        /// <summary>
        /// 作废付款单
        /// </summary>
        /// <param name="entity">待作废的付款单</param>
        public virtual void Abandon(PayItemInfo entity)
        {
            if (entity.SysNo == null)
            {
                throw new ArgumentNullException("entity.SysNo");
            }
            if (entity.OrderType == null)
            {
                throw new ArgumentNullException("entity.OrderType");
            }
            var payItemInfo = ObjectFactory<PayItemProcessor>.Instance.Abandon(entity);
            //记录操作日志
            //if (entity.OrderType == PayableOrderType.ReturnPointCashAdjust
            //    || entity.OrderType == PayableOrderType.SubAccount
            //    || entity.OrderType == PayableOrderType.SubInvoice              
            //)
            //{
            //    //TODO:取得EIMS用户编号
            //    var eimsUser = "==EIMS==";
            //    ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
            //         GetMessageString("PayItem_Log_Abandon_EIMS", eimsUser, payItemInfo.SysNo)
            //        , BizLogType.Finance_Pay_Item_Abandon
            //        , payItemInfo.SysNo.Value
            //       , payItemInfo.CompanyCode);
            //}
            //else
            //{
                ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                     GetMessageString("PayItem_Log_Abandon_EC", ServiceContext.Current.UserSysNo, payItemInfo.SysNo)
                    , BizLogType.Finance_Pay_Item_Abandon
                    , payItemInfo.SysNo.Value
                    , payItemInfo.CompanyCode);
            //}
        }

        /// <summary>
        /// 取消作废付款单
        /// </summary>
        /// <param name="entity">待取消作废的付款单</param>
        public virtual void CancelAbandon(PayItemInfo entity)
        {
            if (entity.SysNo == null)
            {
                throw new ArgumentNullException("entity.SysNo");
            }
            var payItemInfo = ObjectFactory<PayItemProcessor>.Instance.CancelAbandon(entity);
            //记录操作日志
            //if (entity.OrderType == PayableOrderType.ReturnPointCashAdjust
            //    || entity.OrderType == PayableOrderType.SubAccount
            //    || entity.OrderType == PayableOrderType.SubInvoice
            //)
            //{
            //    //TODO:取得EIMS用户编号
            //    var eimsUser = "==EIMS==";
            //    ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
            //         GetMessageString("PayItem_Log_CancelAbandon_EIMS", eimsUser, payItemInfo.SysNo)
            //        , BizLogType.Finance_Pay_Item_CancelAbandon
            //        , payItemInfo.SysNo.Value
            //       , payItemInfo.CompanyCode);
            //}
            //else
            {
                ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                     GetMessageString("PayItem_Log_CancelAbandon_EC", ServiceContext.Current.UserSysNo, payItemInfo.SysNo)
                    , BizLogType.Finance_Pay_Item_CancelAbandon
                    , payItemInfo.SysNo.Value
                    , payItemInfo.CompanyCode);
            }
        }

        /// <summary>
        /// 支付付款单
        /// </summary>
        /// <param name="entity">待支付的付款单</param>
        /// <param name="isForcePay">是否强制付款</param>
        public virtual void Pay(PayItemInfo entity, bool isForcePay)
        {
            if (entity.SysNo == null)
            {
                throw new ArgumentNullException("entity.SysNo");
            }

            var payItemInfo = ObjectFactory<PayItemProcessor>.Instance.Pay(entity, isForcePay);
            //记录操作日志
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                 GetMessageString("PayItem_Log_Pay", ServiceContext.Current.UserSysNo, payItemInfo.SysNo)
                , BizLogType.Finance_Pay_Item_Pay
                , payItemInfo.SysNo.Value
                , payItemInfo.CompanyCode);
        }

        /// <summary>
        /// 取消支付付款单
        /// </summary>
        /// <param name="entity">待取消支付的付款单</param>
        public virtual void CancelPay(PayItemInfo entity)
        {
            if (entity.SysNo == null)
            {
                throw new ArgumentNullException("entity.SysNo");
            }
            var payItemInfo = ObjectFactory<PayItemProcessor>.Instance.CancelPay(entity);
            //记录操作日志
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                 GetMessageString("PayItem_Log_CancelPay", ServiceContext.Current.UserSysNo, payItemInfo.SysNo)
                , BizLogType.Finance_Pay_Item_CancelPay
                , payItemInfo.SysNo.Value
                , payItemInfo.CompanyCode);
        }

        /// <summary>
        /// 锁定付款单
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Lock(PayItemInfo entity)
        {
            if (entity.SysNo == null)
            {
                throw new ArgumentNullException("entity.SysNo");
            }
            var payItemInfo = ObjectFactory<PayItemProcessor>.Instance.Lock(entity);
            //记录操作日志
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                 GetMessageString("PayItem_Log_Lock", ServiceContext.Current.UserSysNo, payItemInfo.SysNo)
                , BizLogType.Finance_Pay_Item_Lock
                , payItemInfo.SysNo.Value
                , payItemInfo.CompanyCode);
        }

        /// <summary>
        /// 取消锁定付款单
        /// </summary>
        /// <param name="entity"></param>
        public virtual void CancelLock(PayItemInfo entity)
        {
            if (entity.SysNo == null)
            {
                throw new ArgumentNullException("entity.SysNo");
            }
            var payItemInfo = ObjectFactory<PayItemProcessor>.Instance.CancelLock(entity);
            //记录业务日志
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                 GetMessageString("PayItem_Log_UnLock", ServiceContext.Current.UserSysNo, payItemInfo.SysNo)
                , BizLogType.Finance_Pay_Item_UnLock
                , payItemInfo.SysNo.Value
                , payItemInfo.CompanyCode);
        }

        /// <summary>
        /// 设置付款单凭证号
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="referenceID"></param>
        public virtual void SetReferenceID(PayItemInfo entity)
        {
            if (!entity.SysNo.HasValue)
            {
                throw new ArgumentException("entity.SysNO");
            }
            var payItem = ObjectFactory<PayItemProcessor>.Instance.SetReferenceID(entity.SysNo.Value, entity.ReferenceID);
            //记录业务日志
            ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                 GetMessageString("PayItem_Log_Update", ServiceContext.Current.UserSysNo, payItem.SysNo)
                , BizLogType.Finance_Pay_Item_Pay
                , payItem.SysNo.Value
                , payItem.CompanyCode);
        }

        /// <summary>
        /// 批量设置付款单凭证号
        /// </summary>
        /// <param name="payItemList"></param>
        /// <returns></returns>
        public virtual string BatchSetReferenceID(List<PayItemInfo> payItemList)
        {
            var request = payItemList.Select(s => new BatchActionItem<PayItemInfo>()
            {
                ID = s.SysNo.ToString(),
                Data = s
            }).ToList();

            var result = BatchActionManager.DoBatchAction(request, payItem => SetReferenceID(payItem));
            return result.PromptMessage;
        }

        /// <summary>
        /// 批量支付付款单
        /// </summary>
        /// <param name="payItemList"></param>
        /// <returns></returns>
        public virtual string BatchPay(List<PayItemInfo> payItemList)
        {
            var request = payItemList.Select(s => new BatchActionItem<PayItemInfo>()
            {
                ID = s.SysNo.ToString(),
                Data = s
            }).ToList();

            var result = BatchActionManager.DoBatchAction(request, payItem => Pay(payItem, false));
            return result.PromptMessage;
        }

        /// <summary>
        /// 取得多语言资源字符串
        /// </summary>
        /// <param name="msgKeyName"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private string GetMessageString(string msgKeyName, params object[] args)
        {
            return string.Format(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.PayItem, msgKeyName), args);
        }
    }
}