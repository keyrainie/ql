using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.OrderMgmt.SendSOPayToEPortJob.Dac.Common;
using SendSOPayToEPortJob.BusinessEntities;
using System.Net;
using System.IO;
using SendSOPayToEPortJob.Utilities;
using System.Xml.Linq;
using SendSOPayToEPortJob.Dac.Common;

namespace IPP.OrderMgmt.SendSOPayToEPortJob.Providers
{
    public class SendSOPayToEPortJob : IJobAction
    {
        public void Run(JobContext context)
        {

            //电商企业代码
            string CompanyEPortNo = System.Configuration.ConfigurationManager.AppSettings["CompanyEPortNo"];
            List<int> SOIDList = CommonDA.GetOrderSysNoList(int.Parse(CompanyEPortNo));

            context = NingBoAPI.SentSo(context, SOIDList);
            Log.WriteLog(context.Message, "log.txt");

        }

        
    }
}
