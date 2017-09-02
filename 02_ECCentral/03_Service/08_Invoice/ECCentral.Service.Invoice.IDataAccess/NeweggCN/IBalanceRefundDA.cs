using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Invoice;
using ECCentral.BizEntity.Invoice.Refund;
using ECCentral.BizEntity.Customer;

namespace ECCentral.Service.Invoice.IDataAccess
{
    public interface IBalanceRefundDA
    {
        /// <summary>
        /// 创建余额退款
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        BalanceRefundInfo Create(BalanceRefundInfo entity);

        /// <summary>
        /// 更新余额退款
        /// </summary>
        /// <param name="entity"></param>
        void Update(BalanceRefundInfo entity);

        /// <summary>
        /// 更新余额退款状态
        /// </summary>
        /// <param name="entity"></param>
        void UpdateStatus(int sysNo, BalanceRefundStatus status);

        /// <summary>
        /// 财务审核状态更新
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="finAuditUserSysNo"></param>
        /// <param name="status"></param>
        void UpdateStatusForFinConfirm(int sysNo, int finAuditUserSysNo, BalanceRefundStatus status);

        /// <summary>
        /// CS审核状态更新
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="csAuditUserSysNo"></param>
        /// <param name="status"></param>
        void UpdateStatusForCSConfirm(int sysNo, int csAuditUserSysNo, BalanceRefundStatus status);

        /// <summary>
        /// 设置凭证号
        /// </summary>
        /// <param name="sysNo"></param>
        /// <param name="referenceID"></param>
        void SetReferenceID(int sysNo, string referenceID);

        /// <summary>
        /// 加载余额退款
        /// </summary>
        /// <param name="sysNo">余额退款系统编号</param>
        /// <returns></returns>
        BalanceRefundInfo Load(int sysNo);

        /// <summary>
        /// 修改积分有效期
        /// </summary>
        /// <param name="sysNo">[OverseaInvoiceReceiptManagement].[dbo].[Point_Obtain]的SysNo</param>
        /// <param name="expiredDate"></param>
        /// <returns></returns>
        int UpdatePointExpiringDate(int sysNo, DateTime expiredDate);

        /// <summary>
        /// 调整顾客积分
        /// </summary>
        /// <param name="adujstInfo"></param>
        /// <returns></returns>
        object Adjust(AdjustPointRequest adujstInfo);

        /// <summary>
        /// 调整顾客积分预检查
        /// </summary>
        /// <param name="adujstInfo"></param>
        /// <returns></returns>
        object AdjustPointPreCheck(AdjustPointRequest adujstInfo);

        object SplitSOPointLog(int customerSysNo, BizEntity.SO.SOBaseInfo master, List<BizEntity.SO.SOBaseInfo> subSoList);

        object CancelSplitSOPointLog(int customerSysNo, BizEntity.SO.SOBaseInfo master, List<BizEntity.SO.SOBaseInfo> subSoList);
    }
}