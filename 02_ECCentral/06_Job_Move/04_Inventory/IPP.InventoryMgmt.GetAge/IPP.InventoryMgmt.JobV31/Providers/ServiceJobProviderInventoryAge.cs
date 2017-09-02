using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.InventoryMgmt.JobV31.Biz.InventoryAge;


namespace IPP.InventoryMgmt.JobV31.Providers
{
    public class ServiceJobProviderInventoryAge:IJobAction 
    {
        public void Run(JobContext context)
        {
            InventoryAgeBP.SolveInventoryAge(context);
        }        
    }
}
