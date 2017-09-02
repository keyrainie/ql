using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.IDataAccess
{
    public interface IPayableDA
    {
        /// <summary>
        /// 创建应付款信息
        /// </summary>
        /// <param name="entity">应付款数据</param>
        /// <returns></returns>
        PayableInfo Create(PayableInfo input);

        /// <summary>
        /// 更新应付款审核信息
        /// </summary>
        /// <param name="entity"></param>
        void UpdateAuditInfo(PayableInfo entity);

        /// <summary>
        /// 根据应付款编号加载应付款信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        PayableInfo LoadBySysNo(int sysNo);

        /// <summary>
        /// 根据查询条件查询应付款列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        List<PayableInfo> GetListByCriteria(PayableInfo query);

        /// <summary>
        /// 更新应付款发票信息
        /// </summary>
        /// <param name="entity"></param>
        void UpdateInvoiceInfo(PayableInfo entity);

        /// <summary>
        /// 更新应付款状态
        /// </summary>
        /// <param name="entity"></param>
        void UpdateStatus(PayableInfo entity);

        /// <summary>
        /// 更新应付款状态和已付金额
        /// </summary>
        /// <param name="entity"></param>
        void UpdateStatusAndAlreadyPayAmt(PayableInfo entity);

        /// <summary>
        /// 更新应付款状态和单据金额
        /// </summary>
        /// <param name="entity"></param>
        void UpdateStatusAndOrderAmt(PayableInfo entity);

        /// <summary>
        /// 更新预计付款时间
        /// </summary>
        void UpdateETP(PayableInfo payableInfo);


        /// <summary>
        /// 根据PaySysNo查询单据状态
        /// </summary>
        /// <param name="sysNo">PaySysNo</param>
        /// <returns></returns>
        string QueryInvoiceStatusByPaySysNo(int sysNo);

        #region For PO Domain

        /// <summary>
        /// 检查付款单是否作废(代收结算单)
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
        /// 获取负PO的供应商财务应付款
        /// </summary>
        /// <param name="vendorSysNo"></param>
        /// <returns></returns>
        decimal GetVendorPayBalanceByVendorSysNo(int vendorSysNo);

        PayItemStatus? GetPOPrePayItemStatus(int poSysNo);

        PayableInfo GetFirstPay(PayableOrderType orderType, int orderSysNo);

        void UpdateFirstPay(PayableInfo entity);

        /// <summary>
        /// 查找未支付或部分支付的应付款
        /// 这里只查询PO、代销结算单和代收结算单三种单据类型
        /// </summary>
        /// <returns></returns>
        List<PayableInfo> GetUnPayOrPartlyPayList();

        #endregion For PO Domain

        void UpdatePayableInvoiceStatus(PayableInfo payable);

        void UpdatePayableInvoiceStatusWithEtp(PayableInfo payable);

        void UpdatePayableEGPAndETP(PayableInfo entity);
    }
}
