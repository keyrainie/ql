using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace ContentMgmt.GiftCardPoolInterface.Entities
{
    public class GCItemEntity 
    {
        [DataMapping("Code", DbType.AnsiStringFixedLength)]
        public string Code
        {
            get;
            set;
        }

         [DataMapping("BarPrefix", DbType.AnsiStringFixedLength)]
        public string BarPrefix
        {
            get;
            set;
        }

         [DataMapping("Amount", DbType.Decimal)]
        public decimal Amount
        {
            get;
            set;
        }
    }
}
