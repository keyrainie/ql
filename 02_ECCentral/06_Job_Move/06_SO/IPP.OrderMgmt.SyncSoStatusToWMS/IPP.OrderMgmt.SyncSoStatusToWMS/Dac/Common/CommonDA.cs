using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility.DataAccess;
using IPP.OrderMgmt.SyncSoStatusToWMS.BusinessEntities;

namespace IPP.OrderMgmt.SyncSoStatusToWMS.Dac.Common
{
    public class CommonDA
    {
        public static bool UpdateSODeclareStatus(int sosysno,int sostatus,int stocksysno)
        {
            DataCommand command = DataCommandManager.GetDataCommand(string.Format("UpdateSODeclareStatus{0}", stocksysno));
            command.SetParameterValue("@SoSysNo", sosysno);
            command.SetParameterValue("@SoStatus", sostatus);
            int result = command.ExecuteNonQuery();
            if (result > 0 )
            {
                return true;
            }
            return false;
        }


        public static List<SoInfo> GetSyncSo(DateTime lastSyncDate)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSyncSo");

            command.SetParameterValue("@LastChangeStatusDate", lastSyncDate);

            return command.ExecuteEntityList<SoInfo>();

        }


        public static DateTime GetDbDate()
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetDbDate"); 

            return command.ExecuteScalar<DateTime>();

        }
    }
}
