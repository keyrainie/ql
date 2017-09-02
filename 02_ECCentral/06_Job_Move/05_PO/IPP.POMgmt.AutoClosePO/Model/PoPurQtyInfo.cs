using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace AutoClose.Model
{
    public class PoPurQtyInfo
    {
       // a.SysNo AS poSysno
       //,a.StockSysNo
       //,c.SysNo AS itemSysno
       //,c.ProductSysNo AS productSysno
       //,pruCount = c.PurchaseQty - c.AvailableQty
        [DataMapping("poSysno", DbType.Int32)]
        public int poSysno { get; set; }

        [DataMapping("StockSysNo", DbType.Int32)]
        public int StockSysNo { get; set; }

        [DataMapping("itemSysno", DbType.Int32)]
        public int itemSysno { get; set; }

        [DataMapping("pruCount", DbType.Int32)]
        public int pruCount { get; set; }

        [DataMapping("productSysno", DbType.Int32)]
        public int productSysno { get; set; }
    }
}
