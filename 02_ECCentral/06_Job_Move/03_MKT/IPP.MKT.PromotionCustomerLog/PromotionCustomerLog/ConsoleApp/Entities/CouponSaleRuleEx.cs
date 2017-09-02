using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;

namespace IPP.EcommerceMgmt.SendCouponCode.Entities
{
    [Serializable]
    public class CouponSaleRuleEx : DetailBaseEntity
    {

        [DataMapping("CouponSysNo", DbType.Int32)]
        public int CouponSysNo { get; set; }

        [DataMapping("OrderAmountLimit", DbType.Decimal)]
        public decimal OrderAmountLimit { get; set; }

        [DataMapping("PayTypeSysNo", DbType.Int32)]
        public int PayTypeSysNo { get; set; }

        [DataMapping("ShippingTypeSysNo", DbType.Int32)]
        public int ShippingTypeSysNo { get; set; }

        [DataMapping("OrderMaxDiscount", DbType.Int32)]
        public int OrderMaxDiscount { get; set; }

        [DataMapping("CustomerMaxFrequency", DbType.Int32)]
        public int CustomerMaxFrequency { get; set; }

        [DataMapping("MaxFrequency", DbType.Int32)]
        public int MaxFrequency { get; set; }

        [DataMapping("UsedFrequency", DbType.Int32)]
        public int UsedFrequency { get; set; }

        [DataMapping("NeedEmailVerification", DbType.String)]
        public string NeedEmailVerification { get; set; }

        [DataMapping("NeedMobileVerification", DbType.String)]
        public string NeedMobileVerification { get; set; }

        [DataMapping("InvalidForAmbassador", DbType.String)]
        public string InvalidForAmbassador { get; set; }

        [DataMapping("IsAutoUse", DbType.String)]
        public string IsAutoUse { get; set; }



    }
}
