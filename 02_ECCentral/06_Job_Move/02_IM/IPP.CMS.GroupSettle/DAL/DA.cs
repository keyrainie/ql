using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using GroupSettle.Model;
using Newegg.Oversea.Framework.DataAccess;
using System.Configuration;

namespace GroupSettle.DAL
{


    public class DA
    {
        public static List<GroupBuyingTicketToAcc> GetSettleGroup()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSettleGroup");
            //command.SetParameterValue("@BeginDate", begin);
            //command.SetParameterValue("@EndDate", end);
            return command.ExecuteEntityList<GroupBuyingTicketToAcc>();
        }
        public static GroupBuyingSettleMaster CreateMaster(GroupBuyingSettleMaster entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CeateSettleMaster");
            command.SetParameterValue("@BeginDate", entity.BeginDate);
            command.SetParameterValue("@EndDate", entity.EndDate);
            command.SetParameterValue("@SettleAmt", entity.SettleAmt);
            command.SetParameterValue("@VendorSysNo", entity.VendorSysNo);
            command.SetParameterValue("@CreateUser", Settings.UserSysNo);
            entity.SysNo = Convert.ToInt32(command.ExecuteScalar());
            return entity;
        }
        public static GroupBuyingSettleItem CreateItem(GroupBuyingSettleItem entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CeateSettleItem");   
            command.SetParameterValue("@SettleAmt", entity.SettleAmt);
            command.SetParameterValue("@GroupBuyingSysNo", entity.GroupBuyingSysNo);
            command.SetParameterValue("@SettlementSysNo", entity.SettlementSysNo);
            entity.SysNo = Convert.ToInt32(command.ExecuteScalar());
            return entity;  
            
        }
        public static bool UpdateSettleAcc(GroupBuyingTicketToAcc entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateSettleAcc");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@SettlementItemSysNo", entity.SettlementItemSysNo);
            command.SetParameterValue("@EditUser", Settings.UserSysNo);            
            return command.ExecuteNonQuery()>0;              
        }     
        
    }       
}
