using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPPOversea.POmgmt.ETA.Model
{
    public class FinancePayItemEntity
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }


        [DataMapping("PaySysNo", DbType.Int32)]
        public int PaySysNo { get; set; }

        [DataMapping("PayStyle", DbType.Int32)]
        public int PayStyle { get; set; }
        
        /// <summary>
        /// 可用金额
        /// </summary>
        [DataMapping("AvailableAmt", DbType.Decimal)]
        public decimal AvailableAmt { get; set; }

        [DataMapping("OrderSysNo", DbType.Int32)]
        public int OrderSysNo { get; set; }
        
        [DataMapping("Status", DbType.Int32)]
        public int Status { get; set; }


        [DataMapping("LanguageCode", DbType.StringFixedLength)]
        public string LanguageCode { get; set; }

        [DataMapping("StoreCompanyCode", DbType.String)]
        public string StoreCompanyCode { get; set; }

        [DataMapping("CompanyCode", DbType.String)]
        public string CompanyCode { get; set; }
    }
}
