using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;
using System.ComponentModel;

namespace IPP.OrderMgmt.JobV31.BusinessEntities
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

        [DataMapping("IsExpiateOrder", DbType.Int32)]
        public int? IsExpiateOrder
        {
            get;
            set;
        }

        [DataMapping("IsDCOrder", DbType.Int32)]
        public int? IsDCOrder
        {
            get;
            set;
        }

        [DataMapping("FPExtend", DbType.String)]
        public string FPExtend
        {
            get;
            set;
        }

        [DataMapping("SOType", DbType.Int32)]
        public int? SOType
        {
            get;
            set;
        }

        [DataMapping("StockStatus", DbType.Int32)]
        public int StockStatus
        {
            get;
            set;
        }

        [DataMapping("IsMergeComplete", DbType.Int32)]
        public int? IsMergeComplete
        {
            get;
            set;
        }

        [DataMapping("RingOutShipType", DbType.AnsiStringFixedLength)]
        public string RingOutShipType
        {
            get;
            set;
        }

        [DataMapping("DeliveryPromise", DbType.AnsiStringFixedLength)]
        public string DeliveryPromise
        {
            get;
            set;
        }

        [DataMapping("HoldStatus", DbType.Int32)]
        public WebHoldType? HoldStatus
        {
            get;
            set;
        }

        [DataMapping("HoldDate", DbType.DateTime)]
        public DateTime HoldDate
        {
            get;
            set;
        }

        [DataMapping("SoSplitType", DbType.AnsiStringFixedLength)]
        public string SoSplitType
        {
            get;
            set;
        }

        [DataMapping("SoSplitMaster", DbType.Int32)]
        public int? SoSplitMaster
        {
            get;
            set;
        }

        [DataMapping("DeliverySection", DbType.String)]
        public string DeliveryTimeRange
        {
            get;
            set;
        }

        [DataMapping("DeliveryType", DbType.String)]
        public string DeliveryType
        {
            get;
            set;
        }

        [DataMapping("MerchantSysNo", DbType.Int32)]
        public int? MerchantSysNo
        {
            get;
            set;
        }


        [DataMapping("StockType", DbType.AnsiStringFixedLength)]
        public string StockTypeStr
        {
            get;
            set;
        }


        [DataMapping("ShippingType", DbType.AnsiStringFixedLength)]
        public string ShippingTypeStr
        {
            get;
            set;
        }

        [DataMapping("InvoiceType", DbType.AnsiStringFixedLength)]
        public string InvoiceTypeStr
        {
            get;
            set;
        }

        [DataMapping("VendorName", DbType.String)]
        public string VendorName
        {
            get;
            set;
        }


        [DataMapping("DestWarehouseNumber", DbType.String)]
        public string DestWarehouseNumber
        {
            get;
            set;
        }

        [DataMapping("DestWarehouseName", DbType.String)]
        public string DestWarehouseName
        {
            get;
            set;
        }

        [DataMapping("ReferenceSysno", DbType.Int32)]
        public int ReferenceSysno
        {
            get;
            set;
        }

        public SOStockType StockType
        {
            get
            {
                switch (StockTypeStr)
                {
                    case "MET":
                        return SOStockType.MET;
                    default:
                        return SOStockType.NEG;
                }
            }
        }

        public SOShippingType ShippingType
        {
            get
            {
                switch (ShippingTypeStr)
                {
                    case "MET":
                        return SOShippingType.MET;
                    default:
                        return SOShippingType.NEG;
                }
            }
        }

        public SOInvoiceType InvoiceType
        {
            get
            {
                switch (InvoiceTypeStr)
                {
                    case "MET":
                        return SOInvoiceType.MET;
                    default:
                        return SOInvoiceType.NEG;
                }
            }
        }

    }


    public enum SOStockType
    {
        [Description("新蛋仓储")]
        NEG,
        [Description("商家仓储")]
        MET

    }
    public enum SOShippingType
    {
        [Description("新蛋配送")]
        NEG,
        [Description("商家配送")]
        MET

    }
    public enum SOInvoiceType
    {
        [Description("新蛋开票")]
        NEG,
        [Description("商家开票")]
        MET

    }

    public enum WebHoldType : int
    {
        [Description("Normal")]
        Normal = 0,
        [Description("IPPHold")]
        IPPHold = 1,
        [Description("WebHold")]
        WebHold = 2,
        [Description("Processing")]
        Processing = 3,
        [Description("CancelNoOrder")]
        CancelNoOrder = 4,
        [Description("Cancel")]
        Cancel = 5,
        [Description("AutoUnHold")]
        AutoUnHold = -1,
    }
}
