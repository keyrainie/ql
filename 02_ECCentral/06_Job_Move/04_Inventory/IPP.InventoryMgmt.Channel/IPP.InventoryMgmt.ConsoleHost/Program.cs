using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.Oversea.CN.InventoryMgmt.JobV31.Provider;
using Newegg.Oversea.Framework.JobConsole.Client;
using System.Net;

namespace IPP.InventoryMgmt.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
           
            Console.WriteLine("请选择操作：1、同步渠道库存，2、退出JOB");
            string result = Console.ReadLine();


            while (result == "1")
            {
                JobAutoRun job = new JobAutoRun();
                JobContext context = new JobContext();
                job.Run(context);
                Console.WriteLine();

                Console.WriteLine("请选择操作：1、同步渠道库存，2、退出JOB");
                result = Console.ReadLine();
            }
        }
    }
}
