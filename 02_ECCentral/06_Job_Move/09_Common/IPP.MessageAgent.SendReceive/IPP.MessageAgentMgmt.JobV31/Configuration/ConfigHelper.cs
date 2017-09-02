using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Configuration;

namespace IPP.MessageAgent.SendReceive.JobV31.Configuration
{
    public class ConfigHelper
    {
        public static SSBProcessConfig SSBProcessConfig
        {
            get { return IPP.Framework.Configuration.ConfigHelper.LoadConfig<SSBProcessConfig>(); }
        }

        public static string GetAppSettings(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static RestServiceConfig RestServiceConfig
        {
            get { return IPP.Framework.Configuration.ConfigHelper.LoadConfig<RestServiceConfig>(); }
        }
    }
}

          