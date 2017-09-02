using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.InventoryMgmt.JobV31.BIZ;

namespace IPP.InventoryMgmt.JobV31
{
    public class JobAutoRun : IJobAction
    {
        public void Run(JobContext context)
        {
            //string option = string.Empty;
            //try
            //{
            //    option = context.Properties["option"];
            //}
            //catch (Exception)
            //{
            //}
            //if (option == "2")
            //{
            ThirdPartInventoryQtyBP bp = new ThirdPartInventoryQtyBP();
            bp.Run(context);
            //}
            //else
            //{
            //    InventoryQtyBP bp = new InventoryQtyBP();
            //    bp.Run(context);
            //}
        }

    }
}
