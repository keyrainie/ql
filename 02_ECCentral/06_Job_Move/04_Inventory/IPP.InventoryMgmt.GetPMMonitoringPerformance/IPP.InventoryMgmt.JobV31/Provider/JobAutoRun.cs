using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.InventoryMgmt.JobV31.BIZ;

namespace IPP.InventoryMgmt.JobV31
{
    public class JobAutoRun : IJobAction
    {
        public void Run(JobContext context)
        {
            PMMPIInventoryBP bp = new PMMPIInventoryBP();
            bp.Run(context);      
        }
    }
}
