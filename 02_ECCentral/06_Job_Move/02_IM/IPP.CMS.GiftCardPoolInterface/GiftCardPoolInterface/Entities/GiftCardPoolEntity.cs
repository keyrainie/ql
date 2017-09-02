using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;

namespace ContentMgmt.GiftCardPoolInterface.Entities
{
    [Serializable]
    public class GiftCardPoolEntity : DetailBaseEntity
    {
        public GiftCardPoolEntity()
        {
            Code = string.Empty;
            Password = string.Empty;
            Barcode = string.Empty;
            Status = string.Empty;
        }

        [DataMapping("Code", DbType.AnsiStringFixedLength)]
        public string Code { get; set; }

        [DataMapping("Password", DbType.AnsiStringFixedLength)]
        public string Password { get; set; }

        [DataMapping("Barcode", DbType.AnsiStringFixedLength)]
        public string Barcode { get; set; }

        [DataMapping("AmountType", DbType.Int32)]
        public int AmountType { get; set; }
    }
}
