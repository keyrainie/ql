/***********************************************************************
 *  Copyright (C) 2009 Newegg Corporation
 *  All rights reserved.
 *  
 *  Author:  Sanford Ma ("Sanford.Y.Ma@Newegg.com)
 *  Date:    2009-06-09 13:07:39
 *  Usage: 
 *  
 *  RevisionHistory
 *  Date         Author               Description
 *  
 * ***********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace IPP.Oversea.CN.ContentMgmt.TimelyPromotionTitle.BizProcess
{
    public static class LoggerManager
    {
        public static TxtFileLogger GetLogger()
        {
            TxtFileLogger logger = null;
            string logFileName = ConfigurationManager.AppSettings["LogFileName"].ToString();
            logger = new TxtFileLogger(logFileName);
            return logger;
        }
        public static TxtFileLogger GetLogger(string logFileName)
        {
            return new TxtFileLogger(logFileName);
        }
    }
}
