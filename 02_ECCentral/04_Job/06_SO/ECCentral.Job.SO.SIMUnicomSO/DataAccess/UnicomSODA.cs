using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using ECCentral.Job.SO.SIMUnicomSO.Logic;
namespace ECCentral.Job.SO.SIMUnicomSO.DataAccess
{
   public class UnicomSODA
    {
       public static int GetUnicomSOListCount() {
           DataCommand command = DataCommandManager.GetDataCommand("GetUnicomSOListCount");
           int count = 0;
           count = Convert.ToInt32(command.ExecuteScalar());
           return count;
       }
       public static List<SOSIMCardEntity> GetList(int page,int pageSize) {
          CustomDataCommand command=DataCommandManager.CreateCustomDataCommandFromConfig("GetUnicomSOListCount");
          int startRow = 0;
          int endRow = 0;
          startRow = (page - 1) * pageSize;
          endRow = page * pageSize;
          string sql =command.CommandText;
          sql= sql.Replace("@EndNumber", endRow.ToString()).Replace("@StartNumber", startRow.ToString());
          command.CommandText = sql;
          return command.ExecuteEntityList<SOSIMCardEntity>();
       }
    }
}
