using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newegg.Oversea.Framework.JobConsole.Client;
using Newegg.Oversea.Framework.ServiceConsole.Client;
using Newegg.Oversea.Framework.Biz;

namespace InvalidGift
{
    public class Biz : IJobAction
    {
        public void Run(JobContext context)
        {
           
            DoJob();
         
        }
        #region 
   
        public void DoJob()
        {
            DA.UpdateCustomerGiftStatus();

        }        

        #endregion

    }
}