using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using System.Configuration;
using System.Threading;

using System.Web.Configuration;
using IPP.OrderMgmt.ServiceJob.Providers;
using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.OrderMgmt.ServiceJob.Biz.SecKill;


namespace ServiceJobConsole
{
    class Program : IJobAction
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
            int repeattime =Convert.ToInt32(ConfigurationManager.AppSettings["RepeatTime"]);
            while (true)
            {
                //Log.WriteLog("Start detect.", "Log\\ServiceInfo.txt", true);
                foreach (object key in JobProviders.Keys)
                {
                    ServiceJobProvider provider = (ServiceJobProvider)JobProviders[key.ToString()];
                   // if (provider.CheckRunTime())
                   // {
                        provider.StartJob();
                    //}
                }
                //Log.WriteLog("End detect.", "Log\\ServiceInfo.txt", true);
                Thread.Sleep(repeattime*1000);
            }
        }

        #region IJobAction Members

        public void Run(JobContext context)
        {
            SecKillBP.context = context;
            SecKillBP.CheckCountDownSecKill("Log\\CountdownSecKill_Biz.txt");
        }

        #endregion
    }
}
