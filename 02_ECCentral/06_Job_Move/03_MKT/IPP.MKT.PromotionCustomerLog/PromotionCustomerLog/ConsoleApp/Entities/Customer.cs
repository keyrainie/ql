using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;

namespace IPP.EcommerceMgmt.SendCouponCode.Entities
{
    [Serializable]
    public class Customer
    {  
        [DataMapping("CustomerSysNo", DbType.Int32)]
        public int CustomerSysNo { get; set; }

        [DataMapping("CustomerName", DbType.String)]
        public string CustomerName { get; set; }

        [DataMapping("CustomerID", DbType.String)]
        public string CustomerID { get; set; }

        [DataMapping("MailAddress", DbType.String)]
        public string MailAddress { get; set; }

        [DataMapping("RegisterTime", DbType.DateTime)]
        public DateTime RegisterTime { get; set; }

        [DataMapping("Birthday", DbType.DateTime)]
        public DateTime  Birthday { get; set; }

        [DataMapping("SOSysNo", DbType.Int32)]
        public int SOSysNo { get; set; }

        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }
    }
}
