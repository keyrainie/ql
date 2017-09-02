using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Web.Configuration;
using System.Collections;
using System.Threading;
using IPP.ECommerceMgmt.AutoInnerOnlineList.Providers;
using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.ECommerceMgmt.AutoInnerOnlineList.Biz;

namespace IPP.ECommerceMgmt.AutoInnerOnlineList
{
    class Program : IJobAction
    {
        private static Hashtable JobProviders = new Hashtable();

        static void Main(string[] args)
        {
            OnlinelistBP.BizLogFile = "Log\\biz2.log";
            OnlinelistBP.CheckOnlinelistItem();
            Thread threadJobDetect = new Thread(new ThreadStart(JobDetect));
            threadJobDetect.Start();
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
            OnlinelistBP.jobContext = context;
            OnlinelistBP.BizLogFile = ConfigurationManager.AppSettings["BizLogFile"];
            OnlinelistBP.CheckOnlinelistItem();
        }

        #endregion
    }

}
