using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using System.Configuration;
using System.Threading;
using System.Web.Configuration;
using IPP.ECommerceMgmt.ServiceJob.Providers;
using IPP.ECommerceMgmt.ServiceJob.Common;

namespace ServiceJobConsole
{
    class Program
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

            //Thread threadJobDetect = new Thread(new ThreadStart(JobDetect));
            //threadJobDetect.Start();
        }

        protected static void OnStop()
        {
            JobProviders.Clear();
        }

        private static void JobDetect()
        {
            int RepeatTime =Convert.ToInt32(Settings.RepeatTime);
            while (true)
            {
            //Log.WriteLog("Start detect.", "Log\\ServiceInfo.txt", true);
            foreach (object key in JobProviders.Keys)
            {
                ServiceJobProvider provider = (ServiceJobProvider)JobProviders[key.ToString()];
                provider.StartJob();
            }
            //Log.WriteLog("End detect.", "Log\\ServiceInfo.txt", true);
             Thread.Sleep(RepeatTime*1000);
            }
        }
    }
}
