using IPP.InventoryMgmt.JobV31.Biz;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.InventoryMgmt.JobV31.Providers
{
    public class ServiceJobProviderVirtualLauncher:IJobAction
    {
        #region IJobAction Members

        public void Run(JobContext context)
        {
            VirtualRequestRunBP runBP = new VirtualRequestRunBP();
            runBP.Process(context);
        }

        #endregion
    }
}
