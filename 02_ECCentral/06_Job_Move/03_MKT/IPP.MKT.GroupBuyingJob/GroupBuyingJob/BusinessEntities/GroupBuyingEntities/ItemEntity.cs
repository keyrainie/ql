using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using Newegg.Oversea.Framework.Entity;

namespace IPP.MktToolMgmt.GroupBuyingJob.BusinessEntities
{
    public class ItemEntity
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }

        [DataMapping("CashRebate", DbType.Decimal)]
        public decimal CashRebate { get; set; }

        [DataMapping("CurrentPrice", DbType.Decimal)]
        public decimal CurrentPrice { get; set; }


        [DataMapping("UnitCost", DbType.Decimal)]
        public decimal UnitCost { get; set; }

        [DataMapping("Point", DbType.Int32)]
        public int Point { get; set; }

        [DataMapping("MaxPerOrder", DbType.Int32)]
        public int MaxPerOrder { get; set; } 

    }
}
