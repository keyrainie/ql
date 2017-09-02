using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.OrderMgmt.JobV31.Biz;

namespace IPP.OrderMgmt.JobV31.Providers
{
    public class ServiceJobProviderGroupByProcess : IJobAction
    {

        #region IJobAction Members

        public void Run(JobContext context)
        {
            GroupBuyProcessBP.Process();
        }

        #endregion
    }
}
