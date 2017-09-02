using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Common
{
    public class PayTypeInfo
    {
        public int SysNo { get; set; }

        public string PayTypeID { get; set; }

        public string PayTypeName { get; set; }

        public decimal PayRate { get; set; }

        public bool IsNet { get; set; }

        public int IsOnlineShow { get; set; }
    }
}
