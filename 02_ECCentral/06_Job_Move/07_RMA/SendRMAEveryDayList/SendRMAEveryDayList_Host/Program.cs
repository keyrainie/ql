using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace IPP.Oversea.CN.CustomerMgmt.SendRMAEveryDayList
{
    class Program 
    {
        //public static string[] ReportNames = new string[] { "RMARequestStatus", "RMAOutBoundStatus", "RMARefundStatus", "RMARevertStatus", "RMAReturnStatus", "RMARequestStatus21" };

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Starting....");


            
            new Biz().DoJob();

            Console.ReadLine();
        }

        
    }
}
