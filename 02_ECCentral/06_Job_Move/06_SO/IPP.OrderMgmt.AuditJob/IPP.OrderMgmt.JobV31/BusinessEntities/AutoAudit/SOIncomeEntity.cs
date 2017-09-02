using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.OrderMgmt.JobV31.BusinessEntities.AutoAudit
{
    public class SOIncomeEntity
    {
        [DataMapping("ConfirmTime", DbType.DateTime)]
        public DateTime ConfirmTime
        {
            get;
            set;
        }

        [DataMapping("ConfirmUserSysNo", DbType.Int32)]
        public int ConfirmUserSysNo
        {
            get;
            set;
        }

        [DataMapping("IncomeAmt", DbType.Decimal)]
        public decimal IncomeAmt
        {
            get;
            set;
        }

        [DataMapping("IncomeStyle", DbType.Int32)]
        public int IncomeStyle
        {
            get;
            set;
        }

        [DataMapping("IncomeTime", DbType.DateTime)]
        public DateTime IncomeTime
        {
            get;
            set;
        }

        [DataMapping("IncomeUserSysNo", DbType.Int32)]
        public int IncomeUserSysNo
        {
            get;
            set;
        }

        [DataMapping("Note", DbType.String)]
        public string Note
        {
            get;
            set;
        }

        [DataMapping("OrderAmt", DbType.Decimal)]
        public decimal OrderAmt
        {
            get;
            set;
        }

        [DataMapping("OrderSysNo", DbType.Int32)]
        public int OrderSysNo
        {
            get;
            set;
        }

        [DataMapping("OrderType", DbType.Int32)]
        public int OrderType
        {
            get;
            set;
        }

        [DataMapping("PrepayAmt", DbType.Decimal)]
        public decimal PrepayAmt
        {
            get;
            set;
        }

        [DataMapping("GiftCardPayAmt", DbType.Decimal)]
        public decimal GiftCardPayAmt
        {
            get;
            set;
        }
        
        [DataMapping("ReferenceID", DbType.String)]
        public string ReferenceID
        {
            get;
            set;
        }

        [DataMapping("Status", DbType.Int32)]
        public int Status
        {
            get;
            set;
        }

        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo
        {
            get;
            set;
        }

    }
}
