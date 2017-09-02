using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using IPP.ECommerceMgmt.ServiceJob.Biz;

namespace IPP.ECommerceMgmt.ServiceJob.Providers
{
    public class ServiceSODC : ServiceJobProvider
    {       
        public override void PostData()
        {
            AutoResetEvent are = new AutoResetEvent(false);
            SODCBP.DoWork(this.JobInfo.BizLog,this.JobInfo.ErrorLog,are);
            are.WaitOne();
        }       
    }
}
