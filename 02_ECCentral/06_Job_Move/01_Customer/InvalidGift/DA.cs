using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.DataAccess;
using System.Xml.Linq;
using System.IO;
using Newegg.Oversea.Framework.Utilities;

namespace InvalidGift
{
    internal class DA
    {
        public static int UpdateCustomerGiftStatus()
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateStatus");

            int count = 0;
            count = command.ExecuteNonQuery();
            return count;
           
        }

      
    }
}