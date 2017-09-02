using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using ECCentral.Job.SO.SIMUnicomSO.Logic;

namespace ECCentral.Job.SO.SIMUnicomSO.Provider
{
   public class JobAutoRun:IJobAction 
    {
       public void Run(JobContext context) {
           UnicomSOBP.Run();
       }
    }
}
