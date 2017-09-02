using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Web.Configuration;
using System.Collections;
using System.Threading;
using IPP.ContentMgmt.SendMailForPicture.Properties;
using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.ContentMgmt.SendMailForPicture.Biz;
using Newegg.Oversea.Framework.DataAccess;
using IPP.ContentMgmt.SendMailForPicture.Providers;

namespace IPP.ContentMgmt.SendMailForPicture
{
    public class OnlineListProgram:IJobAction
    {
        private static Hashtable JobProviders = new Hashtable();

        static void Main(string[] args)
        {
            Product_StatusBP.BizLogFile = "Log\\biz2.log";
            Product_StatusBP.CheckProduct_StatusItem();
        }

        private static void JobDetect()
        {
            Log.WriteLog("Start detect.", "Log\\ServiceInfo.txt", true);
            foreach (object key in JobProviders.Keys)
            {
                ServiceJobProvider provider = (ServiceJobProvider)JobProviders[key.ToString()];
                provider.StartJob();
            }
            Log.WriteLog("End detect.", "Log\\ServiceInfo.txt", true);
        }
        public void Run(JobContext context)
        {
            Product_StatusBP.jobContext = context;
            Product_StatusBP.BizLogFile = ConfigurationManager.AppSettings["BizLogFile"];
            Product_StatusBP.CheckProduct_StatusItem();
        }
    }
}
