using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.ControlPannel;
using ECommerce.Utility.DataAccess;

namespace ECommerce.DataAccess.Common
{
    public class LoginDA
    {
        public static UserInfo Login(string userName, string password)
        {
            DataCommand command = DataCommandManager.GetDataCommand("Login");

            command.SetParameterValue("@UserID", userName);
            command.SetParameterValue("@Pwd", password);
            command.SetParameterValue("@CompanyCode", "8601");
            return command.ExecuteEntity<UserInfo>();
        }
    }
}
