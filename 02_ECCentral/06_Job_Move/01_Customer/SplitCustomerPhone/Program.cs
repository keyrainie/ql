using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.Oversea.CN.CustomerMgmt.SplitPhone;

namespace Split_Customer_Phone
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            //Console.Write("Starting....");

            CustomerBIZ customer = new CustomerBIZ();
            customer.Run(null);
//            customer.StartWork();
            //Console.Write("Finish");
        }
    }
}
