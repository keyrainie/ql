using Newegg.Oversea.Framework.JobConsole.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nesoft.Job.WMS.SendAdjustToDataExchangeCenter
{
    class Program
    {
        static void Main(string[] args)
        {
            JobContext jobContext = new JobContext();
            new Processor().Run(jobContext);
            Console.Write(jobContext.Message);
            Console.Read();
        }
    }
}
