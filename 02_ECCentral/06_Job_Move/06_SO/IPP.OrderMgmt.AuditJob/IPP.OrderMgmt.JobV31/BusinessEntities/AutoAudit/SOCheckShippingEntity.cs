using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.OrderMgmt.JobV31.BusinessEntities.AutoAudit
{
    public class SOCheckShippingEntity : EntityBase
    {
        [DataMapping("SOSysNo", DbType.Int32)]
        public int? SOSysNo
        {
            get;
            set;
        }

        [DataMapping("WeightSO", DbType.Int32)]
        public int? WeightSO
        {
            get;
            set;
        }

        [DataMapping("CreateTime", DbType.DateTime)]
        public DateTime? CreateTime
        {
            get;
            set;
        }

        [DataMapping("ShippingFee", DbType.Decimal)]
        public decimal? ShippingFee
        {
            get;
            set;
        }

        [DataMapping("PackageFee", DbType.Decimal)]
        public decimal? PackageFee
        {
            get;
            set;
        }

        [DataMapping("RegisteredFee", DbType.Decimal)]
        public decimal? RegisteredFee
        {
            get;
            set;
        }

        [DataMapping("UpdateTime", DbType.DateTime)]
        public DateTime? UpdateTime
        {
            get;
            set;
        }

        [DataMapping("Weight3PL", DbType.Int32)]
        public int? Weight3PL
        {
            get;
            set;
        }

        [DataMapping("ShipCost", DbType.Decimal)]
        public decimal? ShipCost
        {
            get;
            set;
        }

        [DataMapping("CustomerIPAddress", DbType.String)]
        public string CustomerIPAddress
        {
            get;
            set;
        }

        [DataMapping("CustomerCookie", DbType.String)]
        public string CustomerCookie
        {
            get;
            set;
        }

        [DataMapping("ShipCost3PL", DbType.Decimal)]
        public decimal? ShipCost3PL
        {
            get;
            set;
        }

        [DataMapping("SpecialSOType", DbType.Int32)]
        public int? SpecialSOType
        {
            get;
            set;
        }

        [DataMapping("IsPhoneOrder", DbType.Int32)]
        public int? IsPhoneOrder
        {
            get;
            set;
        }

        [DataMapping("SplitUserSysNo", DbType.Int32)]
        public int? SplitUserSysNo
        {
            get;
            set;
        }

        [DataMapping("SplitDateTime", DbType.DateTime)]
        public DateTime SplitDateTime
        {
            get;
            set;
        }

        [DataMapping("IsRequireShipInvoice", DbType.Int32)]
        public int? IsRequireShipInvoice
        {
            get;
            set;
        }

        [DataMapping("IsVATPrinted", DbType.Int32)]
        public int? IsVATPrinted
        {
            get;
            set;
        }

        [DataMapping("IsFPSO", DbType.Int32)]
        public int? IsFPSO
        {
            get;
            set;
        }

        [DataMapping("FPReason", DbType.String)]
        public string FPReason
        {
            get;
            set;
        }

        [DataMapping("IsFPCheck", DbType.Int32)]
        public int? IsFPCheck
        {
            get;
            set;
        }

        [DataMapping("FPCheckTime", DbType.DateTime)]
        public DateTime? FPCheckTime
        {
            get;
            set;
        }

        [DataMapping("IsDuplicateOrder", DbType.Int32)]
        public int? IsDuplicateOrder
        {
            get;
            set;
        }

        [DataMapping("IsDirectAlipay", DbType.Int32)]
        public int? IsDirectAlipay
        {
            get;
            set;
        }

        [DataMapping("MemoForCustomer", DbType.String)]
        public string MemoForCustomer
        {
            get;
            set;
        }

        [DataMapping("OriginShipPrice", DbType.Int32)]
        public int? VPOStatus
        {
            get;
            set;
        }

        [DataMapping("OriginShipPrice", DbType.Decimal)]
        public decimal? OriginShipPrice
        {
            get;
            set;
        }

        [DataMapping("IsMultiInvoice", DbType.Int32)]
        public int? IsMultiInvoice
        {
            get;
            set;
        }

        [DataMapping("TenpayCoupon", DbType.Decimal)]
        public decimal? TenpayCoupon
        {
            get;
            set;
        }

        [DataMapping("MKTActivityType", DbType.Int32)]
        public int? MKTActivityType
        {
            get;
            set;
        }

        [DataMapping("IsCombine", DbType.Int32)]
        public int? IsCombine
        {
            get;
            set;
        }

        [DataMapping("IsExtendWarrantyOrder", DbType.Int32)]
        public int? IsExtendWarrantyOrder
        {
            get;
            set;
        }

        [DataMapping("IsBackOrder", DbType.Int32)]
        public int? IsBackOrder
        {
            get;
            set;
        }
    }
}
