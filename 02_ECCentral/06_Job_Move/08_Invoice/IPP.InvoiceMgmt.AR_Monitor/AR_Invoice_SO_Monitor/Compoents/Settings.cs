using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;


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
        public static string SOBeginDate
        {
            get
            {
                return ConfigurationManager.AppSettings["SOBeginDate"];
            }
        }
        public static string[] PayByJiFen
        {
            get
            {
                return ConfigurationManager.AppSettings["PayByJiFen"].Split('|');
            }
        }
    }

