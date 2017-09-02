using IPP.InventoryMgmt.JobV31.Biz;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.InventoryMgmt.JobV31.Providers
{
    public class ServiceJobProviderStatusTerminator : IJobAction
    {
        #region IJobAction Members

        public void Run(JobContext context)
        {
            VirtualRequestCloseBP closeBP = new VirtualRequestCloseBP();
            closeBP.Process(context);
        }

        #endregion
    }
        


}
