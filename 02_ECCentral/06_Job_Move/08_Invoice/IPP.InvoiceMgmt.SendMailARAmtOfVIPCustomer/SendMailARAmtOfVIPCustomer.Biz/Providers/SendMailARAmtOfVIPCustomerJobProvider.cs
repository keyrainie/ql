using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using SendMailARAmtOfVIPCustomer.Biz.Processor;

namespace SendMailARAmtOfVIPCustomer.Biz.Providers
{
    public class SendMailARAmtOfVIPCustomerJobProvider : IJobAction
    {
        /// <summary>
        /// JON Console Run Point
        /// </summary>
        /// <param name="context"></param>
        public void Run(JobContext context)
        {
            SendMailARAmtOfVIPCustomerBP.Start(context);
        }
    }
}
