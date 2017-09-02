using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;


namespace ECommerce.Entity.Promotion
{
    [Serializable]
    public class CouponCodeCustomerLog 
    {
        public string CouponCode { get; set; }

        public int CouponSysNo { get; set; }
        public int CustomerSysNo { get; set; }

        public DateTime GetCouponCodeDate { get; set; }

        public string UserCodeType { get; set; }

        public int SOSysNo { get; set; }
    }
}
