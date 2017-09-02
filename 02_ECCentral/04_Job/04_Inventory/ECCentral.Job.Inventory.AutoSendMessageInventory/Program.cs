using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.Job.Inventory.AutoSendMessageInventory
{
    class Program
    {
        static void Main(string[] args)
        {
            AutoSendMessageInventory.SendMessageInventory();
            Console.Read();
        }
    }
}
