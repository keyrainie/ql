using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using System.Configuration;

namespace IPP.OrderMgmt.Job.Biz
{
    /// <summary>
    /// Abstract job instance, if necessary, pls re-define the name,
    /// description and excuted time.
    /// </summary>
    public abstract class JobInstance : IJobAction
    {
        /// <summary>
        /// define the job name.
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// define the job description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// define the job start time
        /// </summary>
        public string ExcuteTime { get; set; }

        /// <summary>
        /// get the job company code
        /// </summary>
        public string CompanyCode 
        {
            get { return ConfigurationManager.AppSettings["CompanyCode"]; }
        }

        /// <summary>
        /// job logic
        /// </summary>
        public virtual void Run(JobContext context)
        {
            context.Message = string.Empty;
            #if(DEBUG)
            context.Properties = new Dictionary<string, string>();
            context.Properties["ComplainOutdatedAlertMailList"] = "Dan.R.Huang@newegg.com.cn";
            context.Properties["AstraZenecaComplainOutdatedAlertMailList"] = "Dan.R.Huang@newegg.com.cn";
            context.Properties["ExpiredSOMailList"] = "cindy.s.xin@newegg.com.cn";
            #endif
        }

        /// <summary>
        /// show the job description, if run in winform or asp.net pls 
        /// overwritte this method.
        /// </summary>
        public virtual void GetDescription()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine();
            Console.WriteLine("Job Name       :" + JobName);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Job Description:" + Description);
            Console.WriteLine("Job Start time :" + ExcuteTime);
            Console.WriteLine("--------------------------------------");
        }
    }
}
