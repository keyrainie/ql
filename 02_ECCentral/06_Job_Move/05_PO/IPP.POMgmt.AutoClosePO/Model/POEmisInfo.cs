using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Newegg.Oversea.Framework.Entity;

namespace AutoClose.Model
{
    public class POEmisInfo
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("PM_ReturnPointSysNo", DbType.Int32)]
        public int PM_ReturnPointSysNo { get; set; }

        [DataMapping("UsingReturnPoint", DbType.Decimal)]
        public decimal UsingReturnPoint { get; set; }

        [DataMapping("sumEimsCount", DbType.Decimal)]
        public decimal sumEimsCount { get; set; }

        [DataMapping("ReturnPointC3SysNo", DbType.Int32)]
        public int ReturnPointC3SysNo { get; set; }
    }
}
