using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.InventoryMgmt.JobV31.BIZ;
using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.InventoryMgmt.JobV31;

namespace IPP.InventoryMgmt.JobHost
{
    class Program
    {
        static void Main(string[] args)
        {
            string option = string.Empty;

            JobAutoRun job = new JobAutoRun();
            Console.WriteLine("请选择操作：1——同步库存；0——退出系统");
            //Console.WriteLine("请选择操作：1——同步库存[老版本]；2--同步库存[新版本]；0——退出系统");
            option = Console.ReadLine().Trim();
            JobContext context = new JobContext();
            context.Properties = new Dictionary<string, string>();
            context.Properties.Add("BizLog", @"Log\log.txt");

            while (option != "0")
            {
                context.Properties["option"] = option;

                job.Run(context);
                Console.WriteLine("请选择操作：1——同步库存；0——退出系统");
                //Console.WriteLine("请选择操作：1——同步库存[老版本]；2--同步库存[新版本]；0——退出系统");
                option = Console.ReadLine();
            }
        }
    }
}
