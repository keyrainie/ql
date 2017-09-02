using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.CN.ECommerceMgmt.AutoCommentShow.Biz;

namespace IPP.CN.ECommerceMgmt.AutoCommentShow.Providers
{
    public class ServiceJobProviderShowComment : ServiceJobProvider
    {
        public override void PostData()
        {
            ShowCommentBP.CheckRemarkMode();//this.JobInfo.BizLog
        }
    }
}
