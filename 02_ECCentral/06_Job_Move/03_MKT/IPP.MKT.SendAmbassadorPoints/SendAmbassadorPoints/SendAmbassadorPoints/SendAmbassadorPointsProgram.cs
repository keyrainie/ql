using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.ECommerceMgmt.SendAmbassadorPoints.Providers;
using System.Configuration;
using IPP.ECommerceMgmt.SendAmbassadorPoints;
using System.Threading;
using System.Collections;
using System.Web.Configuration;
using IPP.ECommerceMgmt.SendAmbassadorPoints.Biz;
using IPP.ECommerceMgmt.SendAmbassadorPoints.DA;

namespace IPP.ECommerceMgmt.SendAmbassadorPoints
{
    public class SendAmbassadorPointsProgram : IJobAction
    {
        //for local test
        private static void Main()
        {
            SendAmbassadorPointsBP.CheckAmbassadorOrder();
        }

        #region IJobAction Members
        public void Run(JobContext context)
        {
            SendAmbassadorPointsBP.jobContext = context;
            SendAmbassadorPointsBP.CheckAmbassadorOrder();
        }
        #endregion

    }
}
