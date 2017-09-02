using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.OrderMgmt.JobV31.Biz.FetchDubiousUser;
using Newegg.Oversea.Framework.JobConsole.Client;

namespace IPP.OrderMgmt.JobV31.Providers
{
    public class JobV31ProviderSOFetchDubiousUser : IJobAction
    {
        public void Run(JobContext context)
        {
            SOFetchDubiousUserBP.FetchUser(context);
        }
    }
}
