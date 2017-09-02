using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;

namespace IPP.OrderMgmt.ERPOrderStatus
{
    public class ErpOrder
    {
        [DataMapping("RefOrderNo", DbType.String)]
        public string RefOrderNo { get; set; }

        [DataMapping("OrderAmt", DbType.Decimal)]
        public decimal OrderAmt { get; set; }
    }
}
