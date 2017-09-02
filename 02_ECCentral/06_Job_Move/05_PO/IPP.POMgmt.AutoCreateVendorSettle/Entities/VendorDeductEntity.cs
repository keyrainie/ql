using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.PO;
using Newegg.Oversea.Framework.Entity;

namespace POASNMgmt.AutoCreateVendorSettle.Entities
{
    class VendorDeductEntity
    {
         [DataMapping("CalcType", DbType.Int32)]
         public VendorCalcType CalcType { get; set; }

         [DataMapping("DeductPercent", DbType.Int32)]
         public decimal DeductPercent { get; set; }
         [DataMapping("FixAmt", DbType.Int32)]
         public decimal FixAmt { get; set; }
         [DataMapping("MaxAmt", DbType.Int32)]
         public decimal MaxAmt { get; set; }

         //下面二个显示用
         [DataMapping("AccountType", DbType.Int32)]
         public AccountType AccountType { get; set; }
         [DataMapping("DeductMethod", DbType.Int32)]
         public DeductMethod DeductMethod { get; set; }

    }
}
