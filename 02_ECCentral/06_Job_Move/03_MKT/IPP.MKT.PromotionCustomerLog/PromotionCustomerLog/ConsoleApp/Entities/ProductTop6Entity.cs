using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.EcommerceMgmt.SendCouponCode.Entities
{
    [Serializable]
    public class ProductTop6Entity
    {
        [DataMapping("ProductID", DbType.String)]
        public String ProductID { get; set; }

        [DataMapping("DefaultImage", DbType.String)]
        public String DefaultImage { get; set; }

        [DataMapping("ProductTitle", DbType.String)]
        public String ProductTitle { get; set; }

        [DataMapping("CurrentPrice", DbType.String)]
        public Decimal CurrentPrice { get; set; }
    }
}
