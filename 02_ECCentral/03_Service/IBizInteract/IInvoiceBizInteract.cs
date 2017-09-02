/*********************************************************************************************
// Copyright (c) 2012, Newegg (Chengdu) Co., Ltd. All rights reserved.
// Created by Jin.J.Qin at 3/23/2012
// Target Framework : 4.0
// Class Name : IInvoiceBizInteract
// Description : Invoice Domain向其它Domain提供Biz服务的接口//
//*********************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.SO;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.Invoice.Invoice;

namespace ECCentral.Service.IBizInteract
{
    public interface IInvoiceBizInteract
    {
        #region 销售收款单相关

        /// <summary>
        /// 根据单据类型和单据编号取得有效的销售收款单
        /// </summary>
        /// <param name="orderSysNo">单据系统编号</param>
        /// <param name="orderType">单据类型</param>
        /// <returns>符合条件的有效的收款单,如果没有找到符合条件的收款单则返回NULL</returns>
        SOIncomeInfo GetValidSOIncome(int orderSysNo, SOIncomeOrderType orderType);

        /// <summary>
        /// 根据单据类型和单据编号取得已经确认的销售收款单
        /// </summary>
        /// <param name="orderSysNo">单据系统编号</param>
        /// <param name="orderType">单据类型</param>
        /// <returns>符合条件的有效的收款单,如果没有找到符合条件的收款单则返回NULL</returns>
        SOIncomeInfo GetConfirmedSOIncome(int orderSysNo, SOIncomeOrderType orderType);

        /// <summary>
        /// 创建财务收款单
        /// </summary>
        /// <param name="entity">需要创建的财务收款单信息</param>
        /// <returns>创建后的财务收款单，SysNo为数据持久化后的系统编号</returns>
        SOIncomeInfo CreateSOIncome(SOIncomeInfo entity);

        /// <summary>
        /// 拆分销售收款单
        /// </summary>
        /// <param name="masterSO">主单信息</param>
        /// <param name="subSOList">子单信息</param>
        void CreateSplitSOIncome(SOBaseInfo soInfo, List<SOBaseInfo> subSOList);

        /// <summary>
        /// 取消拆分销售收款单
        /// </summary>
        /// <param name="masterSO">主单信息</param>
        /// <param name="subSOList">子单信息</param>
        void CancelSplitSOIncome(SOBaseInfo soInfo, List<SOBaseInfo> subSOList);

        /// <summary>
        /// 自动确认收款单
        /// </summary>
        /// <param name="soIncomeSysNo">收款单系统编号</param>
        /// <param name="soSysNo">订单系统编号</param>
        /// <param name="opUserSysNo">操作用户系统编号</param>
        void AutoConfirmIncome(int soIncomeSysNo, int soSysNo, int opUserSysNo);

        /// <summary>
        /// 作废销售收款单
        /// </summary>
        /// <param name="soIncomeSysNo">收款单系统编号</param>
        void AbandonSOIncome(int soIncomeSysNo);

        /// <summary>
        /// 为PendingList生成销售收款单时需要调用，用来更新收款单单据金额
        /// </summary>
        /// <param name="soIncomeSysNo">销售-收款单系统编号</param>
        /// <param name="orderAmt">单据金额</param>
        void UpdateSOIncomeOrderAmt(int soIncomeSysNo, decimal orderAmt);

        /// <summary>
        /// 创建负收款单
        /// </summary>
        /// <param name="refundInfo"></param>
        /// <returns></returns>
        SOIncomeInfo CreateNegative(SOIncomeRefundInfo refundInfo);

        /// <summary>
        /// 网关订单查询
        /// </summary>
        /// <param name="entity"></param>
        TransactionQueryBill QueryBill(string soSysNo);

        #endregion 销售收款单相关

        #region 付款单相关

        /// <summary>
        /// 创建应付款
        /// </summary>
        /// <param name="payableInfo"></param>
        /// <returns></returns>
        PayableInfo CreatePayable(PayableInfo payableInfo);

        /// <summary>
        /// 创建付款单
        /// </summary>
        /// <param name="entity">付款单信息</param>
        PayItemInfo CreatePayItem(PayItemInfo payItem);

        /// <summary>
        /// 批量创建付款单
        /// </summary>
        /// <param name="payItemList"></param>
        void BatchCreatePayItem(List<PayItemInfo> payItemList);

        #endregion 付款单相关

        #region 网上支付记录相关

        /// <summary>
        /// 创建NetPay，如果是强制核收，可以通过传入的退款信息创建一张多付退款单
        /// </summary>
        /// <param name="netpayEntity">网上支付实体</param>
        /// <param name="refundEntity">退款实体</param>
        /// <param name="isForceCheck">是否强制核收，如果是强制核收，refundEntity必须要有值</param>
        /// <returns>创建好的netpay实体</returns>
        NetPayInfo CreateNetPay(NetPayInfo netpayEntity, SOIncomeRefundInfo refundEntity, bool isForceCheck);

        /// <summary>
        /// 获取订单有效的网上支付记录
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <returns>取得的网上支付记录</returns>
        NetPayInfo GetSOValidNetPay(int soSysNo);

        /// <summary>
        /// 是否存在待审核的网上支付记录
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <returns>存在则返回true；不存在则返回false</returns>
        bool IsExistOriginNetPay(int soSysNo);

        /// <summary>
        /// 审核NetPay
        /// </summary>
        /// <param name="netpaySysNo">netpay系统编号</param>
        void AuditNetPay(int netpaySysNo);

        /// <summary>
        /// 审核NetPay【团购】
        /// </summary>
        /// <param name="netpaySysNo">netpay系统编号</param>
        void AuditNetPay4GroupBuy(int netpaySysNo);

        #endregion 网上支付记录相关

        #region 邮局电汇支付记录相关

        /// <summary>
        /// 根据订单编号取得有效的邮局电汇支付记录
        /// </summary>
        /// <param name="soSysNo">订单编号</param>
        /// <returns></returns>
        PostPayInfo GetValidPostPayBySOSysNo(int soSysNo);

        #endregion 邮局电汇支付记录相关

        #region 销售退款单相关

        /// <summary>
        /// 创建销售退款单
        /// </summary>
        /// <param name="entity">销售退款单信息</param>
        /// <returns>创建后的销售退款单</returns>
        SOIncomeRefundInfo CreateSOIncomeRefund(SOIncomeRefundInfo entity);

        /// <summary>
        /// 更新销售退款单
        /// </summary>
        /// <param name="input">销售退款单信息</param>
        void UpdateSOIncomeRefund(SOIncomeRefundInfo entity);

        /// <summary>
        /// 获取销售退款单信息
        /// </summary>
        /// <param name="orderSysNo">业务单据编号</param>
        /// <param name="orderType">业务单据类型</param>
        /// <returns></returns>
        SOIncomeRefundInfo GetSOIncomeRefund(int orderSysNo, RefundOrderType orderType);

        /// <summary>
        /// 作废RO产生的退款单
        /// </summary>
        /// <param name="sysNo"></param>
        void AbandonSOIncomeRefundForRO(int sysNo);

        /// <summary>
        /// 作废ROBalance产生的退款单
        /// </summary>
        /// <param name="sysNo"></param>
        void AbandonSOIncomeRefundForROBalance(int sysNo);

        /// <summary>
        /// 自动审核销售退款单,在RMA_Refund提交审核且不涉及现金的时候调用，不涉及现金的时候自动审核
        /// </summary>
        /// <param name="refundSysNo">销售退款单系统编号(原SOIncome_BankInfo.SysNo)</param>
        void AutoAuditSOIncomeRefundForRO(int sysNo);

        /// <summary>
        /// 提交审核销售退款单,在RMA_Refund提交审核且涉及现金的时候调用，涉及现金需要提交财务审核
        /// </summary>
        /// <param name="sysNo">销售退款单系统编号(原SOIncome_BankInfo.SysNo)</param>
        void SubmitAuditSOIncomeRefundForRO(int sysNo);

        /// <summary>
        /// 取消提交审核售退款单
        /// </summary>
        /// <param name="sysNo">销售退款单系统编号(原SOIncome_BankInfo.SysNo)</param>
        void CancelSubmitAuditSOIncomeRefundForRO(int sysNo);

        /// <summary>
        /// 检查支付方式是否可以支持现金退款
        /// </summary>
        /// <param name="payType"></param>
        /// <returns></returns>
        bool CheckPayTypeCanCashRefund(int payType);

        /// <summary>
        /// 根据单据类型和单据编号取得有效的销售收款单
        /// </summary>
        /// <param name="orderSysNo">单据系统编号</param>
        /// <returns></returns>
        SOIncomeInfo GetValid(int orderSysNo);

        #endregion 销售退款单相关

        #region 发票相关

        /// <summary>
        /// 创建一张财务发票
        /// </summary>
        /// <param name="invoice">发票信息，包含主信息和Transaction信息</param>
        /// <returns></returns>
        InvoiceInfo CreateInvoice(InvoiceInfo invoice);

        /// <summary>
        /// 获取订单发票主信息
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <returns></returns>
        List<InvoiceMasterInfo> GetSOInvoiceMaster(int soSysNo);

        /// <summary>
        /// 为RMA退款单编辑(三费合计)查询费用信息
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <param name="stockID">仓库系统编号</param>
        /// <param name="totalAmt">三费合计总金额</param>
        /// <param name="premiumAmt">保价费</param>
        /// <param name="shippingCharge">运费</param>
        /// <param name="payPrice">手续费</param>
        void GetShipFee(int soSysNo, string stockSysNo, out decimal totalAmt, out decimal premiumAmt, out decimal shippingCharge, out decimal payPrice);

        /// <summary>
        /// 创建SubInvoice
        /// </summary>
        /// <param name="invoiceItems">SubInvoice列表</param>
        void CreateSubInvoiceItems(List<SubInvoiceInfo> invoiceItems);

        /// <summary>
        /// 根据订单系统编号删除财务拆分发票
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        void DeleteSubInvoiceBySOSysNo(int soSysNo);

        int SOOutStockInvoiceSync(int soSysNo, int stockSysNo, string invoiceNo, string companyCode);

        #endregion 发票相关

        #region For PO Domain

        /// <summary>
        /// 检查付款单是否作废(代收结算单 ）
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        bool IsAbandonGatherPayItem(int sysNo);

        /// <summary>
        /// 检查付款单是否作废(代销结算单)
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        bool IsAbandonPayItem(int sysNo);

        /// <summary>
        /// 锁定或取消锁定供应商对应的付款单,并返回对付款单处理成功的记录数
        ///
        ///  message.Body.VendorSysNo = vendorSysNo;
        ///  message.Body.LockOperAction = isLock ? LockOperActionType.Lock : LockOperActionType.UnLock;
        ///  InvoiceService.LockOperPayItemByVender(message);
        ///
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <param name="isLock"></param>
        int LockOrUnlockPayItemByVendor(int vendorSysNo, bool isLock);

        /// <summary>
        /// 锁定或取消锁定供应商PM对应的付款单,并返回对付款单处理成功的记录数
        /// (锁定成功和解锁成功,list[0] = 锁定成功的条数，list[1]为解锁成功的条数:
        ///   message.Body.VendorSysNo = vendorSysNo;
        ///   message.Body.LockOperAction = LockOperActionType.Lock;
        ///   message.Body.SuccessOperNum = 0;
        ///   message.Body.HoldPMSysNoList = holdPMSysNoList;
        ///   message.Body.UnHoldPMSysNoList = unHoldPMSysNoList;

        ///   IMaintainPayItemV31 service = ServiceBroker.FindService<IMaintainPayItemV31>();
        ///   LockOperPayItemResult result=new LockOperPayItemResult ();
        ///   message = service.LockOperPayItemByVenderPM(message);
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <param name="isLock"></param>
        /// <param name="holdPMSysNoList"></param>
        /// <param name="UnHoldPMSysNoList"></param>
        /// <returns></returns>
        List<int> LockOrUnlockPayItemByVendorPM(int vendorSysNo, bool isLock, List<int> holdPMSysNoList, List<int> UnHoldPMSysNoList);

        /// <summary>
        /// 获取PO单已经存在已付款的预付款记录状态
        /// </summary>
        /// <param name="poSysNo"></param>
        /// <returns></returns>
        PayItemStatus? GetPOPrePayItemStatus(int poSysNo);

        /// <summary>
        /// 获取负PO的供应商财务应付款
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        decimal GetVendorPayBalanceByVendorSysNo(int vendorSysNo);

        /// <summary>
        /// PO单终止入库时,创建NewPay
        /// invoiceService.CreateNewPay(payable);
        /// </summary>
        /// <param name="poSysNo">po单号</param>
        /// <param name="batchNo">批次号</param>
        /// <param name="orderStatus">Payable Status(固定传6)</param>
        /// <param name="orderType">Payable Type(固定传5)</param>
        /// <param name="InStockAmt">入库金额</param>
        /// <returns></returns>
        void CreatePayByVendor(int poSysNo, int batchNo, int orderStatus, PayableOrderType orderType, decimal? inStockAmt, string companyCode);

        PayItemInfo GetFinancePayItemByPOSysNo(int poSysNo);

        void InsertFinancePayItemInfo(PayItemInfo info);

        #endregion For PO Domain

        #region For SO Domain

        /// <summary>
        /// 团购Job调用，用于创建AO单
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        /// <param name="refundPayType">退款类型</param>
        /// <param name="note">退款备注</param>
        /// <param name="refundReason">退款原因系统编号，编号来自OverseaServiceManagement.dbo.refundReason</param>
        void CreateAOForJob(int soSysNo, RefundPayType refundPayType, string note, int? refundReason);

        #endregion For SO Domain

        #region 调整积分相关

        void UpdatePointExpiringDate(int obtainSysNo, DateTime expiredDate);
        /// <summary>
        /// 调整积分
        /// </summary>
        /// <param name="adjustInfo"></param>
        /// <returns></returns>
        object AdjustPoint(AdjustPointRequest adjustInfo);
        /// <summary>
        /// 拆弹调整积分
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="master"></param>
        /// <param name="subSoList"></param>
        /// <returns></returns>
        object SplitSOPointLog(int customerSysNo, SOBaseInfo master, List<SOBaseInfo> subSoList);
        /// <summary>
        /// 撤销拆单调整积分
        /// </summary>
        /// <param name="customerSysNo"></param>
        /// <param name="master"></param>
        /// <param name="subSoList"></param>
        /// <returns></returns>
        object CancelSplitSOPointLog(int customerSysNo, SOBaseInfo master, List<SOBaseInfo> subSoList);
        #endregion
    }
}
