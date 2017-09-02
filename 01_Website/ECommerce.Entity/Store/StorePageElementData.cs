using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Store
{
    public class StorePageElementData
    {
        public int SysNo { get; set; }

        public string ElementKey { get; set; }

        public int SellerSysNo { get; set; }

        public string DataValue { get; set; }

        public int InUserSysNo { get; set; }

        public string InUserName { get; set; }

        public int EditUserSysNo { get; set; }

        public string EditUserName { get; set; }

        public DateTime InDate { get; set; }

        public DateTime EditDate { get; set; }
    }
}
