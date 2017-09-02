using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using ECCentral.Job.SO.SIMUnicomSO.Provider;

namespace ECCentral.Job.SO.SIMUnicomSO.JobHost
{
   public class Program
    {
        static void Main(string[] args)
        {
            string input = string.Empty;
            Console.WriteLine("请选择操作：1、启动JOB    0、退出JOB");
            input = Console.ReadLine();
            JobAutoRun run = new JobAutoRun();
            JobContext context = new JobContext();
            while (input == "1")
            {
                run.Run(context);
                Console.WriteLine("请选择操作：1、启动JOB    0、退出JOB");
                input = Console.ReadLine();
            }
        }
    }
}
