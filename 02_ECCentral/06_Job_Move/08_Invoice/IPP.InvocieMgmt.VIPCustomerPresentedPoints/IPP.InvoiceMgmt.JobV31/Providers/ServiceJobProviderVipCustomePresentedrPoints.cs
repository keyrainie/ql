using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using InvoiceMgmt.JobV31.Biz;

namespace InvoiceMgmt.JobV31.Providers
{
    public class ServiceJobProviderVipCustomePresentedrPoints : IJobAction
    {
        public void Run(JobContext jobContext)
        {
            VIPCustomerPresentedPointsBP.PresentedPoints(jobContext);
        }
    }
}
