using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace IPP.ECommerceMgmt.ServiceJob.Common
{
    class Settings
    {
        public static string UserDisplayName
        {
            get
            {
                return ConfigurationManager.AppSettings["UserDisplayName"];
            }
        }
        public static string UserLoginName
        {
            get
            {
                return ConfigurationManager.AppSettings["UserLoginName"];
            }
        }
        public static string CompanyCode
        {
            get
            {
                return ConfigurationManager.AppSettings["CompanyCode"];
            }
        }
        public static string StoreCompanyCode
        {
            get
            {
                return ConfigurationManager.AppSettings["StoreCompanyCode"];
            }
        }
        public static string StoreSourceDirectoryKey
        {
            get
            {
                return ConfigurationManager.AppSettings["StoreSourceDirectoryKey"];
            }
        }
        public static string RepeatTime
        {
            get
            {
                return ConfigurationManager.AppSettings["RepeatTime"];
            }
        } 
        
    }
}
