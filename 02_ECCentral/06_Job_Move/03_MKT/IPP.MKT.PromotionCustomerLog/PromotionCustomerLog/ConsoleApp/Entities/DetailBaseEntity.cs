using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Newegg.Oversea.Framework.Entity;
using System.ComponentModel;

namespace IPP.EcommerceMgmt.SendCouponCode.Entities
{
    [Serializable]
    public class DetailBaseEntity
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("InDate", DbType.DateTime)]
        public DateTime InDate { get; set; }

        [DataMapping("InUser", DbType.String)]
        public string InUser { get; set; }

        [DataMapping("EditDate", DbType.DateTime)]
        public DateTime EditDate { get; set; }

        [DataMapping("EditUser", DbType.String)]
        public string EditUser { get; set; }
    }
}
