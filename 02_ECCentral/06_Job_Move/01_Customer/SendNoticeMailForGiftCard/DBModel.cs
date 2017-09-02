using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace SendNoticeMailForGiftCard
{
    public class EmailList 
    {
        [DataMapping("TransactionNumber", DbType.Int32)]
        public int TransactionNumber { get; set; }
        [DataMapping("Code", DbType.String)]
        public string Code { get; set; }
        [DataMapping("AvailAmount", DbType.Decimal)]
        public decimal AvailAmount { get; set; }
        [DataMapping("CustomerSysNo", DbType.Int32)]
        public int? CustomerSysNo { get; set; }
        [DataMapping("BindingCustomerSysNo", DbType.Int32)]
        public int? BindingCustomerSysNo { get; set; }
        [DataMapping("EndDate", DbType.DateTime)]
        public DateTime? EndDate { get; set; }
        [DataMapping("email", DbType.String)]
        public string email { get; set; }
        [DataMapping("customerid", DbType.String)]
        public string customerid { get; set; }
    }
}
