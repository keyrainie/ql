using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using IPP.OrderMgmt.JobV31.BusinessEntities.OutStock;
using IPP.OrderMgmt.JobV31.BusinessEntities.Common;
using IPP.Oversea.CN.OrderMgmt.ServiceInterfaces.DataContracts;

namespace IPP.OrderMgmt.JobV31.Dac.OutStockSO
{
    public class OutStockSODA
    {
        public static List<SOEntity4OutStockEntity> GetOutStockSOList(string CompanyCode)
        {
            List<SOEntity4OutStockEntity> result = new List<SOEntity4OutStockEntity>();

            DataCommand command = DataCommandManager.GetDataCommand("GetOutStockSOList");

            command.SetParameterValue("@CompanyCode", CompanyCode);

            result = command.ExecuteEntityList<SOEntity4OutStockEntity>();

            return result;
        }

        public static bool IsItemAvail(int soSysNo, string localWHSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsItemAvail");
            command.SetParameterValue("SOSysNo", soSysNo);
            command.SetParameterValue("LocalWHSysNo", localWHSysNo);

            int result = command.ExecuteScalar<int>() ;
            if (result >= 0)
            {
                return true;
            }

            return false;
        }

        public static List<SOItem4OutStockEntity> GetSOItem4OutStock(int soSysNo, string localWHSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSOItem4OutStock");
            command.SetParameterValue("SOSysNo", soSysNo);
            command.SetParameterValue("LocalWHSysNo", localWHSysNo);

            List<SOItem4OutStockEntity> result = command.ExecuteEntityList<SOItem4OutStockEntity>();

            return result;
        }

        public static int UpdateSOStockStatus(int soSysNo, int stockStatus)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateSOStockStatus");
            command.SetParameterValue("SOSysNo", soSysNo);
            command.SetParameterValue("StockStatus", stockStatus);

            int result = command.ExecuteNonQuery();
            return result;
        }
    }
}
