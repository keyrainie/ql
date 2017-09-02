using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Member
{
    public class ProducePriceNotifiyQueryFilter : QueryFilterBase
    {
        public int? SysNo { get; set; }

        public int? CustomerSysNo { get; set; }
    }
}
