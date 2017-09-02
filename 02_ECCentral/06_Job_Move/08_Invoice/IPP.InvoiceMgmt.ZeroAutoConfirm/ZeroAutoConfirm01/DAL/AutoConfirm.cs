using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using ZeroAutoConfirm.Model;
using Newegg.Oversea.Framework.DataAccess;

namespace ZeroAutoConfirm.DAL
{
    public class AutoConfirm
    {
        public static List<ConfirmEntity> GetConfirmData(DateTime initialDate)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetConfirmData");
            command.SetParameterValue("@InitialDate", initialDate);
            command.SetParameterValue("@CompanyCode", Settings.CompanyCode);

            return command.ExecuteEntityList<ConfirmEntity>();
        }

        public static ConfirmEntity GetEmailData(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetEmailData");
            command.SetParameterValue("@SysNo", sysNo);

            return command.ExecuteEntity<ConfirmEntity>();
        }

        public static string GetConfirmID(int payTypeSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetConfirmID");
            command.SetParameterValue("@PayTypeSysNo", payTypeSysNo);
            command.SetParameterValue("@CompanyCode", Settings.CompanyCode);

            var execResult = command.ExecuteScalar();
            var result = "";
            if (execResult == null)
            {
                result = "N/A";
            }
            else
            {
                result = execResult.ToString();
            }

            return result;
        }
    }
}
