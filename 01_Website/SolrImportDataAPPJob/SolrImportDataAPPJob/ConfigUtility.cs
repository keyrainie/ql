using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace SolrImportDataAPPJob
{
    /// <summary>
    /// 
    /// </summary>
    public class ConfigUtility
    {
        /// <summary>
        /// Gets the configuration value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string GetConfigValue(string key)
        {
            string value = ConfigurationManager.AppSettings[key];
            return (string.IsNullOrEmpty(value) ? string.Empty : value);
        }
    }
}
