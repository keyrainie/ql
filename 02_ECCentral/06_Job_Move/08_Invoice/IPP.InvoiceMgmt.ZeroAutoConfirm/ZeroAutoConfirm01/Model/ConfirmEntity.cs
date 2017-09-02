using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace ZeroAutoConfirm.Model
{
    public class ConfirmEntity
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("SoSysNo", DbType.Int32)]
        public int SoSysNo { get; set; }

        [DataMapping("PayTypeSysNo", DbType.Int32)]
        public int PayTypeSysNo { get; set; }

        [DataMapping("OrderAmt", DbType.Decimal)]
        public decimal OrderAmt { get; set; }

        [DataMapping("IncomeAmt", DbType.Decimal)]
        public decimal IncomeAmt { get; set; }

        [DataMapping("PrepayAmt", DbType.Decimal)]
        public decimal PrepayAmt { get; set; }

        [DataMapping("PointPayAmt", DbType.Decimal)]
        public decimal PointPayAmt { get; set; }

        [DataMapping("GiftCardPayAmt", DbType.Decimal)]
        public decimal GiftCardPayAmt { get; set; }

        [DataMapping("PayTerms", DbType.String)]
        public string PayTerms { get; set; }

        [DataMapping("PayedAmt", DbType.Decimal)]
        public decimal PayedAmt { get; set; }

        [DataMapping("ConfirmedInfo", DbType.String)]
        public string ConfirmedInfo { get; set; }

        [DataMapping("ConfirmedDate", DbType.DateTime)]
        public DateTime ConfirmedDate { get; set; }

        [DataMapping("CofirmedID", DbType.String)]
        public String CofirmedID { get; set; }
    }
}
