using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Common.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Data;
using ECCentral.BizEntity.Common;

namespace ECCentral.Service.Common.SqlDataAccess
{
    [VersionExport(typeof(IHolidayDA))]
    public class HolidayDA : IHolidayDA
    {
        #region IHolidayDA Members

        public List<DateTime> GetHolidayList(string blockedService, string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetHolidayListByBlockedService");
            command.SetParameterValue("@CompanyCode", companyCode);
            command.SetParameterValue("@BlockedService", blockedService);
            List<DateTime> result = new List<DateTime>();
            using (IDataReader dr = command.ExecuteDataReader())
            {
                while (dr.Read())
                {
                    result.Add(Convert.ToDateTime(dr[0]));
                }
                dr.Close();
            }
            return result;
        }

        public List<Holiday> GetHolidaysAfterToday(string companyCode)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetHolidaysAfterToday");
            command.SetParameterValue("@CompanyCode", companyCode);
            return command.ExecuteEntityList<Holiday>();
        }

        #endregion


        public Holiday Create(Holiday entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Holiday_Create");
            cmd.SetParameterValue<Holiday>(entity);
            cmd.SetParameterValue("@CompanyCode", "8601");

            return cmd.ExecuteEntity<Holiday>();
        }

        public void Delete(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("Holiday_Delete");
            cmd.SetParameterValue("@SysNo", sysNo);
            cmd.SetParameterValue("@CompanyCode", "8601");

            cmd.ExecuteNonQuery();
        }


        public List<Holiday> GetHolidaysByEntity(int? shipTypeSysNo, DateTime? holidayDate)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetHolidaysSpecial");
            cmd.SetParameterValue("@ShipTypeSysNo", shipTypeSysNo);
            cmd.SetParameterValue("@HolidayDate", holidayDate);
            cmd.SetParameterValue("@CompanyCode", "8601");

            return cmd.ExecuteEntityList<Holiday>();
        }
    }
}
