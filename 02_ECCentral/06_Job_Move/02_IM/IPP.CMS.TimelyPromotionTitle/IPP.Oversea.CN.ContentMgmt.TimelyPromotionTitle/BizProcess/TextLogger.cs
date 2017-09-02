/***********************************************************************
 *  Copyright (C) 2009 Newegg Corporation
 *  All rights reserved.
 *  
 *  Author:  Sanford Ma ("Sanford.Y.Ma@Newegg.com)
 *  Date:    2009-06-09 13:06:28
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
using System.IO;

namespace IPP.Oversea.CN.ContentMgmt.TimelyPromotionTitle.BizProcess
{
    /***********************************************************************
 *  Copyright (C) 2009 Newegg Corporation
 *  All rights reserved.
 *  
 *  Author:  Sanford Ma ("Sanford.Y.Ma@Newegg.com)
 *  Date:    2009-04-24 17:52:30
 *  Usage: 
 *  
 *  RevisionHistory
 *  Date         Author               Description
 *  
 * ***********************************************************************/

    public class TxtFileLogger
    {
        #region Member Variables

        private string logFilePath;
        private string logFileName;

        #endregion

        #region Constructors

        public TxtFileLogger(string logFileName)
        {
            string currentDir = System.Threading.Thread.GetDomain().BaseDirectory;
            this.logFileName = logFileName.Replace("/", "\\").Trim('\\');
            this.logFilePath = Path.Combine(currentDir, string.Format("log\\{0}", logFileName));
        }

        #endregion

        #region ILog Members

        public void WriteLog(string context)
        {
            Console.WriteLine(context);

            if (!string.IsNullOrEmpty(logFilePath))
            {
                FileStream fileStream = null;
                if (File.Exists(logFilePath))
                {
                    fileStream = new FileStream(logFilePath, FileMode.Append, FileAccess.Write);
                }
                else
                {
                    Directory.CreateDirectory(logFilePath.Substring(0, logFilePath.LastIndexOf("\\")));
                    fileStream = new FileStream(logFilePath, FileMode.Create, FileAccess.Write);
                }
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    //writer.WriteLine("################################ " + DateTime.Now.ToString() + " #############################");
                    writer.WriteLine(context);
                    writer.Flush();

                    writer.Close();
                }

                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        public void WriteLog(Exception exception)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Exception:  {0}{1}", DateTime.Now.ToString(), Environment.NewLine);
            sb.AppendFormat("Message:{0}", exception.Message);
            sb.AppendLine();
            sb.AppendFormat("Source:{0}", exception.Source);
            sb.AppendLine();
            sb.AppendFormat("TargetSite:{0}", exception.TargetSite);
            sb.AppendLine();
            sb.AppendFormat("StackTrace:{0}", exception.StackTrace);
            sb.AppendLine();
            sb.AppendLine();

            string exceptionContent = sb.ToString();

            WriteLog(exceptionContent);
        }

        #endregion
    }
}
