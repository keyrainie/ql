using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;

namespace IPP.EcommerceMgmt.SendCouponCode.Entities
{
    [Serializable]
    public class CouponSaleRule : DetailBaseEntity
    {
        [DataMapping("CouponSysNo", DbType.Int32)]
        public int CouponSysNo { get; set; }

        [DataMapping("Type", DbType.String)]
        public string Type { get; set; }    

        [DataMapping("CustomerRank", DbType.Int32)]
        public int CustomerRank { get; set; }     

    }
}
