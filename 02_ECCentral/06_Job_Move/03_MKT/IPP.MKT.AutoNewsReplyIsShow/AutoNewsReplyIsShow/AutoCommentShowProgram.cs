using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Configuration;
using IPP.CN.ECommerceMgmt.AutoCommentShow.Providers;
using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.CN.ECommerceMgmt.AutoCommentShow.Biz;

namespace IPP.CN.ECommerceMgmt.AutoCommentShow
{
    public class Program : IJobAction
    {
        private static Hashtable JobProviders = new Hashtable();

        static void Main(string[] args)
        {
            ServiceJobSection jobSection = (ServiceJobSection)ConfigurationManager.GetSection("ServiceJobSection");
            ServiceJobInfoCollection JobCollection = jobSection.JobCollection;
            ProviderSettingsCollection jobProviders = (ProviderSettingsCollection)jobSection.JobProviders;

            for (int i = 0; i < JobCollection.Count; i++)
            {
                ProviderSettings providerSetting = jobProviders[JobCollection[i].Provider];
                ServiceJobProvider provider = (ServiceJobProvider)ProvidersHelper.InstantiateProvider(providerSetting, typeof(ServiceJobProvider));

                if (!JobProviders.Contains(JobCollection[i].JobName))
                {
                    provider.JobInfo = JobCollection[i];
                    JobProviders.Add(JobCollection[i].JobName, provider);
                }
            }
            JobDetect();

        }

        protected static void OnStop()
        {
            JobProviders.Clear();
        }

        private static void JobDetect()
        {
            int repeattime = Convert.ToInt32(ConfigurationManager.AppSettings["RepeatTime"]);
            while (true)
            {
                Log.WriteLog("Start detect.", "Log\\ServiceInfo.txt", true);
                foreach (object key in JobProviders.Keys)
                {
                    ServiceJobProvider provider = (ServiceJobProvider)JobProviders[key.ToString()];

                    provider.StartJob();

                }
                Log.WriteLog("End detect.", "Log\\ServiceInfo.txt", true);
                Thread.Sleep(repeattime * 10000);
            }
        }

        #region IJobAction Members

        void IJobAction.Run(JobContext context)
        {
            ShowCommentBP.jobContext = context;
            ShowCommentBP.BizLogFile = ConfigurationManager.AppSettings["BizLogFile"];
            ShowCommentBP.CheckRemarkMode();
        }

        #endregion
    }
}
