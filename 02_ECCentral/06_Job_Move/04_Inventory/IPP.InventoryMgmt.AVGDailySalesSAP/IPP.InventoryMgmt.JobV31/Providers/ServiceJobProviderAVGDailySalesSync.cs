using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.InventoryMgmt.JobV31.Biz;

namespace IPP.InventoryMgmt.JobV31.Providers
{
    public class ServiceJobProviderAVGDailySalesSync : IJobAction
    {
        public void Run(JobContext context)
        {
            AVGDailySalesToSAPBP.Process(context);
        }

    }
}
