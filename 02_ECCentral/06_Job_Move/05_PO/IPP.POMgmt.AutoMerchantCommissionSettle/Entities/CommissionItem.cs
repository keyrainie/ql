using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Newegg.Oversea.Framework.Entity;

namespace MerchantCommissionSettle.Entities
{
    public sealed class CommissionItem
    {
        [DataMapping("SysNo",DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("CommissionMasterSysNo", DbType.Int32)]
        public int CommissionMasterSysNo { get; set; }

        [DataMapping("VendorManufacturerSysNo", DbType.Int32)]
        public int VendorManufacturerSysNo { get; set; }

        [DataMapping("RuleSysNo", DbType.Int32)]
        public int RuleSysNo { get; set; }
        
        [DataMapping("RentFee",DbType.Decimal)]
        public decimal Rent { get; set; }

        [DataMapping("DeliveryFee", DbType.Decimal)]
        public decimal DeliveryFee { get; set; }

        [DataMapping("SalesCommissionFee", DbType.Decimal)]
        public decimal SalesCommissionFee { get; set; }

        [DataMapping("OrderCommissionFee", DbType.Decimal)]
        public decimal OrderCommissionFee { get; set; }

        [DataMapping("TotalSaleAmt", DbType.Decimal)]
        public decimal TotalSaleAmt { get; set; }

        [DataMapping("InUser", DbType.String)]
        public string InUser { get; set; }

        [DataMapping("InDate", DbType.DateTime)]
        public DateTime InDate { get; set; }

        [DataMapping("EditUser", DbType.String)]
        public string EditUser { get; set; }

        [DataMapping("EditDate", DbType.DateTime)]
        public DateTime EditDate { get; set; }

        [DataMapping("CurrencyCode", DbType.String)]
        public string CurrencyCode { get; set; }

        [DataMapping("CompanyCode", DbType.String)]
        public string CompanyCode { get; set; }

        [DataMapping("StoreCompanyCode", DbType.String)]
        public string StoreCompanyCode { get; set; }

        [DataMapping("LanguageCode", DbType.String)]
        public string LanguageCode { get; set; }

    }
}
