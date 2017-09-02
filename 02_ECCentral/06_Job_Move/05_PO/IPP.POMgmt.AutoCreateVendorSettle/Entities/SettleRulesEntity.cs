using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace POASNMgmt.AutoCreateVendorSettle.Entities
{
    //CRL20438 By Kilin
    //代销结算规则
    [Serializable]
    internal class SettleRulesEntity
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("SettleRulesCode", DbType.String)]
        public string SettleRulesCode { get; set; }

        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }

        [DataMapping("VendorSysNo", DbType.Int32)]
        public int VendorSysNo { get; set; }

        [DataMapping("NewSettlePrice", DbType.Decimal)]
        public decimal NewSettlePrice { get; set; }

        [DataMapping("SettleRuleQuantity", DbType.Int32)]
        public int? SettleRuleQuantity { get; set; }

        [DataMapping("SettleedQuantity", DbType.Int32)]
        public int? SettleedQuantity { get; set; }

    }
}
