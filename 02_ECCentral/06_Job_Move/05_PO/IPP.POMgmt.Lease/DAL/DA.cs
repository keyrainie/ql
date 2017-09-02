using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using AutoClose.Model;
using Newegg.Oversea.Framework.DataAccess;

namespace AutoClose.DAL
{


    public class DA
    {
        public static List<LeaseProduct> GetLeaseSO(DateTime begin, DateTime end)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetLeaseSO");
            command.SetParameterValue("@Begin", begin);
            command.SetParameterValue("@End", end);
            return command.ExecuteEntityList<LeaseProduct>();
        }

        public static List<LeaseProduct> GetLeaseRO(DateTime begin, DateTime end)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetLeaseRO");
            command.SetParameterValue("@Begin", begin);
            command.SetParameterValue("@End", end);
            return command.ExecuteEntityList<LeaseProduct>();
        }



        public static void SettleLeaseSO(LeaseProduct entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SettleLeaseSO");
            command.SetParameterValue("@SOItemSysNo", entity.OrderSysNo);
            command.ExecuteNonQuery();
        }
        public static void SettleLeaseRO(LeaseProduct entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("SettleLeaseRO");
            command.SetParameterValue("@RegisterSysNo", entity.OrderSysNo);
            command.ExecuteNonQuery();
        }
    }
       
}
