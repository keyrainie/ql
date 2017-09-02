using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.DataAccess
{
    public class DataMonitorDA
    {
        public void InsertDataLog(string dataLog)
        {
            var dataCommand = DataCommandManager.GetDataCommand("InsertDataMonitorLog");
            dataCommand.SetParameterValue("@DataLog", dataLog);
            dataCommand.ExecuteNonQuery();
        }
    }
}
