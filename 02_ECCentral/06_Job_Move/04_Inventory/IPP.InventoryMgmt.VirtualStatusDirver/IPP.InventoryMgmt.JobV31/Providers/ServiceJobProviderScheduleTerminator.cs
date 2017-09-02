using IPP.InventoryMgmt.JobV31.Biz;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.InventoryMgmt.JobV31.Providers
{
    public class ServiceJobProviderScheduleTerminator:IJobAction
    {

        #region IJobAction Members

        public void Run(JobContext context)
        {
            VirtualRequestExpireBP expireBP = new VirtualRequestExpireBP();
            expireBP.Process(context);
        }

        #endregion
    }
}
