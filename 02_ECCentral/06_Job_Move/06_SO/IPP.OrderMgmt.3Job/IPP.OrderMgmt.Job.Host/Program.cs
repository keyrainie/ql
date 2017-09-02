using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.OrderMgmt.Job.Biz;

namespace IPP.OrderMgmt.Job.Host
{
    class Program
    {
        private static JobContext context = new JobContext();
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            ToDearLily();
            var selectJobs = SelectTheJobs();
            if (selectJobs.Count > 0)
            {
                var instances = new List<JobInstance>();
                selectJobs.ForEach(x =>
                {
                    instances.Add(AbstractJobFactory.GetJobInstance(x));
                });

                //Execute the jobs
                foreach (var job in instances)
                {
                    var context = new JobContext();
                    job.GetDescription();
                    job.Run(context);
                    Console.WriteLine(context.Message);
                }
            }

            Console.WriteLine();
            Console.WriteLine("---------------------");
            Console.WriteLine("The Job already done");
            Console.ReadLine();
        }

        /// <summary>
        /// get the select jobs
        /// </summary>
        /// <returns></returns>
        private static List<string> SelectTheJobs()
        {
            //Store the select the jobs
            Console.WriteLine("pls select the job you want to test:");
            var jobNames = AbstractJobFactory.GetTheAllJobNameFromAssmeble();
            var index = 1;
            //Show the names of the jobs
            jobNames.ForEach(x => 
            {
                Console.WriteLine("Job" + index + ": " + x);
                index++;
            });
            Console.WriteLine("Job"+ index + ": All");
            Console.WriteLine();
            try
            {
                var numbers = Console.ReadLine().Split(new char[] { ' ' });
                if (numbers.Length > 0)
                {
                    var selectJobs = new List<string>();
                    foreach(var key in numbers)
                    {
                        selectJobs.Add(jobNames[Convert.ToInt32(key)-1]);
                    }
                    return selectJobs;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Input error, pls re-select the jobs");
            }
            return new List<string>();
        }

        /// <summary>
        /// I have something to tell you lily 
        /// </summary>
        private static void ToDearLily()
        {
            var saySomething = "Dear Lily:\n";
            saySomething += "I know you are working well and study hard,";
            saySomething += "as the test leader, you do excellent job. ";
            saySomething += "I am very admire your responsibility and courage\n";
            saySomething += "Regards \n";
            saySomething += "Jay";
            Console.WriteLine(saySomething);
            Console.ReadLine();
        }
    }
}
