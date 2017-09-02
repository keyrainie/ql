using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using ECCentral.Job.SO.SendAlarmMailJob.Biz.SendMail;

namespace ECCentral.Job.SO.SendAlarmMailJob.Providers
{
   public class ServiceJobProviderSOSendAlarmMail:IJobAction
    {
       public void Run(JobContext  context) {
           SOSendAlarmMailBP.Check(context);
       }
    }
}
