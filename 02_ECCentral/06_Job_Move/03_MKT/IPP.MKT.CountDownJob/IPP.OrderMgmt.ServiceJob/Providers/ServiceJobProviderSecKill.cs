using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.OrderMgmt.ServiceJob.Biz.SecKill;

namespace IPP.OrderMgmt.ServiceJob.Providers
{
    public class ServiceJobProviderSecKill : ServiceJobProvider
    {
        public override void PostData()
        {
            SecKillBP.CheckCountDownSecKill(this.JobInfo.BizLog);
        }
    }
}
