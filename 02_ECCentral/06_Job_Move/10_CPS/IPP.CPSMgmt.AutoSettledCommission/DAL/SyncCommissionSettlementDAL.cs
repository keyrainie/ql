using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using IPPOversea.Invoicemgmt.AutoSettledCommission.Model;

namespace IPPOversea.Invoicemgmt.AutoSettledCommission.DAL
{
    public class SyncCommissionSettlementDAL
    {
        #region GET



        
        /// <summary>
        /// 获取SO单 的商品信息
        /// </summary>
        /// <param name="soSysNo"></param>
        /// <returns></returns>
        public static List<OrderProductEntity> GetItemSO(int soSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Sync.GetItemSO");
            command.SetParameterValue("@SOSysNo", soSysNo);
            return command.ExecuteEntityList<OrderProductEntity>();
        }

        /// <summary>
        /// 获取退款申请的商品信息
        /// </summary>
        /// <param name="refundSysNo"></param>
        /// <returns></returns>
        public static List<OrderProductEntity> GetItemRMA(int refundSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Sync.GetItemRMA");
            command.SetParameterValue("@RefundSysNo", refundSysNo);
            return command.ExecuteEntityList<OrderProductEntity>();
        }
        #endregion GET


        public static int CreateCommissionSettlement(CommissionSettlementEntity entity)
        {

            DataCommand command = DataCommandManager.GetDataCommand("Sync.CreateCommissionSettlement");
            command.SetParameterValue("@UserSysNo", entity.UserSysNo);
            command.SetParameterValue("@SettledBeginDate", entity.SettledBeginDate);
            command.SetParameterValue("@SettledEndDate", entity.SettledEndDate);
            command.SetParameterValue("@SettledTime", DateTime.Now);
            
            command.SetParameterValue("@InUser", "System Job");
            
            return command.ExecuteScalar<int>();
        }



        public static bool UpdateCommissionSettlement(int commSysNo, decimal totalAmt)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Sync.UpdateCommissionSettlement");
            command.SetParameterValue("@SysNo", commSysNo);
            command.SetParameterValue("@CommissionAmt", totalAmt);

            return command.ExecuteNonQuery() > 0;
        }
        /// <summary>
        /// 添加账户余额
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <param name="balanceAmt"></param>
        /// <returns></returns>
        public static bool AddUserBalanceAmt(int userSysNo,decimal balanceAmt)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Sync.AddUserBalanceAmt");
            command.SetParameterValue("@SysNo", userSysNo);
            command.SetParameterValue("@BalanceAmt", balanceAmt);

            return command.ExecuteNonQuery() > 0;
        }
    }
}
