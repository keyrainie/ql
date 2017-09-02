using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using IPPOversea.Invoicemgmt.SyncCommissionSettlement.Model;

namespace IPPOversea.Invoicemgmt.SyncCommissionSettlement.DAL
{
    public class SettledCommissionDAL
    {
        /// <summary>
        /// 
        /// </summary>
        public static List<UserEntity> GetSettledUserList(DateTime beginDate, DateTime endDate)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Settled.GetSettledUserList");
            command.SetParameterValue("@BeginDate", beginDate);
            command.SetParameterValue("@EndDate", endDate);
            return command.ExecuteEntityList<UserEntity>();
        }
        public static List<CommissionSettlementItemEntity> CommissionSettlementItemBySysNo(int userSysNo, DateTime beginDate, DateTime endDate)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Settled.CommissionSettlementItemBySysNo");
            command.SetParameterValue("@UserSysNo", userSysNo); 
            command.SetParameterValue("@BeginDate", beginDate);
            command.SetParameterValue("@EndDate", endDate);
            return command.ExecuteEntityList<CommissionSettlementItemEntity>();
        }

        public static bool UpdateCommissionSettlementItem(CommissionSettlementItemEntity entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Settled.UpdateCommissionSettlementItem");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@CommissionAmt", entity.CommissionAmt);
            command.SetParameterValue("@CommissionSettlementSysNo", entity.CommissionSettlementSysNo);
            command.SetParameterValue("@EditUser", "System Job");

            return command.ExecuteNonQuery() > 0;
        }
        
    }
}
