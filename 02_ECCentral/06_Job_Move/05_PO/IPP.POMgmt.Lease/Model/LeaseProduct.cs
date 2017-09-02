using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Newegg.Oversea.Framework.Entity;

namespace AutoClose.Model
{
    public class LeaseProduct
    {



        [DataMapping("OrderSysNo", DbType.Int32)]
        public int OrderSysNo { get; set; }

        [DataMapping("IsConsign", DbType.Int32)]
        public int IsConsign { get; set; }

        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }

        [DataMapping("PMUserSysNo", DbType.Int32)]
        public int PMUserSysNo { get; set; }

        [DataMapping("Cost", DbType.Decimal)]
        public decimal Cost { get; set; }

        [DataMapping("VendorSysNo", DbType.Int32)]
        public int VendorSysNo { get; set; }

        [DataMapping("Quantity", DbType.Int32)]
        public int Quantity { get; set; }

        [DataMapping("TaxRate", DbType.Decimal)]
        public decimal TaxRate { get; set; }
       
    }
}
