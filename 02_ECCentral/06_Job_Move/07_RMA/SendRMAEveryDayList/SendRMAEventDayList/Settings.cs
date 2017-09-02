using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace IPP.Oversea.CN.CustomerMgmt.SendRMAEveryDayList
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

        #region AutoNotifyMailSender Settings

        /// <summary>
        /// LogFileName
        /// </summary>
        public static string LogFileName
        {
            get
            {
               return ConfigurationManager.AppSettings["LogFileName"];
                 //return "Log.txt";
            }
        }
        public static string ConString
        {
            get
            {
                return ConfigurationManager.AppSettings["ConString"];
            }
        }
        public static string CompanyCode
        {
            get
            {
                return ConfigurationManager.AppSettings["CompanyCode"];
            }
        }     
        #endregion
    }
}
