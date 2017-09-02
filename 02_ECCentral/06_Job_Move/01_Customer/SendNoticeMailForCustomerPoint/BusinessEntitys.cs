using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace SendNoticeMailForCustomerPoint
{
    public class ExpireEmail
    {
        [DataMapping("AvailablePoint", DbType.Int32)]
        public int AvailablePoint { get; set; }
        [DataMapping("InDate", DbType.DateTime)]
        public DateTime InDate { get; set; }
        [DataMapping("ExpireDate", DbType.DateTime)]
        public DateTime ExpireDate { get; set; }
        [DataMapping("Email", DbType.String)]
        public string Email { get; set; }
        [DataMapping("CustomerID", DbType.String)]
        public string CustomerID { get; set; }
    }
}
