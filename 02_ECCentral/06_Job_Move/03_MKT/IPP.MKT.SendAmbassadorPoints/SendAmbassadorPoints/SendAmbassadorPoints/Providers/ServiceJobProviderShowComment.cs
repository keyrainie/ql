using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IPP.EcommerceMgmt.SendAmbassadorPoints.Biz;

namespace IPP.ECommerceMgmt.SendAmbassadorPoints.Providers
{
    public class ServiceJobProviderShowComment : ServiceJobProvider
    {
        public override void PostData()
        {
            SendAmbassadorPointsBP.CheckAmbassadorOrder(this.JobInfo.BizLog);//
        }
    }
}
