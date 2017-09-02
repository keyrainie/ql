using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECCentral.Job.Invoice.AutoRunSOFreight;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Processor pr = new Processor();
                JobContext context = new JobContext();
                pr.Run(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.Read();
            }
        }
    }
}
