using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;

namespace SendNoticeMailForCustomerPoint
{
    public class DA
    {
        public static string GetSys_Configuration_NewPointSwitch()
        {
            DataCommand command = DataCommandManager.GetDataCommand(
                "GetSys_Configuration_NewPointSwitch");
            object value = command.ExecuteScalar();
            if (value != null && value != DBNull.Value)
            {
                return value.ToString();
            }
            return string.Empty;
        }
        public static int InsertEmail(string emailAddress, string mailSubject, string mailBody)
        {
            DataCommand command = DataCommandManager.GetDataCommand("InsertEmail");
            command.SetParameterValue("@emailAddress", emailAddress);
            command.SetParameterValue("@mailSubject", mailSubject);
            command.SetParameterValue("@mailBody", mailBody);
            int count = command.ExecuteNonQuery();
            return count;
        }
        public static List<ExpireEmail> GetExpireIEmailList(int days)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetExpireIEmailList");
            command.SetParameterValue("@Days", days);
            command.SetParameterValue("@Status", "A");
            command.SetParameterValue("@CompanyCode", "8601");
            List<ExpireEmail> result = command.ExecuteEntityList<ExpireEmail>();
            return result;
        }
    }
}
