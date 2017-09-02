/***********************************************************************
 *  Copyright (C) 2009 Newegg Corporation
 *  All rights reserved.
 *  
 *  Author:  Phoebe Zhang (Phoebe.F.Zhang@Newegg.com)
 *  Date:    2009-05-20 17:28:55
 *  Usage: 
 *  
 *  RevisionHistory
 *  Date         Author               Description
 *  
 * ***********************************************************************/

using System.Configuration;
using System.Collections;
using System;

namespace IPP.Oversea.CN.ContentManagement.BizProcess.Common
{    /// <summary>
    /// Summary description for AppConfig.
    /// </summary>
    public static class AppConfig
    {
        static AppConfig()
        {

        }

        public static string ErrorLogFolder
        {
            get
            {
                return ConfigurationManager.AppSettings["ErrorLogFolder"];
            }
        }

        public static string CompanyCode
        {
            get
            {
                return ConfigurationManager.AppSettings["CompanyCode"] ?? "8601";
            }
        }
    }
}
