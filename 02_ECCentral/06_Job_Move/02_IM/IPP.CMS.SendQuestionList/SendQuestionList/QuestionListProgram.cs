using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Web.Configuration;
using System.Collections;
using System.Threading;
using IPP.ContentMgmt.SendQuestionList.Providers;
using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.ContentMgmt.SendQuestionList.Biz;

namespace IPP.ContentMgmt.SendQuestionList
{
    class QuestionListProgram : IJobAction
    {
        private static Hashtable JobProviders = new Hashtable();

        static void Main(string[] args)
        {
            QuestionListBP.BizLogFile = "Log\\biz2.log";
            QuestionListBP.CheckQuestionListItem();
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
            QuestionListBP.jobContext = context;
            QuestionListBP.BizLogFile = ConfigurationManager.AppSettings["BizLogFile"];
            QuestionListBP.CheckQuestionListItem();
        }

        #endregion
    }

}
