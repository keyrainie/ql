using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Newegg.Oversea.Framework.Entity;

namespace AutoClose.Model
{
    public class CurrencyInfo
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("CurrencyID", DbType.String)]
        public string CurrencyID { get; set; }

        [DataMapping("CurrencyName", DbType.String)]
        public string CurrencyName { get; set; }

        [DataMapping("CurrencySymbol", DbType.String)]
        public string CurrencySymbol { get; set; }

        [DataMapping("IsLocal", DbType.Int32)]
        public int IsLocal { get; set; }

        [DataMapping("ExchangeRate", DbType.Decimal)]
        public decimal ExchangeRate { get; set; }

        [DataMapping("ListOrder", DbType.String)]
        public string ListOrder { get; set; }

        [DataMapping("Status", DbType.Int32)]
        public int Status { get; set; }

    }
}
