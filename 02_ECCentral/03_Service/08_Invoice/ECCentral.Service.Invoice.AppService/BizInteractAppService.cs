using System.Collections.Generic;
using System.Linq;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.Service.IBizInteract;
using ECCentral.Service.Invoice.BizProcessor;
using ECCentral.Service.Utility;
using System;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Invoice.Invoice;

namespace ECCentral.Service.Invoice.AppService
{
    /// <summary>
    /// Domain接口调用应用层服务
    /// </summary>
    [VersionExport(typeof(IInvoiceBizInteract))]
    public class BizInteractAppService : IInvoiceBizInteract
    {
        #region IInvoiceBizInteract Members

        /// <summary>
        /// 创建销售收款单
        /// </summary>
        /// <param name="entity">收款单信息</param>
        /// <returns>创建好的销售收款单</returns>
        public SOIncomeInfo CreateSOIncome(SOIncomeInfo entity)
        {
            return ObjectFactory<SOIncomeProcessor>.Instance.Create(entity);
        }

        /// <summary>
        /// 拆分销售收款单
        /// </summary>
        /// <param name="soInfo">主订单</param>
        /// <param name="subSOList">子订单列表</param>
        public void CreateSplitSOIncome(SOBaseInfo soInfo, List<SOBaseInfo> subSOList)
        {
            ObjectFactory<SOIncomeProcessor>.Instance.CreateSplitPayForSO(soInfo, subSOList);
        }

        /// <summary>
        /// 取消拆分销售收款单
        /// </summary>
        /// <param name="soInfo">主订单</param>
        /// <param name="subSOList">子订单列表</param>
        public void CancelSplitSOIncome(SOBaseInfo soInfo, List<SOBaseInfo> subSOList)
        {
            ObjectFactory<SOIncomeProcessor>.Instance.AbandonSplitPayForSO(soInfo, subSOList);
        }

        /// <summary>
        /// 作废销售收款单
        /// </summary>
        /// <param name="soIncomeSysNo">收款单系统编号</param>
        public void AbandonSOIncome(int soIncomeSysNo)
        {
            ObjectFactory<SOIncomeProcessor>.Instance.Abandon(soIncomeSysNo);
        }

        /// <summary>
        /// 创建应付款
        /// </summary>
        /// <param name="payableInfo"></param>
        /// <returns></returns>
        public PayableInfo CreatePayable(PayableInfo entity)
        {
            return ObjectFactory<PayableProcessor>.Instance.Create(entity);
        }

        /// <summary>
        /// 创建付款单
        /// </summary>
        /// <param name="entity">付款单信息</param>
        public PayItemInfo CreatePayItem(PayItemInfo entity)
        {
            return ObjectFactory<PayItemProcessor>.Instance.Create(entity);
        }

        /// <summary>
        /// 批量创建付款单
        /// </summary>
        /// <param name="payItemList"></param>
        public void BatchCreatePayItem(List<PayItemInfo> payItemList)
        {
            var request = payItemList.Select(s => new BatchActionItem<PayItemInfo>()
            {
                ID = s.SysNo.ToString(),
                Data = s
            }).ToList();

            var BL = ObjectFactory<PayItemProcessor>.Instance;
            BatchActionManager.DoBatchAction(request, payItem => BL.Create(payItem));
        }

        /// <summary>
        /// 创建NetPay
        /// </summary>
        /// <param name="netpayEntity">网上支付实体</param>
        /// <param name="refundEntity">退款实体</param>
        /// <param name="isForceCheck">是否强制核收，如果是强制核收，refundEntity必须要有值</param>
        /// <returns>创建好的netpay实体</returns>
        public NetPayInfo CreateNetPay(NetPayInfo netpayEntity, SOIncomeRefundInfo refundEntity, bool isForceCheck)
        {
            return ObjectFactory<NetPayProcessor>.Instance.Create(netpayEntity, refundEntity, isForceCheck);
        }

        /// <summary>
        /// 获取订单有效的NetPay信息
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <returns>订单有效的netpay，如果不存在，则返回NULL</returns>
        public NetPayInfo GetSOValidNetPay(int soSysNo)
        {
            return ObjectFactory<NetPayProcessor>.Instance.GetValidBySOSysNo(soSysNo);
        }

        /// <summary>
        /// 审核netpay
        /// </summary>
        /// <param name="netpaySysNo">netpay系统编号</param>
        public void AuditNetPay(int netpaySysNo)
        {
            ObjectFactory<NetPayProcessor>.Instance.Audit(netpaySysNo);
        }

        /// <summary>
        /// 创建销售-退款单
        /// </summary>
        /// <param name="refund"></param>
        /// <returns></returns>
        public SOIncomeRefundInfo CreateSOIncomeRefund(SOIncomeRefundInfo refund)
        {
            return ObjectFactory<SOIncomeRefundProcessor>.Instance.CreateSOIncomeRefund(refund);
        }

        /// <summary>
        /// 取得销售-退款单
        /// </summary>
        /// <param name="orderSysNo">单据系统编号</param>
        /// <param name="orderType">单据类型</param>
        /// <returns>符合条件的销售-退款单，如果没有找到则返回NULL</returns>
        public SOIncomeRefundInfo GetSOIncomeRefund(int orderSysNo, RefundOrderType orderType)
        {
            var refundList = ObjectFactory<SOIncomeRefundProcessor>.Instance.GetListByCriteria(new SOIncomeRefundInfo()
            {
                OrderSysNo = orderSysNo,
                OrderType = orderType
            });

            if (refundList != null && refundList.Count > 0)
            {
                return refundList[0];
            }
            return null;
        }

        /// <summary>
        /// RMA作废销售-退款单
        /// </summary>
        /// <param name="refundSysNo">销售-退款单系统编号</param>
        public void AbandonSOIncomeRefundForRO(int refundSysNo)
        {
            ObjectFactory<SOIncomeRefundProcessor>.Instance.AbandonForRO(refundSysNo);
        }

        /// <summary>
        /// RMA作废销售-退款单
        /// </summary>
        /// <param name="refundSysNo">销售-退款单系统编号</param>
        public void AbandonSOIncomeRefundForROBalance(int refundSysNo)
        {
            ObjectFactory<SOIncomeRefundProcessor>.Instance.AbandonForROBalance(refundSysNo);
        }

        /// <summary>
        /// 更新销售-退款单
        /// </summary>
        /// <param name="entity">销售-退款单实体</param>
        public void UpdateSOIncomeRefund(SOIncomeRefundInfo entity)
        {
            ObjectFactory<SOIncomeRefundProcessor>.Instance.Update(entity);
        }

        /// <summary>
        /// 创建财务拆分发票
        /// </summary>
        /// <param name="invoiceItems">拆分发票信息</param>
        public void CreateSubInvoiceItems(List<SubInvoiceInfo> invoiceItems)
        {
            ObjectFactory<SubInvoiceProcessor>.Instance.Create(invoiceItems);
        }

        /// <summary>
        /// 根据订单系统编号删除财务拆分发票
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        public void DeleteSubInvoiceBySOSysNo(int soSysNo)
        {
            ObjectFactory<SubInvoiceProcessor>.Instance.DeleteBySOSysNo(soSysNo);
        }

        /// <summary>
        /// 是否存在待审核的网上支付记录
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <returns>存在则返回true；不存在则返回false</returns>
        public bool IsExistOriginNetPay(int soSysNo)
        {
            return ObjectFactory<NetPayProcessor>.Instance.IsExistOriginalBySOSysNo(soSysNo);
        }

        /// <summary>
        /// 根据单据类型和单据编号取得有效的销售收款单
        /// </summary>
        /// <param name="orderSysNo">单据系统编号</param>
        /// <param name="orderType">单据类型</param>
        /// <returns>符合条件的有效的收款单,如果没有找到符合条件的收款单则返回NULL</returns>
        public SOIncomeInfo GetValidSOIncome(int orderSysNo, SOIncomeOrderType orderType)
        {
            return ObjectFactory<SOIncomeProcessor>.Instance.GetValid(orderSysNo, orderType);
        }

        /// <summary>
        /// 根据单据类型和单据编号取得销售收款单
        /// </summary>
        /// <param name="orderSysNo">单据系统编号</param>
        /// <param name="orderType">单据类型</param>
        /// <returns>符合条件的有效的收款单,如果没有找到符合条件的收款单则返回NULL</returns>
        public SOIncomeInfo GetConfirmedSOIncome(int orderSysNo, SOIncomeOrderType orderType)
        {
            return ObjectFactory<SOIncomeProcessor>.Instance.GetConfirmed(orderSysNo, orderType);
        }

        /// <summary>
        /// 为PendingList生成销售收款单时需要调用，用来更新收款单单据金额
        /// </summary>
        /// <param name="soIncomeSysNo">销售-收款单系统编号</param>
        /// <param name="orderAmt">单据金额</param>
        public void UpdateSOIncomeOrderAmt(int soIncomeSysNo, decimal orderAmt)
        {
            ObjectFactory<SOIncomeProcessor>.Instance.UpdateOrderAmtForSO(soIncomeSysNo, orderAmt);
        }

        /// <summary>
        /// 取得发票信息列表
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <returns></returns>
        public List<InvoiceMasterInfo> GetSOInvoiceMaster(int soSysNo)
        {
            return ObjectFactory<InvoiceProcessor>.Instance.GetMasterInfoList(soSysNo);
        }

        /// <summary>
        /// 自动确认收款单
        /// </summary>
        /// <param name="soIncomeSysNo">收款单系统编号</param>
        /// <param name="soSysNo">订单系统编号</param>
        /// <param name="opUserSysNo">操作用户系统编号</param>
        public void AutoConfirmIncome(int soIncomeSysNo, int soSysNo, int opUserSysNo)
        {
            var soIncomeInfo = new SOIncomeInfo();
            soIncomeInfo.SysNo = soIncomeSysNo;
            soIncomeInfo.ConfirmUserSysNo = opUserSysNo;
            soIncomeInfo.OrderSysNo = soSysNo;

            ObjectFactory<SOIncomeProcessor>.Instance.Confirm(soIncomeInfo,true);
        }

        /// <summary>
        /// 自动审核销售退款单(For RO)
        /// </summary>
        /// <param name="refundSysNo">销售退款单系统编号(原SOIncome_BankInfo.SysNo)</param>
        public void AutoAuditSOIncomeRefundForRO(int sysNo)
        {
            ObjectFactory<SOIncomeRefundProcessor>.Instance.AuditForRO(sysNo);
        }

        /// <summary>
        /// 提交审核销售退款单(For RO)
        /// </summary>
        /// <param name="sysNo">销售退款单系统编号(原SOIncome_BankInfo.SysNo)</param>
        public void SubmitAuditSOIncomeRefundForRO(int sysNo)
        {
            ObjectFactory<SOIncomeRefundProcessor>.Instance.SubmitAuditForRO(sysNo);
        }

        /// <summary>
        /// 取消提交审核售退款单(For RO)
        /// </summary>
        /// <param name="sysNo">销售退款单系统编号(原SOIncome_BankInfo.SysNo)</param>
        public void CancelSubmitAuditSOIncomeRefundForRO(int sysNo)
        {
            ObjectFactory<SOIncomeRefundProcessor>.Instance.CancelSubmitAuditForRO(sysNo);
        }

        /// <summary>
        /// 为RMA退款单编辑(三费合计)查询费用信息
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <param name="stockID">仓库系统编号</param>
        /// <param name="totalAmt">三费合计总金额</param>
        /// <param name="premiumAmt">保价费</param>
        /// <param name="shippingCharge">运费</param>
        /// <param name="payPrice">手续费</param>
        public void GetShipFee(int soSysNo, string stockSysNo, out decimal totalAmt, out decimal premiumAmt, out decimal shippingCharge, out decimal payPrice)
        {
            ObjectFactory<InvoiceProcessor>.Instance.GetShipFee(soSysNo, stockSysNo, out totalAmt, out premiumAmt, out shippingCharge, out payPrice);
        }

        /// <summary>
        /// 根据订单编号取得订单有效的postpay
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns></returns>
        public PostPayInfo GetValidPostPayBySOSysNo(int soSysNo)
        {
            return ObjectFactory<PostPayProcessor>.Instance.GetValidPostPayBySOSysNo(soSysNo);
        }

        /// <summary>
        /// 创建一张财务发票
        /// </summary>
        /// <param name="invoice">发票信息，包含主信息和Transaction信息</param>
        /// <returns></returns>
        public InvoiceInfo CreateInvoice(InvoiceInfo invoice)
        {
            return ObjectFactory<InvoiceProcessor>.Instance.Create(invoice);
        }

        #region For PO Domain

        public bool IsAbandonGatherPayItem(int sysNo)
        {
            return ObjectFactory<PayableProcessor>.Instance.IsAbandonGatherPayItem(sysNo);
        }

        public bool IsAbandonPayItem(int sysNo)
        {
            return ObjectFactory<PayableProcessor>.Instance.IsAbandonPayItem(sysNo);
        }

        public int LockOrUnlockPayItemByVendor(int vendorSysNo, bool isLock)
        {
            return ObjectFactory<PayItemProcessor>.Instance.LockOrUnLockByVendor(vendorSysNo, isLock);
        }

        public List<int> LockOrUnlockPayItemByVendorPM(int vendorSysNo, bool isLock, List<int> holdPMSysNoList, List<int> unHoldPMSysNoList)
        {
            return ObjectFactory<PayItemProcessor>.Instance.LockOrUnlockByVendorPM(vendorSysNo, isLock, holdPMSysNoList, unHoldPMSysNoList);
        }

        public decimal GetVendorPayBalanceByVendorSysNo(int vendorSysNo)
        {
            return ObjectFactory<PayableProcessor>.Instance.GetVendorPayBalanceByVendorSysNo(vendorSysNo);
        }

        public PayItemStatus? GetPOPrePayItemStatus(int poSysNo)
        {
            return ObjectFactory<PayableProcessor>.Instance.GetPOPrePayItemStatus(poSysNo);
        }

        public void CreatePayByVendor(int poSysNo, int batchNo, int orderStatus, PayableOrderType orderType, decimal? inStockAmt, string companyCode)
        {
            PayableInfo entity = new PayableInfo();
            entity.OrderSysNo = poSysNo;
            entity.BatchNumber = batchNo;
            entity.OrderStatus = orderStatus;
            entity.OrderType = orderType;
            entity.InStockAmt = inStockAmt;
            entity.CompanyCode = companyCode;

            ObjectFactory<PayableProcessor>.Instance.CreateByVendor(entity);
        }

        public PayItemInfo GetFinancePayItemByPOSysNo(int poSysNo)
        {
            return ObjectFactory<PayItemProcessor>.Instance.GetFinancePayItemInfoByPOSysNo(poSysNo);
        }

        public void InsertFinancePayItemInfo(PayItemInfo info)
        {
            ObjectFactory<PayItemProcessor>.Instance.InsertPayItemInfo(info);
        }

        #endregion For PO Domain

        /// <summary>
        /// 团购Job调用，用于创建AO单
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <param name="refundPayType">退款类型</param>
        /// <param name="note">退款备注</param>
        /// <param name="refundReason">退款原因系统编号，编号来自OverseaServiceManagement.dbo.refundReason</param>
        public void CreateAOForJob(int soSysNo, RefundPayType refundPayType, string note, int? refundReason)
        {
            ObjectFactory<SOIncomeRefundProcessor>.Instance.CreateAOForJob(soSysNo, refundPayType, note, refundReason);
        }

        /// <summary>
        /// 创建负的财务收款单
        /// </summary>
        /// <param name="refundInfo"></param>
        /// <returns></returns>
        public SOIncomeInfo CreateNegative(SOIncomeRefundInfo refundInfo)
        {
            return ObjectFactory<SOIncomeProcessor>.Instance.CreateNegative(refundInfo);
        }

        //调整积分有效期
        public void UpdatePointExpiringDate(int obtainSysNo, DateTime expiredDate)
        {
            ObjectFactory<BalanceRefundProcessor>.Instance.UpdatePointExpiringDate(obtainSysNo, expiredDate);
        }

        public virtual object AdjustPoint(AdjustPointRequest adjustInfo)
        {
            return ObjectFactory<BalanceRefundProcessor>.Instance.AdjustPoint(adjustInfo);
        }

        public virtual object SplitSOPointLog(int customerSysNo, BizEntity.SO.SOBaseInfo master, List<BizEntity.SO.SOBaseInfo> subSoList)
        {
            return ObjectFactory<BalanceRefundProcessor>.Instance.SplitSOPointLog(customerSysNo, master, subSoList);
        }

        public virtual object CancelSplitSOPointLog(int customerSysNo, BizEntity.SO.SOBaseInfo master, List<BizEntity.SO.SOBaseInfo> subSoList)
        {
            return ObjectFactory<BalanceRefundProcessor>.Instance.CancelSplitSOPointLog(customerSysNo, master, subSoList);
        }

        /// <summary>
        /// 检查支付方式是否可以支持现金退款
        /// </summary>
        /// <param name="payType"></param>
        /// <returns></returns>
        public bool CheckPayTypeCanCashRefund(int payType)
        {
            return ObjectFactory<SOIncomeRefundProcessor>.Instance.CheckPayTypeCanCashRefund(payType);
        }

        /// <summary>
        /// 审核NetPay【团购】
        /// </summary>
        /// <param name="netpaySysNo">netpay系统编号</param>
        public void AuditNetPay4GroupBuy(int netpaySysNo)
        {
            ObjectFactory<NetPayProcessor>.Instance.AuditNetPay4GroupBuy(netpaySysNo);
        }

        /// <summary>
        /// 网关订单查询
        /// </summary>
        /// <param name="entity"></param>
        public TransactionQueryBill QueryBill(string soSysNo) {

            return ObjectFactory<SOIncomeProcessor>.Instance.QueryBill(soSysNo);
        }

        /// <summary>
        /// 根据单据类型和单据编号取得有效的销售收款单
        /// </summary>
        /// <param name="orderSysNo">单据系统编号</param>
        /// <returns></returns>
        public SOIncomeInfo GetValid(int orderSysNo)
        {
            return ObjectFactory<SOIncomeProcessor>.Instance.GetValid(orderSysNo, SOIncomeOrderType.SO);
        }

        #endregion IInvoiceBizInteract Members



        public int SOOutStockInvoiceSync(int soSysNo, int stockSysNo, string invoiceNo, string companyCode)
        {
           return ObjectFactory<InvoiceProcessor>.Instance.SOOutStockInvoiceSync(soSysNo, stockSysNo, invoiceNo, companyCode);
        }
    }
}