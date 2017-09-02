using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.Oversea.CN.InventoryMgmt.JobV31.Biz;

namespace IPP.Oversea.CN.InventoryMgmt.JobV31.Provider
{
    public class JobAutoRun : IJobAction
    {
        #region IJobAction Members

        public void Run(JobContext context)
        {
            ChannelInventoryBP.Run(context);
        }

        #endregion
    }
}
