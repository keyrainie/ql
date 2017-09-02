using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.OrderMgmt.JobV31.Providers
{
    public class JobV31ProviderSOAutoAuditSendMessage : IJobAction
    {
        public void Run(JobContext context)
        {
            //SOAutoAuditSendMessageBP.SendMessage(context);  
            //已集成在SOAutoAuditJob中了
            throw new NotImplementedException();
        }
    }
}
