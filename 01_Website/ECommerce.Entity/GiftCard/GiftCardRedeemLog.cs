using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.GiftCard
{
    public class GiftCardRedeemLog
    {
        public string Code { get; set; }

        public int CustomerSysNo { get; set; }

        public decimal ConsumeAmount { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
