using System;
using System.Data;
using Newegg.Oversea.Framework.Entity;

namespace AutoSetResponsibleUser.Entities
{
    internal sealed class TrackingInfoEntity
    {
        [DataMapping("OrderSysNo", DbType.Int32)]
        public int OrderSysNo { get; set; }

        [DataMapping("IncomeAmt", DbType.Decimal)]
        public Decimal IncomeAmt { get; set; }
    }
}
