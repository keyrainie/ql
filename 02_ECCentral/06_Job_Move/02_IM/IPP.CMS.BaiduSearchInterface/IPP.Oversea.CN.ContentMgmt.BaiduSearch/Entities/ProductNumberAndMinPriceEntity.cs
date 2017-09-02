using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.Oversea.CN.ContentMgmt.Baidu.Entities
{
    public class ProductNumberAndMinPriceEntity
    {
        [DataMapping("ProductCount", DbType.Int32)]
        public int ProductCount { get; set; }

        [DataMapping("MinPrice", DbType.Decimal)]
        public decimal MinPrice { get; set; }
    }
}
