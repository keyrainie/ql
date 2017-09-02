using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace IPP.ECommerceMgmt.SendAmbassadorPoints.Utilities
{
    public static class AppConfigHelper
    {
        public static readonly string CompanyCode = ConfigurationManager.AppSettings["CompanyCode"];
        public static readonly int PerTopCount = ConvertHelper.ToInt(ConfigurationManager.AppSettings["PerTopCount"], 500);
        public static readonly int PerSleepSecond = ConvertHelper.ToInt(ConfigurationManager.AppSettings["PerSleepSecond"]);
        public static readonly int TestSOSysNo = ConvertHelper.ToInt(ConfigurationManager.AppSettings["TestSOSysNo"]);
        public static readonly int TestAgentSysNo = ConvertHelper.ToInt(ConfigurationManager.AppSettings["TestAgentSysNo"]);
    }
}
