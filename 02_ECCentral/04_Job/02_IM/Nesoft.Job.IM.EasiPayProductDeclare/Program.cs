using Newegg.Oversea.Framework.JobConsole.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nesoft.Job.IM.EasiPayProductDeclare
{
    class Program
    {
        static void Main(string[] args)
        {
            var jobContext = new JobContext();
            new Processor().Run(jobContext);
            Console.WriteLine(jobContext.Message);
            Console.ReadLine();
        }
    }
}
