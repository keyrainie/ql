using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;

namespace IPP.EcommerceMgmt.SendCouponCode.Entities
{
    [Serializable]
    public class CouponCodeCustomerLog : DetailBaseEntity
    {
        [DataMapping("CouponCode", DbType.AnsiString)]
        public string CouponCode { get; set; }

        [DataMapping("CouponSysNo", DbType.Int32)]
        public int CouponSysNo { get; set; }

        [DataMapping("CustomerSysNo", DbType.Int32)]
        public int CustomerSysNo { get; set; }

        [DataMapping("GetCouponCodeDate", DbType.DateTime)]
        public DateTime GetCouponCodeDate { get; set; }

        public string UserCodeType { get; set; }

        [DataMapping("SOSysNo", DbType.Int32)]
        public int SOSysNo { get; set; }
    }
}
