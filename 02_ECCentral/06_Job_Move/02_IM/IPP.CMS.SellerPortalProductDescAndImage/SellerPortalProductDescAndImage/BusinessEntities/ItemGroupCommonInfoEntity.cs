using Newegg.Oversea.Framework.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace IPP.ContentMgmt.SellerPortalProductDescAndImage.BusinessEntities
{
    public class ItemGroupCommonInfoEntity
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int? SysNo { get; set; }

        [DataMapping("CommonSKUNumber", DbType.String)]
        public string CommonSKUNumber { get; set; }

        [DataMapping("ProductGroupSysno", DbType.Int32)]
        public int? ProductGroupSysno { get; set; }

        [DataMapping("UPCCode", DbType.String)]
        public string UPCCode { get; set; }

        [DataMapping("ProductName", DbType.String)]
        public string ProductName { get; set; }

        [DataMapping("PromotionTitle", DbType.String)]
        public string PromotionTitle { get; set; }

        [DataMapping("Keywords", DbType.String)]
        public string Keywords { get; set; }

        [DataMapping("PMUserSysNo", DbType.Int32)]
        public int? PMUserSysNo { get; set; }

        [DataMapping("ProductModel", DbType.String)]
        public string ProductModel { get; set; }

        [DataMapping("ProductType", DbType.String)]
        public string ProductType { get; set; }

        [DataMapping("IsTakePictures", DbType.String)]
        public string IsTakePictures { get; set; }

        [DataMapping("PackageList", DbType.String)]
        public string PackageList { get; set; }

        [DataMapping("ProductLink", DbType.String)]
        public string ProductLink { get; set; }

        [DataMapping("Attention", DbType.String)]
        public string Attention { get; set; }

        [DataMapping("Note", DbType.String)]
        public string Note { get; set; }

        [DataMapping("CompanyBelongs", DbType.String)]
        public string CompanyBelongs { get; set; }

        [DataMapping("OnlyForRank", DbType.Int32)]
        public int? OnlyForRank { get; set; }

        [DataMapping("PicNumber", DbType.Int32)]
        public int? PicNumber { get; set; }

        [DataMapping("HostWarrantyDay", DbType.Int32)]
        public int? HostWarrantyDay { get; set; }

        [DataMapping("PartWarrantyDay", DbType.Int32)]
        public int? PartWarrantyDay { get; set; }

        [DataMapping("Warranty", DbType.String)]
        public string Warranty { get; set; }

        [DataMapping("ServicePhone", DbType.String)]
        public string ServicePhone { get; set; }

        [DataMapping("ServiceInfo", DbType.String)]
        public string ServiceInfo { get; set; }

        [DataMapping("IsOfferInvoice", DbType.String)]
        public string IsOfferInvoice { get; set; }

        [DataMapping("PartWarrantyDay", DbType.Int32)]
        public int? Weight { get; set; }

        [DataMapping("IsLarge", DbType.String)]
        public string IsLarge { get; set; }

        [DataMapping("Length", DbType.Decimal)]
        public decimal? Length { get; set; }

        [DataMapping("Width", DbType.Decimal)]
        public decimal? Width { get; set; }

        [DataMapping("Height", DbType.Decimal)]
        public decimal? Height { get; set; }

        [DataMapping("MinPackNumber", DbType.Int32)]
        public int? MinPackNumber { get; set; }

        [DataMapping("InDate", DbType.DateTime)]
        public DateTime? InDate { get; set; }

        [DataMapping("InUser", DbType.String)]
        public string InUser { get; set; }

        [DataMapping("EditDate", DbType.DateTime)]
        public DateTime? EditDate { get; set; }

        [DataMapping("EditUser", DbType.String)]
        public string EditUser { get; set; }

        [DataMapping("CompanyCode", DbType.String)]
        public string CompanyCode { get; set; }

        [DataMapping("StoreCompanyCode", DbType.String)]
        public string StoreCompanyCode { get; set; }

        [DataMapping("LanguageCode", DbType.String)]
        public string LanguageCode { get; set; }

        [DataMapping("BrandSysno", DbType.Int32)]
        public int? BrandSysno { get; set; }
        [DataMapping("C3SysNo", DbType.Int32)]
        public int? C3SysNo { get; set; }
        [DataMapping("ProductDesc", DbType.String)]
        public string ProductDesc { get; set; }
        [DataMapping("Is360Show", DbType.String)]
        public string Is360Show { get; set; }
        [DataMapping("IsVideo", DbType.String)]
        public string IsVideo { get; set; }

        [DataMapping("ProductDescLong", DbType.String)]
        public string ProductDescLong { get; set; }
        [DataMapping("ProductPhotoDesc", DbType.String)]
        public string ProductPhotoDesc { get; set; }

        [DataMapping("ProductID", DbType.String)]
        public string ProductID { get; set; }
        [DataMapping("ProductMode", DbType.String)]
        public string ProductMode { get; set; }
        [DataMapping("Performance", DbType.String)]
        public string Performance { get; set; }

        [DataMapping("ManufacturerSysNo", DbType.Int32)]
        public int? ManufacturerSysNo { get; set; }
        [DataMapping("MultiPicNum", DbType.Int32)]
        public int? MultiPicNum { get; set; }
        [DataMapping("PPMUserSysNo", DbType.Int32)]
        public int? PPMUserSysNo { get; set; }
        [DataMapping("CreateUserSysNo", DbType.Int32)]
        public int? CreateUserSysNo { get; set; }
        [DataMapping("CreateTime", DbType.DateTime)]
        public DateTime? CreateTime { get; set; }
        [DataMapping("BarCode", DbType.String)]
        public string BarCode { get; set; }
        [DataMapping("Status", DbType.Int32)]
        public int? Status { get; set; }
        [DataMapping("OrderByDateTime", DbType.DateTime)]
        public DateTime? OrderByDateTime { get; set; }
        [DataMapping("BriefName", DbType.String)]
        public string BriefName { get; set; }
        [DataMapping("RemarkCount", DbType.Int32)]
        public int? RemarkCount { get; set; }
        [DataMapping("RemarkScore", DbType.Int32)]
        public int? RemarkScore { get; set; }
        [DataMapping("IsConsign", DbType.Int32)]
        public int? IsConsign { get; set; }
        [DataMapping("OwnerList", DbType.String)]
        public string OwnerList { get; set; }
        [DataMapping("ProductTitle", DbType.String)]
        public string ProductTitle { get; set; }
        [DataMapping("ProductLine", DbType.String)]
        public string ProductLine { get; set; }
        [DataMapping("BriefNameAddition", DbType.String)]
        public string BriefNameAddition { get; set; }
        [DataMapping("IsHaveValidGift", DbType.Int32)]
        public int? IsHaveValidGift { get; set; }
        [DataMapping("CurrencySysNo", DbType.Int32)]
        public int? CurrencySysNo { get; set; }
        [DataMapping("VFItem", DbType.String)]
        public string VFItem { get; set; }
        [DataMapping("ProductCommonInfoSysno", DbType.Int32)]
        public int? ProductCommonInfoSysno { get; set; }
        [DataMapping("MerchantSysNo", DbType.Int32)]
        public int? MerchantSysNo { get; set; }

        [DataMapping("IsPermitRefund", DbType.Int32)]
        public int? IsPermitRefund { get; set; }

        [DataMapping("IsOfferValueAddedInvoice", DbType.Int32)]
        public int? IsOfferValueAddedInvoice { get; set; }

        [DataMapping("DMSValue", DbType.Decimal)]
        public decimal? DMSValue { get; set; }

        [DataMapping("SaleDays", DbType.Decimal)]
        public decimal? SaleDays { get; set; }

        [DataMapping("InventoryDays", DbType.Int32)]
        public int? InventoryDays { get; set; }

        [DataMapping("CompanyProduct", DbType.Int32)]
        public int? CompanyProduct { get; set; }

        [DataMapping("PreviousProductSysNo", DbType.Int32)]
        public int? PreviousProductSysNo { get; set; }

        [DataMapping("BuyOnlyForRank", DbType.Int32)]
        public int? BuyOnlyForRank { get; set; }

        [DataMapping("IsInstalmentProduct", DbType.Int32)]
        public int? IsInstalmentProduct { get; set; }

        [DataMapping("IsInstalmentProduct", DbType.Int32)]
        public int? InstalmentPhase { get; set; }

        [DataMapping("HoldMark", DbType.String)]
        public string HoldMark { get; set; }

        [DataMapping("HoldDate", DbType.DateTime)]
        public DateTime? HoldDate { get; set; }

        [DataMapping("HoldUser", DbType.Int32)]
        public int? HoldUser { get; set; }

        [DataMapping("HoldReason", DbType.String)]
        public string HoldReason { get; set; }

        [DataMapping("NoInventoryDays", DbType.Int32)]
        public int? NoInventoryDays { get; set; }

        [DataMapping("AccessoriesMemo", DbType.String)]
        public string AccessoriesMemo { get; set; }

        [DataMapping("ProductNotifyTimes", DbType.Int32)]
        public int? ProductNotifyTimes { get; set; }

        [DataMapping("ProductColor", DbType.String)]
        public string ProductColor { get; set; }

        [DataMapping("WeighupByWMS", DbType.Int32)]
        public int? WeighupByWMS { get; set; }

        [DataMapping("VirtualType", DbType.Int32)]
        public int? VirtualType { get; set; }

        [DataMapping("PromotionType", DbType.String)]
        public string PromotionType { get; set; }

        [DataMapping("AutoAdjustPrice", DbType.Int32)]
        public int? AutoAdjustPrice { get; set; }

        [DataMapping("UnshownAllowSearch", DbType.String)]
        public string UnshownAllowSearch { get; set; }

        [DataMapping("AutoPricingStartDate", DbType.DateTime)]
        public DateTime? AutoPricingStartDate { get; set; }

        [DataMapping("AutoPricingEndDate", DbType.DateTime)]
        public DateTime? AutoPricingEndDate { get; set; }

        [DataMapping("ImageVersion", DbType.String)]
        public string ImageVersion { get; set; }

        [DataMapping("IsAutoShowSecHand", DbType.String)]
        public string IsAutoShowSecHand { get; set; }

        [DataMapping("ValueDescription", DbType.String)]
        public string ValueDescription { get; set; }

        public string Tag { get; set; }

        [DataMapping("PropertySysNo1", DbType.Int32)]
        public int? PropertySysNo1 { get; set; }

        [DataMapping("PropertySysNo2", DbType.Int32)]
        public int? PropertySysNo2 { get; set; }

        [DataMapping("GroupSysNo1", DbType.Int32)]
        public int? GroupSysNo1 { get; set; }

        [DataMapping("GroupSysNo2", DbType.Int32)]
        public int? GroupSysNo2 { get; set; }

        [DataMapping("ValueSysNo1", DbType.Int32)]
        public int? ValueSysNo1 { get; set; }

        [DataMapping("ValueSysNo2", DbType.Int32)]
        public int? ValueSysNo2 { get; set; }

        [DataMapping("PropertyValue1", DbType.String)]
        public string PropertyValue1 { get; set; }

        [DataMapping("PropertyValue2", DbType.String)]
        public string PropertyValue2 { get; set; }

        [DataMapping("VirtualPrice", DbType.Decimal)]
        public decimal? VirtualPrice { get; set; }

        [DataMapping("Category3SysNo", DbType.Int32)]
        public int? Category3SysNo { get; set; }

        [DataMapping("IsChangeStyleWithTemplate", DbType.Int32)]
        public int? IsChangeStylewithTemplate { get; set; }

        [DataMapping("IsExtendWarrantyDisuse", DbType.Int32)]
        public int? IsExtendWarrantyDisuse { get; set; }

        [DataMapping("IsBasicInformationCorrect", DbType.Int32)]
        public int? IsBasicInformationCorrect { get; set; }

        [DataMapping("IsAccessoriesShow", DbType.Int32)]
        public int? IsAccessoriesShow { get; set; }

        [DataMapping("IsAccessoriesCorrect", DbType.Int32)]
        public int? IsAccessoriesCorrect { get; set; }

        [DataMapping("IsVirtualPic", DbType.Int32)]
        public int? IsVirtualPic { get; set; }

        [DataMapping("IsPictureCorrect", DbType.Int32)]
        public int? IsPictureCorrect { get; set; }

        [DataMapping("IsWarrantyCorrect", DbType.Int32)]
        public int? IsWarrantyCorrect { get; set; }

        [DataMapping("IsWarrantyShow", DbType.Int32)]
        public int? IsWarrantyShow { get; set; }

        [DataMapping("IsWeightCorrect", DbType.Int32)]
        public int? IsWeightCorrect { get; set; }

        [DataMapping("InfoStatus", DbType.Int32)]
        public int? InfoStatus { get; set; }

        [DataMapping("InfoShowStatus", DbType.Int32)]
        public int? InfoShowStatus { get; set; }

        [DataMapping("GroupProductName", DbType.String)]
        public string GroupProductName { get; set; }

        [DataMapping("ProductInfoFinishStatus", DbType.String)]
        public String ProductInfoFinishStatus { get; set; }

        [DataMapping("PO_Memo", DbType.String)]
        public string PO_Memo { get; set; }

        public string Type { get; set; }

        [DataMapping("VendorName", DbType.String)]
        public string VendorName { get; set; }

        [DataMapping("VendorSysNo", DbType.String)]
        public int VendorSysNo { get; set; }

        /// <summary>
        /// 常规促销语
        /// </summary>
        [DataMapping("NormalPromotionTitle", DbType.String)]
        public string NormalPromotionTitle { get; set; }
        /// <summary>
        /// 时效促销语
        /// </summary>
        [DataMapping("CountDownPromotionTitle", DbType.String)]
        public string CountDownPromotionTitle { get; set; }
        /// <summary>
        /// 促销语开始时间
        /// </summary>
        [DataMapping("PromotionTitleBeginDate", DbType.String)]
        public string PromotionTitleBeginDate { get; set; }
        /// <summary>
        /// 促销语结束时间
        /// </summary>
        [DataMapping("PromotionTitleEndDate", DbType.String)]
        public string PromotionTitleEndDate { get; set; }

        [DataMapping("MaterialNumber", DbType.String)]
        public string MaterialNumber { get; set; }

        public ItemGroupCommonInfoEntity Clone()
        {
            //MemoryStream memoryStream = new MemoryStream();

            //BinaryFormatter formatter = new BinaryFormatter();

            //formatter.Serialize(memoryStream, this);

            //memoryStream.Position = 0;

            //return (ItemGroupCommonInfoEntity)formatter.Deserialize(memoryStream);
            return (ItemGroupCommonInfoEntity)this.MemberwiseClone();
        }
    }
}
