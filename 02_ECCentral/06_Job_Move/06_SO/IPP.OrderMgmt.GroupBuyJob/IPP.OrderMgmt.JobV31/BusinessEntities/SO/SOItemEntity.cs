using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.OrderMgmt.JobV31.BusinessEntities
{
    public class SOItemEntity : EntityBase
    {
        [DataMapping("IsMemberPrice", DbType.Int32)]
        public int? IsMemberPrice
        {
            get;
            set;
        }

        [DataMapping("BriefName", DbType.String)]
        public string BriefName
        {
            get;
            set;
        }

        [DataMapping("C1SysNo", DbType.Int32)]
        public int C1SysNo
        {
            get;
            set;
        }

        [DataMapping("C2SysNo", DbType.Int32)]
        public int C2SysNo
        {
            get;
            set;
        }

        [DataMapping("C3SysNo", DbType.Int32)]
        public int C3SysNo
        {
            get;
            set;
        }

        [DataMapping("CompanyProduct", DbType.Int32)]
        public int CompanyProduct
        {
            get;
            set;
        }

        [DataMapping("Cost", DbType.Decimal)]
        public decimal Cost
        {
            get;
            set;
        }

        [DataMapping("DiscountAmt", DbType.Decimal)]
        public decimal DiscountAmt
        {
            get;
            set;
        }

        [DataMapping("GiftSysNo", DbType.Int32)]
        public int GiftSysNo
        {
            get;
            set;
        }

        [DataMapping("ManufacturerSysNo", DbType.Int32)]
        public int ManufacturerSysNo
        {
            get;
            set;
        }

        [DataMapping("MasterProductSysNo", DbType.String)]
        public string MasterProductSysNo
        {
            get;
            set;
        }

        [DataMapping("OriginalPrice", DbType.Decimal)]
        public decimal OriginalPrice
        {
            get;
            set;
        }

        [DataMapping("Point", DbType.Int32)]
        public int Point
        {
            get;
            set;
        }

        [DataMapping("PointType", DbType.Int32)]
        public int? PointType
        {
            get;
            set;
        }

        [DataMapping("PreviousProductSysNo", DbType.Int32)]
        public int PreviousProductSysNo
        {
            get;
            set;
        }

        [DataMapping("Price", DbType.Decimal)]
        public decimal Price
        {
            get;
            set;
        }

        [DataMapping("ProductID", DbType.String)]
        public string ProductID
        {
            get;
            set;
        }

        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo
        {
            get;
            set;
        }

        [DataMapping("ProductType", DbType.Int32)]
        public int? ProductType
        {
            get;
            set;
        }

        [DataMapping("PromotionDiscount", DbType.Decimal)]
        public decimal PromotionDiscount
        {
            get;
            set;
        }

        [DataMapping("Quantity", DbType.Int32)]
        public int Quantity
        {
            get;
            set;
        }

        [DataMapping("SOSysNo", DbType.Int32)]
        public int SOSysNo
        {
            get;
            set;
        }

        [DataMapping("UnitCostWithoutTax", DbType.Decimal)]
        public decimal UnitCostWithoutTax
        {
            get;
            set;
        }

        [DataMapping("WarehouseNumber", DbType.String)]
        public string WarehouseNumber
        {
            get;
            set;
        }

        [DataMapping("WarehouseName", DbType.String)]
        public string WarehouseName
        {
            get;
            set;
        }

        [DataMapping("Warranty", DbType.String)]
        public string Warranty
        {
            get;
            set;
        }

        [DataMapping("Weight", DbType.Int32)]
        public int Weight
        {
            get;
            set;
        }

        [DataMapping("OnlineQty", DbType.Int32)]
        public int OnlineQty
        {
            get;
            set;
        }

        [DataMapping("DuplicateSOSysNo", DbType.String)]
        public string DuplicateSOSysNo
        {
            get;
            set;
        }

        [DataMapping("Discount", DbType.Decimal)]
        public decimal Discount
        {
            get;
            set;
        }

        [DataMapping("Reason", DbType.String)]
        public string Reason
        {
            get;
            set;
        }

        [DataMapping("Length", DbType.Decimal)]
        public decimal Length
        {
            get;
            set;
        }

        [DataMapping("Width", DbType.Decimal)]
        public decimal Width
        {
            get;
            set;
        }

        [DataMapping("Height", DbType.Decimal)]
        public decimal Height
        {
            get;
            set;
        }


        [DataMapping("DiscountType", DbType.Int32)]
        public int DiscountType
        {
            get;
            set;
        }

        public string OriginalPriceString
        {
            get { return OriginalPrice.ToString(AppConst.DecimalFormat); }
            set { }
        }

        public string DiscountAmtString
        {
            get { return DiscountAmt.ToString(AppConst.DecimalFormat); }
            set { }
        }

        public string PromotionDiscountString
        {
            get { return PromotionDiscount.ToString(AppConst.DecimalFormat); }
            set { }
        }

        public decimal CurrentPrice
        {
            get;
            set;
        }


    }
}
