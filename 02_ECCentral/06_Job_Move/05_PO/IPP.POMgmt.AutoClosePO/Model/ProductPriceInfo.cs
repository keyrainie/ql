using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Newegg.Oversea.Framework.Entity;

namespace AutoClose.Model
{
    public class ProductPriceInfo
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("Id", DbType.String)]
        public string Id { get; set; }

        [DataMapping("CurrentPrice", DbType.Decimal)]
        public decimal? CurrentPrice { get; set; }

        [DataMapping("Point", DbType.Decimal)]
        public decimal? Point { get; set; }

        [DataMapping("UnitCost", DbType.Decimal)]
        public decimal? UnitCost { get; set; }

        [DataMapping("C3SysNo", DbType.Int32)]
        public int? C3SysNo { get; set; }

        [DataMapping("LastPrice", DbType.Decimal)]
        public decimal? LastPrice { get; set; }
    }
}
