using System.Collections;
using System.Configuration;
using System.ServiceProcess;
using System.Threading;
using System.Web.Configuration;
using IPP.ECommerceMgmt.ServiceJob.Properties;
using IPP.ECommerceMgmt.ServiceJob.Providers;

namespace IPP.ECommerceMgmt.ServiceJob
{
    public partial class Service : ServiceBase
    {
        public Service()
        {
            InitializeComponent();
        }

        private Hashtable JobProviders = new Hashtable();

        protected override void OnStart(string[] args)
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

            Thread threadJobDetect = new Thread(new ThreadStart(this.JobDetect));
            threadJobDetect.Start();

            Log.WriteLog(Resources.ServiceStarted, "Log\\ServiceInfo.txt", true);
        }

        protected override void OnStop()
        {
            JobProviders.Clear();
            Log.WriteLog(Resources.ServiceStopped, "Log\\ServiceInfo.txt", true);
        }

        private void JobDetect()
        {
            while (true)
            {
                //Log.WriteLog("Start detect.", "Log\\ServiceInfo.txt", true);
                foreach (object key in JobProviders.Keys)
                {
                    ServiceJobProvider provider = (ServiceJobProvider)JobProviders[key.ToString()];
                  //  if (provider.CheckRunTime())
                  //  {
                        provider.StartJob();
                  //  }
                }
                //Log.WriteLog("End detect.", "Log\\ServiceInfo.txt", true);
                Thread.Sleep(60000);
            }
        }
    }
}
