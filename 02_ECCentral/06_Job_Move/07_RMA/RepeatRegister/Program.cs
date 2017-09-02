using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.Oversea.CN.ServiceMgmt.Job.CalculateRepeat
{
    class Program
    {
        static void Main(string[] args)
        {
            CalculateRepeat c = new CalculateRepeat();
            JobContext context = new JobContext();
            context.Properties = new Dictionary<string, string>();
            c.Run(context);
        }
    }
}
