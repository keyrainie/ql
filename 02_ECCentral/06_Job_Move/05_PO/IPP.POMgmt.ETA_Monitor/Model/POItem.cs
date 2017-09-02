using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPPOversea.POmgmt.Model
{
    public class POItem
    {
        [DataMapping("SysNo",DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("PurchaseQty", DbType.Int32)]
        public int PurchaseQty { get; set; }

        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }

        [DataMapping("ProductID", DbType.String)]
        public string ProductID { get; set; }
    }
}
