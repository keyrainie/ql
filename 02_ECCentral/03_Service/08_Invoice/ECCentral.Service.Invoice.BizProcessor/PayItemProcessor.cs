using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Invoice;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Invoice.BizProcessor.PayItemProcess;
using ECCentral.Service.Invoice.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.Invoice.BizProcessor
{
    [VersionExport(typeof(PayItemProcessor))]
    public class PayItemProcessor
    {
        private IPayItemDA m_PayItemDA = ObjectFactory<IPayItemDA>.Instance;

        /// <summary>
        /// 创建收款单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual PayItemInfo Create(PayItemInfo entity)
        {
            IProcess processor = PayItemProcessFactory.Get(entity.OrderType.Value);
            return processor.Create(entity);
        }

        /// <summary>
        /// 更新收款单
        /// </summary>
        /// <param name="entity"></param>
        public virtual PayItemInfo Update(PayItemInfo entity)
        {
            return m_PayItemDA.Update(entity);
        }

        /// <summary>
        /// 作废付款单
        /// </summary>
        /// <param name="entity"></param>
        public virtual PayItemInfo Abandon(PayItemInfo entity)
        {
            IProcess processor = PayItemProcessFactory.Get(entity.OrderType.Value);
            return processor.Abandon(entity);
        }

        /// <summary>
        /// 取消作废付款单
        /// </summary>
        /// <param name="entity"></param>
        public virtual PayItemInfo CancelAbandon(PayItemInfo entity)
        {
            IProcess processor = PayItemProcessFactory.Get(entity.OrderType.Value);
            return processor.CancelAbandon(entity);
        }

        /// <summary>
        /// 支付付款单
        /// </summary>
        /// <param name="entity"></param>
        public virtual PayItemInfo Pay(PayItemInfo entity, bool isForcePay)
        {
            IProcess processor = PayItemProcessFactory.Get(entity.OrderType.Value);
            return processor.Pay(entity, isForcePay);
        }

        /// <summary>
        /// 取消支付付款单
        /// </summary>
        /// <param name="entity"></param>
        public virtual PayItemInfo CancelPay(PayItemInfo entity)
        {
            IProcess processor = PayItemProcessFactory.Get(entity.OrderType.Value);
            return processor.CancelPay(entity);
        }

        /// <summary>
        /// 锁定付款单
        /// </summary>
        /// <param name="entity"></param>
        public virtual PayItemInfo Lock(PayItemInfo entity)
        {
            IProcess processor = PayItemProcessFactory.Get(entity.OrderType.Value);
            return processor.Lock(entity);
        }

        /// <summary>
        /// 取消锁定付款单
        /// </summary>
        /// <param name="entity"></param>
        public virtual PayItemInfo CancelLock(PayItemInfo entity)
        {
            IProcess processor = PayItemProcessFactory.Get(entity.OrderType.Value);
            return processor.CancelLock(entity);
        }

        /// <summary>
        /// 自动支付付款单
        /// </summary>
        /// <param name="entity"></param>
        public virtual void AutoPay(PayItemInfo entity)
        {
            if (!entity.SysNo.HasValue || entity.SysNo == 0)
            {
                throw new ArgumentNullException("entity.SysNo");
            }

            if (!entity.PaySysNo.HasValue || entity.PaySysNo == 0)
            {
                throw new ArgumentNullException("entity.PaySysNo");
            }

            //获取系统用户编号
            int sysUserSysNo = GetSystemUserSysNo();

            //更新应付款的状态为“财务已审核”
            PayableInfo payableInfo = new PayableInfo();
            payableInfo.SysNo = entity.PaySysNo;
            payableInfo.AuditUserSysNo = sysUserSysNo;
            payableInfo.AuditStatus = PayableAuditStatus.Audited;
            ObjectFactory<PayableProcessor>.Instance.UpdateAuditInfo(payableInfo);

            //TODO:自动支付不是强制支付（待确认）
            entity.PayUserSysNo = sysUserSysNo;
            Pay(entity, false);
        }

        /// <summary>
        /// 取得系统用户编号
        /// </summary>
        /// <returns></returns>
        public virtual int GetSystemUserSysNo()
        {
            return m_PayItemDA.GetSystemUserSysNo();
        }

        /// <summary>
        /// 设置凭证号
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="referenceID"></param>
        public virtual PayItemInfo SetReferenceID(int sysNo, string referenceID)
        {
            return m_PayItemDA.SetReferenceID(sysNo, referenceID);
        }

        /// <summary>
        /// 更新付款单状态
        /// </summary>
        /// <param name="entity"></param>
        public virtual PayItemInfo UpdateStatus(PayItemInfo entity)
        {
            return m_PayItemDA.UpdateStatus(entity);
        }

        /// <summary>
        /// 更新付款单状态和编辑人
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual PayItemInfo UpdateStatusAndEditUser(PayItemInfo entity)
        {
            return m_PayItemDA.UpdateStatusAndEditUser(entity);
        }

        /// <summary>
        /// 根据查询条件取得付款单列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual List<PayItemInfo> GetListByCriteria(PayItemInfo query)
        {
            return m_PayItemDA.GetListByCriteria(query);
        }

        /// <summary>
        /// 根据付款单编号取得付款单
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public virtual PayItemInfo LoadBySysNo(int sysNo)
        {
            var entity = m_PayItemDA.Load(sysNo);
            if (entity == null)
            {
                ThrowBizException("PayItem_RecordNotExist", sysNo);
            }
            return entity;
        }

        /// <summary>
        /// 对于一个PO单对应的付款状态为FullPay的应付款，如果这个PO单对应的应付款有存在付款状态为Origin的付款单，
        /// 系统自动将这些付款单的付款状态从Origin置为Abandon
        /// </summary>
        /// <param name="entity"></param>
        public virtual void SetAbandonOfFullPay(PayItemInfo entity)
        {
            m_PayItemDA.SetAbandonOfFullPay(entity);
        }

        /// <summary>
        /// 建立应付款和付款单的关联,写付款单扩展信息表Finance_Pay_Ex
        /// </summary>
        /// <param name="entity"></param>
        public virtual void CreatePayEx(PayItemInfo entity)
        {
            m_PayItemDA.CreatePayEx(entity);
        }

        /// <summary>
        /// 付款单是否被供应商PM锁定
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool IsHoldByVendorPM(PayItemInfo entity)
        {
            var pay = ObjectFactory<PayableProcessor>.Instance.LoadBySysNo(entity.PaySysNo.Value);
            bool result = false;
            if (entity.OrderType == PayableOrderType.PO)
            {
                result = ExternalDomainBroker.IsHolderVendorPMByPOSysNo(pay.OrderSysNo.Value);
            }
            else if (entity.OrderType == PayableOrderType.VendorSettleOrder)
            {
                result = ExternalDomainBroker.IsHolderVendorPMByVendorSettleSysNo(pay.OrderSysNo.Value);
            }

            return result;
        }

        /// <summary>
        /// 付款单是否被供应商锁定
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool IsHoldByVendor(PayItemInfo entity)
        {
            var pay = ObjectFactory<PayableProcessor>.Instance.LoadBySysNo(entity.PaySysNo.Value);
            bool result = false;
            if (entity.OrderType == PayableOrderType.PO)
            {
                result = ExternalDomainBroker.IsHolderVendorByPOSysNo(pay.OrderSysNo.Value);
            }
            else if (entity.OrderType == PayableOrderType.VendorSettleOrder)
            {
                result = ExternalDomainBroker.IsHolderVendorByVendorSettleSysNo(pay.OrderSysNo.Value);
            }
            else if (entity.OrderType == PayableOrderType.CollectionSettlement)
            {
                result = ExternalDomainBroker.IsHolderVendorByCollectionSettlementSysNo(pay.OrderSysNo.Value);
            }
            return result;
        }

        /// <summary>
        /// 是否是最后一个未作废的付款单
        /// </summary>
        /// <param name="payItemInfo"></param>
        /// <returns></returns>
        public virtual bool IsLastUnAbandon(PayItemInfo payItemInfo)
        {
            return m_PayItemDA.IsLastUnAbandon(payItemInfo);
        }

        #region [For PO Domain]

        public virtual void InsertPayItemInfo(PayItemInfo info)
        {
            m_PayItemDA.InsertPayItemInfo(info);
        }

        public virtual PayItemInfo GetFinancePayItemInfoByPOSysNo(int poSysNo)
        {
            return m_PayItemDA.GetPayItemInfoByPOSysNo(poSysNo);
        }

        /// <summary>
        /// 供应商锁定或取消锁定付款单
        /// </summary>
        /// <param name="vendorSysNo">供应商系统编号</param>
        /// <param name="isLock">是否锁定，true为锁定</param>
        /// <returns>对付款单处理成功的记录数</returns>
        public virtual int LockOrUnLockByVendor(int vendorSysNo, bool isLock)
        {
            int successOperNum;
            //根据供应商编号查找未支付的应付款（包括PO、代销结算单和代收结算单）
            var unPayList = GetUnPayListByVendorSysNo(vendorSysNo);

            //没有可以Action的应付款
            if (unPayList == null || unPayList.Count == 0)
            {
                successOperNum = 0;
                return successOperNum;
            }

            //根据应付款获取要操作的PayItem
            var operPayItems = new List<PayItemInfo>();
            //锁定动作：根据供应商编号查询“待处理”的付款单
            if (isLock)
            {
                operPayItems = GetListByVendorSysNo(vendorSysNo, PayItemStatus.Origin);
            }
            //取消锁定动作：根据供应商编号查询“已锁定”的付款单
            else
            {
                operPayItems = GetListByVendorSysNo(vendorSysNo, PayItemStatus.Locked);
            }

            //没有可以Action的付款单
            if (operPayItems.Count == 0)
            {
                successOperNum = 0;
                return successOperNum;
            }

            //取收款单上的companycode
            string companyCode = operPayItems[0].CompanyCode;

            //锁定或取消锁定Action
            var successOperItems = LockOrUnLockBySysNoList(operPayItems.Select(s => s.SysNo.Value).ToList(), isLock);

            //更新的记录数
            successOperNum = successOperItems.Count;

            if (isLock)
            {
                //写lock日志
                ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                    GetMessageString("PayItem_Log_VendorLock", ServiceContext.Current.UserSysNo, vendorSysNo, string.Join(",", successOperItems.Select(s => s.SysNo)))
                    , BizLogType.Finance_Pay_Item_Lock
                    , vendorSysNo
                    , companyCode);
            }
            else
            {
                //写unlock日志
                ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                    GetMessageString("PayItem_Log_VendorUnLock", ServiceContext.Current.UserSysNo, vendorSysNo, string.Join(",", successOperItems.Select(s => s.SysNo)))
                    , BizLogType.Finance_Pay_Item_UnLock
                    , vendorSysNo
                    , companyCode);
            }

            return successOperNum;
        }

        /// <summary>
        /// 供应商PM锁定或取消锁定付款单
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <param name="isLock"></param>
        /// <param name="holdPMSysNoList"></param>
        /// <param name="unHoldPMSysNoList"></param>
        /// <returns></returns>
        public virtual List<int> LockOrUnlockByVendorPM(int vendorSysNo, bool isLock, List<int> holdPMSysNoList, List<int> unHoldPMSysNoList)
        {
            List<int> result = new List<int>(2);
            result.Add(0);
            result.Add(0);

            //根据供应商编号查找未支付的应付款（包括PO、代销结算单）
            var unPayHoldList = GetUnPayListByVendorPMSysNo(vendorSysNo, holdPMSysNoList);
            var unPayUnHoldList = GetUnPayListByVendorPMSysNo(vendorSysNo, unHoldPMSysNoList);
            int willUpdateCount = 0;

            if (unPayHoldList != null)
            {
                willUpdateCount += unPayHoldList.Count;
            }
            if (unPayUnHoldList != null)
            {
                willUpdateCount += unPayUnHoldList.Count;
            }
            //没有可以Action的应付款
            if (willUpdateCount == 0)
            {
                return result;
            }

            //根据应付款获取要操作的PayItem
            var operPayItemsHold = new List<PayItemInfo>();
            var operPayItemsUnHold = new List<PayItemInfo>();

            //锁定动作：根据供应商编号查询“待处理”的付款单
            if (holdPMSysNoList != null && holdPMSysNoList.Count > 0)
            {
                operPayItemsHold = GetListByVenderPMSysNo(vendorSysNo, PayItemStatus.Origin, holdPMSysNoList);
            }
            //取消锁定动作：根据供应商编号查询“已锁定”的付款单
            if (unHoldPMSysNoList != null && unHoldPMSysNoList.Count > 0)
            {
                operPayItemsUnHold = GetListByVenderPMSysNo(vendorSysNo, PayItemStatus.Locked, unHoldPMSysNoList);
            }

            //更新锁定
            List<int> holdItemSysNoList = operPayItemsHold.Select(p => p.SysNo.Value).ToList();
            List<PayItemInfo> lockProcessPayItems = LockOrUnLockBySysNoList(holdItemSysNoList, true);

            //更新解锁
            List<int> unHoldItemSysNoList = operPayItemsUnHold.Select(p => p.SysNo.Value).ToList();
            List<PayItemInfo> unLockProcessPayItems = LockOrUnLockBySysNoList(unHoldItemSysNoList, false);

            //写lock日志
            if (lockProcessPayItems != null && lockProcessPayItems.Count > 0)
            {
                result[0] = lockProcessPayItems.Count;

                ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                    GetMessageString("PayItem_Log_VendorLock", ServiceContext.Current.UserSysNo, vendorSysNo, string.Join(",", lockProcessPayItems.Select(s => s.SysNo)))
                    , BizLogType.Finance_Pay_Item_Lock
                    , vendorSysNo
                    , lockProcessPayItems[0].CompanyCode);
            }
            //写unlock日志
            if (unLockProcessPayItems != null && unLockProcessPayItems.Count > 0)
            {
                result[1] = unLockProcessPayItems.Count;
                ObjectFactory<ICommonBizInteract>.Instance.CreateOperationLog(
                   GetMessageString("PayItem_Log_VendorUnLock", ServiceContext.Current.UserSysNo, vendorSysNo, string.Join(",", unLockProcessPayItems.Select(s => s.SysNo)))
                   , BizLogType.Finance_Pay_Item_UnLock
                   , vendorSysNo
                   , unLockProcessPayItems[0].CompanyCode);
            }

            return result;
        }

        /// <summary>
        /// 根据供应商编号取得未支付的应付款（包括PO、代销结算单、代收结算单）
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        protected virtual List<PayableInfo> GetUnPayListByVendorSysNo(int vendorSysNo)
        {
            var unpayList = ObjectFactory<PayableProcessor>.Instance.GetUnPayOrPartlyPayList();

            //调用PO接口取得PO单系统编号列表
            var poSysNoList = ExternalDomainBroker.GetPOSysNoListByVendorSysNo(vendorSysNo, null);

            //调用PO接口取得代销结算单系统编号列表
            var vendorSettleSysNoList = ExternalDomainBroker.GetVendorSettleSysNoListByVendorSysNo(vendorSysNo, null);

            //调用PO接口取得代收结算单系统编号列表
            var collectionSettleSysNoList = ExternalDomainBroker.GetCollectionSettlementSysNoListByVendorSysNo(vendorSysNo);

            unpayList = unpayList.Where(w => (w.OrderType == PayableOrderType.PO && poSysNoList.Contains(w.OrderSysNo.Value))
                || (w.OrderType == PayableOrderType.VendorSettleOrder && vendorSettleSysNoList.Contains(w.OrderSysNo.Value))
                || (w.OrderType == PayableOrderType.CollectionSettlement && collectionSettleSysNoList.Contains(w.OrderSysNo.Value)))
            .ToList();

            return unpayList;
        }

        /// <summary>
        /// 根据供应商编号查找未支付的应付款（包括PO、代销结算单）
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <param name="pmSysNoList"></param>
        /// <returns></returns>
        protected virtual List<PayableInfo> GetUnPayListByVendorPMSysNo(int vendorSysNo, List<int> pmSysNoList)
        {
            var unpayList = ObjectFactory<PayableProcessor>.Instance.GetUnPayOrPartlyPayList();
            unpayList.RemoveAll(r => r.OrderType == PayableOrderType.CollectionSettlement);

            //调用PO接口取得PO单系统编号列表
            var poSysNoList = ExternalDomainBroker.GetPOSysNoListByVendorSysNo(vendorSysNo, pmSysNoList);

            //调用PO接口取得代销结算单系统编号列表
            var vendorSettleSysNoList = ExternalDomainBroker.GetVendorSettleSysNoListByVendorSysNo(vendorSysNo, pmSysNoList);

            unpayList = unpayList.Where(w => (w.OrderType == PayableOrderType.PO && poSysNoList.Contains(w.OrderSysNo.Value))
                || (w.OrderType == PayableOrderType.VendorSettleOrder && vendorSettleSysNoList.Contains(w.OrderSysNo.Value)))
            .ToList();

            return unpayList;
        }

        /// <summary>
        /// 锁定Or取消锁定付款单
        /// </summary>
        /// <param name="payItemSysNoList">付款单系统编号列表</param>
        /// <param name="isLock">锁定（true）；取消锁定（false）</param>
        /// <returns>成功执行的付款单数量</returns>
        protected virtual List<PayItemInfo> LockOrUnLockBySysNoList(List<int> payItemSysNoList, bool isLock)
        {
            PayItemStatus status = isLock ? PayItemStatus.Locked : PayItemStatus.Origin;
            return m_PayItemDA.LockOrUnLockBySysNoList(payItemSysNoList, status);
        }

        /// <summary>
        /// 根据供应商系统编号、付款单状态和供应商PM编号列表取得付款单列表（包括PO、代销结算单）
        /// </summary>
        /// <param name="vendorSysNo">供应商系统编号</param>
        /// <param name="status">付款单状态</param>
        /// <param name="pmSysNoList">供应商PM系统编号列表</param>
        /// <returns></returns>
        protected virtual List<PayItemInfo> GetListByVenderPMSysNo(int vendorSysNo, PayItemStatus status, List<int> pmSysNoList)
        {
            //取得所有满足状态Status的PO单、代销结算单
            var payItemList = m_PayItemDA.GetListByStatus(status);
            payItemList.RemoveAll(r => r.OrderType == PayableOrderType.CollectionSettlement);

            //调用PO接口取得PO单系统编号列表
            var poSysNoList = ExternalDomainBroker.GetPOSysNoListByVendorSysNo(vendorSysNo, pmSysNoList);

            //调用PO接口取得代销结算单系统编号列表
            var vendorSettleSysNoList = ExternalDomainBroker.GetVendorSettleSysNoListByVendorSysNo(vendorSysNo, pmSysNoList);

            payItemList = payItemList.Where(w => (w.OrderType == PayableOrderType.PO && poSysNoList.Contains(w.OrderSysNo.Value))
                || (w.OrderType == PayableOrderType.VendorSettleOrder && vendorSettleSysNoList.Contains(w.OrderSysNo.Value)))
            .ToList();

            return payItemList;
        }

        /// <summary>
        /// 根据供应商系统编号、付款单状态取得付款单列表（包括PO、代销结算单、代收结算单）
        /// </summary>
        /// <param name="vendorSysNo">供应商系统编号</param>
        /// <param name="status">付款单状态</param>
        /// <returns>满足条件的付款单列表</returns>
        protected virtual List<PayItemInfo> GetListByVendorSysNo(int vendorSysNo, PayItemStatus status)
        {
            //取得所有满足状态Status的PO单、代销结算单、代收结算单的付款单列表
            var payItemList = m_PayItemDA.GetListByStatus(status);

            //调用PO接口取得PO单系统编号列表
            var poSysNoList = ExternalDomainBroker.GetPOSysNoListByVendorSysNo(vendorSysNo, null);

            //调用PO接口取得代销结算单系统编号列表
            var vendorSettleSysNoList = ExternalDomainBroker.GetVendorSettleSysNoListByVendorSysNo(vendorSysNo, null);

            //调用PO接口取得代收结算单系统编号列表
            var collectionSettleSysNoList = ExternalDomainBroker.GetCollectionSettlementSysNoListByVendorSysNo(vendorSysNo);

            payItemList = payItemList.Where(w => (w.OrderType == PayableOrderType.PO && poSysNoList.Contains(w.OrderSysNo.Value))
                || (w.OrderType == PayableOrderType.VendorSettleOrder && vendorSettleSysNoList.Contains(w.OrderSysNo.Value))
                || (w.OrderType == PayableOrderType.CollectionSettlement && collectionSettleSysNoList.Contains(w.OrderSysNo.Value)))
            .ToList();

            return payItemList;
        }

        /// <summary>
        /// 更新付款单可用金额
        /// </summary>
        /// <param name="orderType">单据类型</param>
        /// <param name="orderSysNo">单据编号</param>
        /// <param name="availableAmt">可用金额</param>
        public virtual void UpdateAvailableAmt(PayableOrderType orderType, int orderSysNo, decimal availableAmt)
        {
            m_PayItemDA.UpdateAvailableAmt(orderType, orderSysNo, availableAmt);
        }

        /// <summary>
        /// 付款单是否是预付款
        /// </summary>
        /// <param name="orderType">单据类型</param>
        /// <param name="orderSysNo">单据编号</param>
        /// <returns></returns>
        public virtual bool IsAdvanced(PayableOrderType orderType, int orderSysNo)
        {
            return m_PayItemDA.IsAdvanced(orderType, orderSysNo);
        }

        #endregion [For PO Domain]

        #region Helper Methods

        private void ThrowBizException(string msgKeyName, params object[] args)
        {
            throw new BizException(GetMessageString(msgKeyName, args));
        }

        private string GetMessageString(string msgKeyName, params object[] args)
        {
            return string.Format(ResouceManager.GetMessageString(InvoiceConst.ResourceTitle.PayItem, msgKeyName), args);
        }

        #endregion Helper Methods
    }
}
