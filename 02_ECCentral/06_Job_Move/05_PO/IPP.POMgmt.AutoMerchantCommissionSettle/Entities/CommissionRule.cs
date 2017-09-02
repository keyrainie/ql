using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Newegg.Oversea.Framework.Entity;

namespace MerchantCommissionSettle.Entities
{
    public sealed class CommissionRule
    {
        [DataMapping("SysNo",DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("VendorManufacturerSysNo", DbType.Int32)]
        public int VendorManufacturerSysNo { get; set; }

        [DataMapping("OrderCommissionFee", DbType.Decimal)]
        public decimal OrderCommissionFee { get; set; }

        [DataMapping("SalesRule", DbType.String)]
        public string SalesRule { get; set; }

        [DataMapping("DeliveryFee", DbType.Decimal)]
        public decimal DeliveryFee { get; set; }

        [DataMapping("RentFee", DbType.Decimal)]
        public decimal RentFee { get; set; }

        [DataMapping("Status", DbType.String)]
        public string Status { get; set; }

        [DataMapping("VendorSysNo", DbType.Int32)]
        public int VendorSysNo { get; set; }

        [DataMapping("ManufacturerSysNo", DbType.Int32)]
        public int ManufacturerSysNo { get; set; }

        [DataMapping("BrandSysNo", DbType.Int32)]
        public int BrandSysNo { get; set; }

        [DataMapping("C3SysNo", DbType.Int32)]
        public int C3SysNo { get; set; }
    }
}
