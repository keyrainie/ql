using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;

using ECCentral.BizEntity;
using ECCentral.Service.Utility;
using ECCentral.Service.RMA.IDataAccess;
using ECCentral.BizEntity.RMA;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Invoice;

namespace ECCentral.Service.RMA.SqlDataAccess
{
    [VersionExport(typeof(IRefundBalanceDA))]
    public class RefundBalanceDA : IRefundBalanceDA
    {
        #region Load

        /// <summary>
        /// 根据退款单系统编号获取新建退款调整单的基本信息
        /// </summary>
        /// <param name="OrgRefundSysNo"></param>
        /// <returns></returns>
        public virtual RefundBalanceInfo LoadNewRefundBalanceByRefundSysNo(int OrgRefundSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("LoadNewRefundBalanceByRefundSysNo");
            command.SetParameterValue("@RefundSysNo", OrgRefundSysNo);

            return command.ExecuteEntity<RefundBalanceInfo>();
        }

        #endregion

        #region
        /// <summary>
        /// 创建退款调整单。
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual RefundBalanceInfo CreateRefundBalance(RefundBalanceInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertRefundBalance");
            command.SetParameterValue("@NewOrderSysNo", entity.NewOrderSysNo);
            command.SetParameterValue("@BalanceOrderType", entity.BalanceOrderType);
            command.SetParameterValue("@OrgRefundSysNo", entity.OriginalRefundSysNo);
            command.SetParameterValue("@OrgSOSysNo", entity.OriginalSOSysNo);
            command.SetParameterValue("@CustomerSysNo", entity.CustomerSysNo);
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@ProductID", entity.ProductID);
            command.SetParameterValueAsCurrentUserSysNo("@CreateUserSysNo");
            command.SetParameterValue("@RefundTime", entity.RefundTime);
            command.SetParameterValue("@RefundUserSysNo", entity.RefundUserSysNo);
            command.SetParameterValue("@CashAmt", entity.CashAmt);
            command.SetParameterValue("@GiftCardAmt", entity.GiftCardAmt);
            command.SetParameterValue("@PointAmt", entity.PointAmt);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@Note", entity.Note);
            command.SetParameterValue("@RefundPayType", entity.RefundPayType);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);
            
            entity.SysNo = Convert.ToInt32(command.ExecuteScalar());
            return entity;
        }
        /// <summary>
        /// 更新退款调整单。
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual void UpdateRefundBalance(RefundBalanceInfo entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateRefundBalance");

            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@NewOrderSysNo", entity.NewOrderSysNo);
            command.SetParameterValue("@BalanceOrderType", entity.BalanceOrderType);
            command.SetParameterValue("@RefundTime", entity.RefundTime);
            command.SetParameterValue("@RefundUserSysNo", entity.RefundUserSysNo);
            command.SetParameterValue("@Status", entity.Status);
            command.ExecuteNonQuery();

        }

        #endregion

        /// <summary>
        /// 已发生的，退款单总的金额。
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual RefundItemInfo GetRefundTotalAmount(RefundBalanceInfo entity)
        {
            RefundItemInfo result = null;

            DataCommand command = DataCommandManager.GetDataCommand("GetRefundTotalAmount");
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@RefundSysNo", entity.OriginalRefundSysNo);
            command.SetParameterValue("@SoSysNo", entity.OriginalSOSysNo);

            result = command.ExecuteEntity<RefundItemInfo>();

            return result;
        }
        /// <summary>
        /// 已发生的，退款调整总的金额。
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual decimal GetRefundBalanceTotalAmount(RefundBalanceInfo entity)
        {
            decimal result = 0;

            DataCommand command = DataCommandManager.GetDataCommand("GetRefundBalanceTotalAmount");
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@OrgRefundSysNo", entity.OriginalRefundSysNo);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);

            result = command.ExecuteScalar<decimal>();

            return result;
        }

        public virtual RefundBalanceInfo GetRefundBalanceBySysNo(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetRefundBalanceBySysNo");
            command.SetParameterValue("@SysNo", sysNo);
            return command.ExecuteEntity<RefundBalanceInfo>();
        }
    }
}
