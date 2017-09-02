using Newegg.Oversea.Framework.JobConsole.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendMessage
{
    public class SendSMSJobAction : IJobAction
    {
        public void Run(JobContext context)
        {
            SmsSender.SendSmsMessage(context);
        }
    }
}
