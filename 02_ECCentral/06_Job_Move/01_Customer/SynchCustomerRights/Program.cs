using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using System.Windows.Forms;


namespace IPP.Oversea.CN.CustomerMgmt.SynchCustomerRights
{
    class Program
    {
        static void Main(string[] args)
        {
         
            CustomerRightsTest form = new CustomerRightsTest();
            Application.Run(form); 
        }
    }
}
