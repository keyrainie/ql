using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using Nesoft.Job.WMS.AsyncCallAPIService.Core;
using Nesoft.Utility;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace Nesoft.Job.WMS.AsyncCallAPIService
{
    class Program
    {
        static void Main(string[] args)
        {
            new Processor().Run(new JobContext());
            Console.Read();
        }
    }
}
