using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.EcommerceMgmt.SendCouponCode.Biz;
using IPP.EcommerceMgmt.SendCouponCode.Entities;
using IPP.EcommerceMgmt.SendCouponCode.DA;
using IPP.EcommerceMgmt.SendCouponCode;
using System.Configuration;

namespace MktToolMgmt.PromotionCustomerLogApp
{
    class Program : IJobAction
    {
        static void Main(string[] args)
        {
            BizProcess.BizLogFile = ConfigurationManager.AppSettings["BizLogFile"];
            try
            {
                BizProcess.WriteLog("\r\n********************** Begin *************************");
                BizProcess.Process();
                BizProcess.WriteLog("********************** End *************************\r\n");
                //Console.ReadLine();
            }
            catch (Exception ex)
            {
                string errorDetail = ex.ToString();
                Console.WriteLine(errorDetail);

            }
        }

        #region IJobAction Members

        public void Run(JobContext context)
        {
            BizProcess.BizLogFile = ConfigurationManager.AppSettings["BizLogFile"];
            BizProcess.Process();
            // biz.jobContext = context;           
        }

        #endregion
    }
}
