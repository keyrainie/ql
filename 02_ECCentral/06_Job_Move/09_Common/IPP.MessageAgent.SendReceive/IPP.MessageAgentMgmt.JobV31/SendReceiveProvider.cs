using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.MessageAgent.SendReceive.JobV31.Biz;

namespace IPP.MessageAgent.SendReceive.JobV31
{
    public class SendReceiveProvider : IJobAction
    {
        public void Run(JobContext context)
        {
            SSBProcessBP.RunProcess(context);
        }
    }
}
