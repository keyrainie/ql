using System;
using System.Configuration;
using IPP.MktToolMgmt.GroupBuyingJob.Biz;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.MktToolMgmt.GroupBuyingJob
{
    class GroupBuyingProgram : IJobAction
    {
        string bizLog = ConfigurationManager.AppSettings["BizLog"];
        static void Main(string[] args)
        {
            try
            {
                string bizLog1 = ConfigurationManager.AppSettings["BizLog"];
                GroupBuyingBP.CheckGroupBuying(bizLog1);
            }
            catch (Exception ex)
            {
                string errorLog = ConfigurationManager.AppSettings["ErrorLog"];
                Log.WriteLog(ex.ToString(), errorLog);
                JobHelper.SendMail(ex.ToString());
            }
        }

        #region IJobAction Members

        public void Run(JobContext context)
        {
            GroupBuyingBP.jobContext = context;
            GroupBuyingBP.CheckGroupBuying(bizLog);
        }

        #endregion
    }
}
