using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
namespace IPPToSAP
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
        public static string CompanyCode
        {
            get
            {
                return ConfigurationManager.AppSettings["CompanyCode"];
            }
        }
        public static string EmailAddress
        {
            get
            {
                return ConfigurationManager.AppSettings["EmailAddress"];
            }
        }

        public static string MailSubject
        {
            get
            {
                return ConfigurationManager.AppSettings["MailSubject"];
            }
        }
    }
}
