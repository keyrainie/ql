using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Newegg.Oversea.Framework.Entity;


namespace IPPOversea.POmgmt.Model
{
    public class POEntity
    {
        [DataMapping("Sysno", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("TotalAmt", DbType.Decimal)]
        public decimal TotalAmt { get; set; }

        [DataMapping("ETATime", DbType.DateTime)]
        public DateTime? ETATime { get; set; }

        //[DataMapping("ETATime", DbType.Int32)]
        //public int ProductSysNo { get; set; }

        [DataMapping("PM_ReturnPointSysNo", DbType.Int32)]
        public int PM_ReturnPointSysNo { get; set; }

        [DataMapping("UsingReturnPoint", DbType.Decimal)]
        public decimal UsingReturnPoint { get; set; }

        [DataMapping("ReturnPointC3SysNo", DbType.Int32)]
        public int ReturnPointC3SysNo { get; set; }

        [DataMapping("StockSysNo", DbType.Int32)]
        public int StockSysNo { get; set; }

        [DataMapping("ETAHalfDay", DbType.String)]
        public string ETAHalfDay { get; set; }

        public List<POItem> Items { get; set; }
    }
}
