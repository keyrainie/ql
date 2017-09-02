using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WMSWaitInStock
{
    class Program
    {
        static void Main(string[] args)
        {
            WMSInStockJob j = new WMSInStockJob();
            j.Run(null);
        }
    }
}
