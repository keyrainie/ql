using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;

namespace IPP.OrderMgmt.ShipLog.Dac.Common
{
    public class CommonDA
    {
        public static bool IsTelPhoneCheck(int customerSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("IsTelPhoneCheck");
            command.SetParameterValue("@CustomerSysNo", customerSysNo);

            object result = command.ExecuteScalar();
            if (result != null)
            {
                return true;
            }
            return false;
        }

        
    }
}
