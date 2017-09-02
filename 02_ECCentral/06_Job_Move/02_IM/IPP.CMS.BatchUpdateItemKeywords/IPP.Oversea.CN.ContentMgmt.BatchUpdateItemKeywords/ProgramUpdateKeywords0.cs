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
using System.Collections;
using System.Configuration;
using IPP.ContentMgmt.BatchUpdateItemKeywords.Providers;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.ContentMgmt.BatchUpdateItemKeywords
{
    class ProgramUpdateKeywords0 : IJobAction
    {

        private static Hashtable JobProviders = new Hashtable();

        static void Main(string[] args)
        {
            BatchUpdateItemKeywordsBP.BizLogFile = "Log\\biz2.log";
            //if (args != null && args.Any())
            //{
            //    int productId;
            //    if (int.TryParse(args[0], out productId))
            //    {
            //        BatchUpdateItemKeywordsBP.UpdateKeywords0ByProductSysNo(productId);
            //    }
            //    else if (args[0].ToLower() == "updatekeywords0")
            //    {
            //        BatchUpdateItemKeywordsBP.UpdateKeywords0();
            //    }
            //    else if (args[0].ToLower() == "updatekeywords0fromqueue")
            //    {
            //        BatchUpdateItemKeywordsBP.UpdateKeywords0Queue();
            //    }
            //    else
            //    {
            //        Console.WriteLine("非法的参数");
            //    }
            //}
            BatchUpdateItemKeywordsBP.UpdateKeywords0();
        }


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
            BatchUpdateItemKeywordsBP.UpdateKeywords0();
        }

        #endregion
    }
}
