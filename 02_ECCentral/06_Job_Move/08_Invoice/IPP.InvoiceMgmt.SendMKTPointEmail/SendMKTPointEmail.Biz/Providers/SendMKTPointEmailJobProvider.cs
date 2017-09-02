using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using SendMKTPointEmail.Biz.Processor;

namespace SendMKTPointEmail.Biz.Providers
{
    public class SendMKTPointEmailJobProvider : IJobAction
    {
        /// <summary>
        /// JON Console Run Point
        /// </summary>
        /// <param name="context"></param>
        public void Run(JobContext context)
        {
            SendMKTPointEmailBP.Start(context);
        }
    }
}
