using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using IPPOversea.Invoicemgmt.SyncCommissionSettlement.Model;

namespace IPPOversea.Invoicemgmt.SyncCommissionSettlement.DAL
{
    public class SyncCommissionSettlementDAL
    {
        #region GET
        /// <summary>
        /// 获取需要同步创建的 SO& RMA 信息
        /// </summary>
        public static List<CommissionSettlementItemEntity> GetNeedToSynchronizeCreate()
        {
            DataCommand command = DataCommandManager.GetDataCommand("Sync.GetNeedToSynchronizeCreate");
            return command.ExecuteEntityList<CommissionSettlementItemEntity>();
        }

        /// <summary>
        /// 获取需要同步更新的 SO& RMA 信息
        /// </summary>
        public static List<CommissionSettlementItemEntity> GetNeedToSynchronizeUpdate()
        {
            DataCommand command = DataCommandManager.GetDataCommand("Sync.GetNeedToSynchronizeUpdate");
            return command.ExecuteEntityList<CommissionSettlementItemEntity>();
        }

        
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

        /// <summary>
        /// 创建佣金结算Item
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool CreateCommissionSettlementItem(CommissionSettlementItemEntity entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Sync.CreateCommissionSettlementItem");
            command.SetParameterValue("@OrderSysNo", entity.OrderSysNo);
            command.SetParameterValue("@OrderDate", entity.OrderDate);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@OrderAmt", entity.OrderAmt);
            command.SetParameterValue("@Type", entity.Type);
            command.SetParameterValue("@UserSysNo", entity.UserSysNo);
            command.SetParameterValue("@ChannelSysNo", entity.ChannelSysNo);
            command.SetParameterValue("@CommissionAmt", entity.CommissionAmt);
            command.SetParameterValue("@SubSource", entity.SubSource);
            command.SetParameterValue("@InUser", "System Job");

            return command.ExecuteNonQuery() > 0;
        }
        /// <summary>
        /// 同步作废
        /// </summary>
        public static int VoidCommissionSettlementItem(int syncCount)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Sync.VoidCommissionSettlementItem");
            command.SetParameterValue("@SyncCount", syncCount);
            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// 同步更新出库时间和RMA审核通过时间
        /// </summary>
        /// <returns></returns>
        public static int UpdateCommissionSettlementItemOrder(int syncCount)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Sync.UpdateCommissionSettlementItemOrder");
            command.SetParameterValue("@SyncCount", syncCount);
            return command.ExecuteNonQuery();
        }
        
        
        #endregion GET

        public static bool UpdateCommissionSettlementItem(CommissionSettlementItemEntity entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Sync.UpdateCommissionSettlementItem");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@CommissionAmt", entity.CommissionAmt);
            command.SetParameterValue("@OrderAmt", entity.OrderAmt);
            command.SetParameterValue("@EditUser", "System Job");
            return command.ExecuteNonQuery() > 0;
        }

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



        internal static bool UpdateCommissionSettlement(int commSysNo, decimal totalAmt)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Sync.UpdateCommissionSettlement");
            command.SetParameterValue("@SysNo", commSysNo);
            command.SetParameterValue("@CommissionAmt", totalAmt);

            return command.ExecuteNonQuery() > 0;
        }
    }
}
