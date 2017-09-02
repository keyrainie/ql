using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.OrderMgmt.JobV31.Biz.SendMail;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.OrderMgmt.JobV31.Providers
{
    public class ServiceJobProviderSOSendAlarmMail : IJobAction
    {
        public void Run(JobContext context)
        {
            SOSendAlarmMailBP.Check(context);
        }
    }
}
