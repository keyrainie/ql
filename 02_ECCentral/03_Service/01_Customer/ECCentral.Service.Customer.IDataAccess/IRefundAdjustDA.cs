using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Customer;
using ECCentral.BizEntity.RMA;

namespace ECCentral.Service.Customer.IDataAccess
{
    public interface IRefundAdjustDA
    {
        /// <summary>
        /// 创建补偿退款单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        void RefundAdjustCreate(RefundAdjustInfo entity);

        /// <summary>
        /// 修改补偿退款单
        /// </summary>
        /// <param name="entity"></param>
        void RefundAdjustUpdate(RefundAdjustInfo entity);

        /// <summary>
        /// 修改补偿退款单状态
        /// </summary>
        /// <param name="refundSysNo"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        Boolean UpdateRefundAdjustStatus(RefundAdjustInfo entity);

        /// <summary>
        /// 根据订单编号获取退款单状态
        /// </summary>
        /// <param name="SoSysNo"></param>
        /// <returns></returns>
        int? GetRequestStatusBySoSysNo(int SoSysNo);

        /// <summary>
        /// 根据订单号获取补偿退款单相关信息
        /// </summary>
        /// <param name="RefundID"></param>
        /// <returns></returns>
        RefundAdjustInfo GetRefundDetailBySoSysNo(RefundAdjustInfo entity);

        /// <summary>
        /// 审核补偿退款单(退款操作，记录退款人和退款信息 2012.11.15 add by norton)
        /// </summary>
        /// <param name="SysNo"></param>
        /// <param name="Status"></param>
        /// <param name="RefundUserSysNo"></param>
        /// <returns></returns>
        bool AuditRefundAdjust(int SysNo, RefundAdjustStatus Status, int? RefundUserSysNo, DateTime? AuditTime);

        /// <summary>
        /// 根据补偿退款单的系统编号获取补偿退款单详细信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        RefundAdjustInfo GetRefundDetailBySysNo(RefundAdjustInfo entity);

        /// <summary>
        /// 获取补偿退款单的状态信息
        /// </summary>
        /// <param name="SysNo"></param>
        int? GetRefundAdjustStatus(int SysNo);

        /// <summary>
        /// 根据订单号从订单信息中获取顾客ID
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        RefundAdjustInfo GetCustomerIDBySOSysNo(RefundAdjustInfo entity);

        #region 节能补贴相关

        /// <summary>
        /// 根据订单号获取节能补贴信息
        /// </summary>
        /// <param name="SOSysNo"></param>
        /// <returns></returns>
        RefundAdjustInfo GetEnergySubsidyBySOSysNo(RefundAdjustInfo entity);

        /// <summary>
        /// 查看订单中的商品是否有节能补贴商品
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        RefundAdjustInfo GetInProductEnergySubsidyBySOSysNo(RefundAdjustInfo entity);

        /// <summary>
        /// 判断某订单是否已经存在有效的节能补贴信息（非【已作废】和【审核拒绝】的状态为有效）
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        bool GetEffectiveEnergySubsidySO(int soSysNo);

        /// <summary>
        /// 判断订单是否已出库
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        bool IsShippingOut(int soSysNo);

        /// <summary>
        /// 判断是否做过物流拒收
        /// </summary>
        /// <param name="SOSysNo"></param>
        /// <returns></returns>
        bool IsHaveAutoRMA(int SOSysNo);

        /// <summary>
        /// 查询节能补贴详细信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        List<EnergySubsidyInfo> GetEnergySubsidyDetialsBySOSysNo(EnergySubsidyInfo entity);

        /// <summary>
        /// 获取节能补贴信息 只关注订单中的商品是否存在于ProductEnergy表中
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        List<EnergySubsidyInfo> GetInProductEnergySubsidyDetials(EnergySubsidyInfo entity);
        #endregion
    }
}
