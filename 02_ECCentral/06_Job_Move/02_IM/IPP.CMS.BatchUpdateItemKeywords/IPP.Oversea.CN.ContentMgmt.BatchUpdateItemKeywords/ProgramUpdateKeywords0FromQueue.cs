using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.Oversea.CN.ContentManagement.BizProcess.Common;
using IPP.ContentMgmt.BatchUpdateItemKeywords.BizProcess;
using IPP.ContentMgmt.BatchUpdateItemKeywords.Utility;
using Newegg.Oversea.Framework.ExceptionBase;
using Newegg.Oversea.Framework.ExceptionHandler;
using System.Xml.Linq;
using System.Configuration;
using IPP.ContentMgmt.BatchUpdateItemKeywords.Providers;
using System.Collections;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.ContentMgmt.BatchUpdateItemKeywords
{
    class ProgramUpdateKeywords0FromQueue : IJobAction
    {
        private static Hashtable JobProviders = new Hashtable();

        private static void JobDetect()
        {
            Log.WriteLog("Start detect.", "Log\\ServiceInfo.txt", true);
            foreach (object key in JobProviders.Keys)
            {
                ServiceJobProvider provider = (ServiceJobProvider)JobProviders[key.ToString()];
                provider.StartJob();
            }
            Log.WriteLog("End detect.", "Log\\ServiceInfo.txt", true);
        }

        #region IJobAction Members

        public void Run(JobContext context)
        {
            BatchUpdateItemKeywordsBP.jobContext = context;
            BatchUpdateItemKeywordsBP.BizLogFile = ConfigurationManager.AppSettings["BizLogFile"];
            BatchUpdateItemKeywordsBP.UpdateKeywords0Queue();
        }

        #endregion
    }
}
