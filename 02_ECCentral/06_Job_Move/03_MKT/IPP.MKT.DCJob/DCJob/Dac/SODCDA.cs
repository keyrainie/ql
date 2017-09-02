using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.ECommerceMgmt.ServiceJob.BusinessEntities;
using Newegg.Oversea.Framework.DataAccess;
using System.Data;

namespace IPP.ECommerceMgmt.ServiceJob.Dac
{
    public static class SODCDA
    {
        public static List<SOEntity> GetSOInfo()
        {
            List<SOEntity> result = new List<SOEntity>();
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetSOStatus");       
            result = command.ExecuteEntityList<SOEntity>();
            return result;
        }
        public static bool MakeOpered(int SoSysNo)
        {
            List<SOEntity> result = new List<SOEntity>();
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("MakeOpered");
            command.AddInputParameter("@SOSysNo",DbType.Int32,SoSysNo);
            return command.ExecuteNonQuery()>0;
        }
    }
}
