using System;
using System.Collections.Generic;

using Newegg.Oversea.Framework.DataAccess;

using IPPOversea.Invoicemgmt.AutoSettledCommission.Model;

namespace IPPOversea.Invoicemgmt.AutoSettledCommission.DAL
{
    public class SyncAutomationDAL
    {
        /// <summary>
        /// 获取全部未提交付款申请的结算单
        /// </summary>
        /// <returns></returns>
        public static List<CommissionSettlementEntity> GetUnApplyedCommissionSettlement()
        {
            DataCommand command = DataCommandManager.GetDataCommand("Settled.GetUnApplyedCommissionSettlement");
            return command.ExecuteEntityList<CommissionSettlementEntity>();
        }

        /// <summary>
        /// 按照用户系统编号SysNo获取用户信息
        /// </summary>
        /// <param name="userSysNo">系统编号</param>
        /// <returns>用户信息</returns>
        public static UserInfo GetUserInfo(int userSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Settled.GetUser");
            command.SetParameterValue("@SysNo", userSysNo);
            return command.ExecuteEntity<UserInfo>();
        }

        /// <summary>
        /// 获取用户未提交申请的结算单金额
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <returns></returns>
        public static decimal GetUnRequestCommissionSettlementAmt(int userSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Settled.GetUnRequestCommissionSettlementAmt");
            command.SetParameterValue("@UserSysNo", userSysNo);
            return command.ExecuteScalar<decimal>();
        }

        /// <summary>
        /// 创建佣金兑现申请
        /// </summary>
        /// <param name="data"></param>
        public static int CreateRequestToCashRecord(CommissionToCashRecordEntity recordEntity)
        {
            //1.创建兑现申请单
            //2.修改对应的佣金结算单信息【CommissionToCashRecordSysNo，Status】
            //设置命令对象信息

            DataCommand command = DataCommandManager.GetDataCommand("Settled.CreateRequestToCash");
            command.SetParameterValue("@UserSysNo", recordEntity.UserSysNo);
            command.SetParameterValue("@Status", recordEntity.Status);
            command.SetParameterValue("@ToCashAmt", recordEntity.ToCashAmt);
            command.SetParameterValue("@AfterTaxAmt", recordEntity.AfterTaxAmt);
            command.SetParameterValue("@BankCode", recordEntity.BankCode);
            command.SetParameterValue("@BankName", recordEntity.BankName);
            command.SetParameterValue("@BranchBank", recordEntity.BranchBank);
            command.SetParameterValue("@BankCardNumber", recordEntity.BankCardNumber);
            command.SetParameterValue("@ReceivableName", recordEntity.ReceivableName);
            command.SetParameterValue("@IsHasInvoice", recordEntity.IsHasInvoice);
            command.SetParameterValue("@InUser", recordEntity.InUser);

            return command.ExecuteScalar<int>();
        }

        /// <summary>
        /// 更新结算单 CommissionToCashRecordSysNo
        /// </summary>
        /// <param name="toCashSysNos"></param>
        public static bool UpdateCommissionSettlement(int recordSysNo, string toCashSysNos)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Settled.UpdateCommissionSettlement");
            command.SetParameterValue("@CommissionToCashRecordSysNo", recordSysNo);
            command.ReplaceParameterValue("@CacheSysNoList", toCashSysNos);

            return command.ExecuteNonQuery() > 0;
        }
    }
}
