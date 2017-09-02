using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.Invoice.IDataAccess
{
    public interface ISOIncomeRefundDA
    {
        /// <summary>
        /// 创建退款单
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        SOIncomeRefundInfo Create(SOIncomeRefundInfo input);

        /// <summary>
        /// 更新退款单
        /// </summary>
        /// <param name="input"></param>
        void Update(SOIncomeRefundInfo input);

        /// <summary>
        /// 加载退款单
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        SOIncomeRefundInfo LoadBySysNo(int sysNo);

        /// <summary>
        /// 获取销售退款单列表
        /// </summary>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        List<SOIncomeRefundInfo> GetListByCriteria(SOIncomeRefundInfo query);

        /// <summary>
        /// 创建Alipay退款
        /// </summary>
        /// <param name="soSysNo">订单系统编号</param>
        void CreateAliPayRefund(int soSysNo);

        /// <summary>
        /// 更新销售退款单状态
        /// </summary>
        /// <param name="sysNo">销售退款单系统编号</param>
        /// <param name="userSysNo">引起状态改变的操作人系统编号，如果不是审核动作可以填null</param>
        /// <param name="status">销售退款单的目标状态</param>
        /// <param name="auditTime">审核时间，如果不是审核动作可以填null</param>
        bool UpdateStatus(int sysNo, int? userSysNo, RefundStatus status, DateTime? auditTime);

        /// <summary>
        /// 更新销售退款单状态和财务备注
        /// </summary>
        /// <param name="sysNo">销售退款单系统编号</param>
        /// <param name="userSysNo">引起状态改变的操作人系统编号，可以填null</param>
        /// <param name="status">销售退款单的目标状态</param>
        /// <param name="auditTime">审核时间，如果不是审核动作可以填null</param>
        /// <param name="finNote">财务备注</param>
        bool UpdateStatusAndFinNote(int sysNo, int? userSysNo, RefundStatus refundStatus, DateTime? dateTime, string finNote);

        /// <summary>
        /// 根据退款单号获取退款单信息
        /// </summary>
        /// <param name="SysNo"></param>
        /// <param name="CompanyCode"></param>
        /// <returns></returns>
        SOIncomeRefundInfo GetSOIncomeRefundByID(int SysNo, string CompanyCode);

        /// <summary>
        /// 获取退款记录数
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        int GetRefundOrder(int soSysNo, string companyCode);

        /// <summary>
        /// 获取最后一次退款的payment
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        decimal GetPayAmountBeHis(int soSysNo, string companyCode);
    }
}