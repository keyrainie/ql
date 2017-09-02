using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
namespace GiveERPCustomerPoint
{
    /// <summary>
    /// Configuration Settings class for this App.
    /// </summary>
    public sealed class Settings
    {
        #region Constructors

        private Settings()
        {
        }

        #endregion

        /// <summary>
        /// LogFileName
        /// </summary>
        public static string LogFileName
        {
            get
            {
                return ConfigurationManager.AppSettings["LogFileName"];
            }
        }
    }
}
