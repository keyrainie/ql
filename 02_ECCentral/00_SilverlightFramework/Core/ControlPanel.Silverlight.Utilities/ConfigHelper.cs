using System;
using System.Collections.Generic;
using System.Windows.Resources;
using System.Windows;
using System.Linq;
using System.Xml.Linq;

namespace Newegg.Oversea.Silverlight.Utilities
{
    public class ConfigHelper
    {
        private Dictionary<string, string> mSettings;

        private ConfigHelper(string configFileName)
        {
            StreamResourceInfo configStream = Application.GetResourceStream(new Uri(configFileName, UriKind.Relative));
            if (configStream == null)
            {
                throw new System.IO.FileNotFoundException(
                    string.Format("There is not any configuration file in xap package which name is {0}", configFileName));
            }
            mSettings = new Dictionary<string, string>();
            XDocument doc = XDocument.Load(configStream.Stream);
            var settings = from node in doc.Descendants("add") 
                           select new { Key = node.Attribute("key").Value, Value = node.Attribute("value").Value };
            foreach (var setting in settings)
            {
                mSettings.Add(setting.Key, setting.Value);
            }
        }

        private static Dictionary<string,ConfigHelper> sConfigs;

        static ConfigHelper()
        {
            sConfigs = new Dictionary<string, ConfigHelper>();
        }

        public static ConfigHelper GetConfig(string configFileName)
        {
            if (!sConfigs.ContainsKey(configFileName))
            {
                sConfigs.Add(configFileName, new ConfigHelper(configFileName));
            }
            return sConfigs[configFileName];
        }

        public static ConfigHelper GetConfig()
        {
            return GetConfig("Client.config");
        }

        public Dictionary<string, string> Settings
        {
            get
            {
                return mSettings;
            }
        }
    }
}
