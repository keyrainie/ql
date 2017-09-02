using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using IPP.InventoryMgmt.JobV31.Common;
using IPP.InventoryMgmt.JobV31.BusinessEntities;

namespace IPP.InventoryMgmt.JobV31.DA
{
    public class InventoryQtyDA
    {
        public static int QueryRecordsCount()
        {
            int records = 0;
            DataCommand command = DataCommandManager.GetDataCommand("QueryThirdPartInventoryRecords");
            command.SetParameterValue("@PartnerType", Config.PartnerType);
            command.SetParameterValue("@CompanyCode", Config.CompanyCode);
            return records;
        }

        public static List<ThirdPartInventoryEntity> Query(int pageSize, int page)
        {
            CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("QueryThirdPartInventory");

            int StartRowNumber = (page - 1) * pageSize;
            string sql = command.CommandText;
            sql = sql.Replace("#PageSize#", pageSize.ToString());
            sql = sql.Replace("#StartRowNumber#", StartRowNumber.ToString());
            sql = sql.Replace("#PartnerType#", string.Format("'{0}'", Config.PartnerType));
            sql = sql.Replace("#CompanyCode#", string.Format("'{0}'", Config.CompanyCode));
            sql = sql.Replace("#WarehourseNumber#",Util.Contract(",",Config.WareHourseNumbers));
            command.CommandText = sql;
            return command.ExecuteEntityList<ThirdPartInventoryEntity>();
        }


    }
}
