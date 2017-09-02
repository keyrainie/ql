using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Newegg.Oversea.Framework.Entity;

namespace MerchantCommissionSettle.Entities
{
    public sealed class Vendor
    {
        [DataMapping("SysNo",DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("PayPeriodType", DbType.Int32)]
        public int PayPeriodType { get; set; }
    }
}
