using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Newegg.Oversea.Framework.Entity;

namespace MerchantCommissionSettle.Entities
{
    public sealed class CommissionLog
    {
        [DataMapping("SysNo",DbType.Int32)]
        public int? SysNo { get; set; }

        [DataMapping("CommissionItemSysNo", DbType.Int32)]
        public int CommissionItemSysNo { get; set; }

        [DataMapping("MerchantSysNo",DbType.Int32)]
        public int MerchantSysNo { get; set; }

        [DataMapping("Type", DbType.String)]
        public string Type { get; set; }

        [DataMapping("VendorManufacturerSysNo", DbType.Int32)]
        public int VendorManufacturerSysNo { get; set; }

        [DataMapping("ReferenceSysNo",DbType.Int32)]
        public int ReferenceSysNo { get; set; }

        [DataMapping("ReferenceType",DbType.String)]
        public string ReferenceType { get; set; }

        [DataMapping("ProductSysNo",DbType.Int32)]
        public int ProductSysNo { get; set; }

        [DataMapping("PromotionDiscount",DbType.Decimal)]
        public decimal? PromotionDiscount { get; set; }


        [DataMapping("Price",DbType.Decimal)]
        public Decimal Price { get; set; }

        [DataMapping("Qty",DbType.Int32)]
        public int Qty { get; set; }

        [DataMapping("C2SysNo", DbType.Int32)]
        public int C2SysNo { get; set; }

        [DataMapping("C3SysNo", DbType.Int32)]
        public int C3SysNo { get; set; }

        [DataMapping("InUser",DbType.String)]
        public string InUser { get; set; }

        [DataMapping("InDate",DbType.DateTime)]
        public DateTime? InDate { get; set; }

        [DataMapping("EditUser", DbType.String)]
        public string EditUser { get; set; }

        [DataMapping("EditDate", DbType.DateTime)]
        public DateTime? EditDate { get; set; }

        [DataMapping("CurrencyCode",DbType.String)]
        public string CurrencyCode { get; set; }

        [DataMapping("CompanyCode",DbType.String)]
        public string CompanyCode { get; set; }

        [DataMapping("StoreCompanyCode", DbType.String)]
        public string StoreCompanyCode { get; set; }

        [DataMapping("LanguageCode", DbType.String)]
        public string LanguageCode { get; set; }

        [DataMapping("Point", DbType.Int32)]
        public int Point { get; set; }

        [DataMapping("DiscountAmt", DbType.Decimal)]
        public decimal DiscountAmt { get; set; }

        public decimal CommissionAmt { get; set; }

        [DataMapping("HaveAutoRMA", DbType.Int32)]
        public int HaveAutoRMA { get; set; }

        [DataMapping("SoSysNo", DbType.Int32)]
        public int SoSysNo { get; set; }
    }
}
