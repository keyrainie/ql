using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.ContentMgmt.SellerPortalProductDescAndImage.BusinessEntities
{
    [Serializable]
    public class ItemVendorProductRequestEntity
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("RequestSysno", DbType.Int32)]
        public int RequestSysno { get; set; }

        [DataMapping("SellerSysno", DbType.Int32)]
        public int SellerSysNo { get; set; }

        [DataMapping("GroupSysno", DbType.Int32)]
        public int GroupSysno { get; set; }

        [DataMapping("ProductSysno", DbType.Int32)]
        public int ProductSysno { get; set; }

        [DataMapping("ProductInfoProductName", DbType.String)]
        public string ProductInfoProductName
        {
            get;
            set;
        }

        [DataMapping("CommonSKUNumber", DbType.String)]
        public string CommonSKUNumber
        {
            get;
            set;
        }

        [DataMapping("ProductID", DbType.String)]
        public string ProductID { get; set; }

        [DataMapping("ProductName", DbType.String)]
        public string ProductName { get; set; }

        [DataMapping("ProductModel", DbType.String)]
        public string ProductMode { get; set; }

        [DataMapping("BriefName", DbType.String)]
        public string BriefName { get; set; }

        [DataMapping("PromotionTitle", DbType.String)]
        public string PromotionTitle { get; set; }

        [DataMapping("Keywords", DbType.String)]
        public string Keywords { get; set; }

        [DataMapping("UPCCode", DbType.String)]
        public string UPCCode { get; set; }

        [DataMapping("ProductLink", DbType.String)]
        public string ProductLink { get; set; }

        [DataMapping("ProductInfoProductLink", DbType.String)]
        public string ProductInfoProductLink { get; set; }

        [DataMapping("PackageList", DbType.String)]
        public string PackageList { get; set; }

        [DataMapping("MinPackNumber", DbType.Int32)]
        public int? MinPackNumber { get; set; }

        [DataMapping("HostWarrantyDay", DbType.Int32)]
        public int? HostWarrantyDay { get; set; }

        [DataMapping("PartWarrantyDay", DbType.Int32)]
        public int? PartWarrantyDay { get; set; }

        [DataMapping("Warranty", DbType.String)]
        public string Warranty { set; get; }

        [DataMapping("ServicePhone", DbType.String)]
        public string ServicePhone { set; get; }

        [DataMapping("ServiceInfo", DbType.String)]
        public string ServiceInfo { set; get; }

        [DataMapping("Note", DbType.String)]
        public string Note { get; set; }

        [DataMapping("Weight", DbType.Int32)]
        public int? Weight { get; set; }

        [DataMapping("IsLarge", DbType.String)]
        public string IsLarge { get; set; }

        [DataMapping("Length", DbType.Decimal)]
        public decimal? Length { get; set; }

        [DataMapping("Width", DbType.Decimal)]
        public decimal? Width { get; set; }

        [DataMapping("Height", DbType.Int32)]
        public decimal? Height { get; set; }

        [DataMapping("IsTakePictures", DbType.String)]
        public string IsTakePictures { set; get; }

        [DataMapping("PicNumber", DbType.Int32)]
        public int PicNumber { get; set; }

        [DataMapping("Attention", DbType.String)]
        public string Attention { set; get; }

        [DataMapping("OnlyForRank", DbType.Int32)]
        public int OnlyForRank { get; set; }

        [DataMapping("IsOfferInvoice", DbType.String)]
        public string IsOfferInvoice { set; get; }

        [DataMapping("VirtualPrice", DbType.Int32)]
        public decimal? VirtualPrice { get; set; }

        [DataMapping("Status", DbType.String)]
        public string Status { get; set; }

        [DataMapping("Type", DbType.String)]
        public string Type { get; set; }

        [DataMapping("Auditor", DbType.String)]
        public string Auditor { get; set; }

        [DataMapping("AuditDate", DbType.DateTime)]
        public DateTime? AuditDate { get; set; }

        [DataMapping("CompanyCode", DbType.String)]
        public string CompanyCode { get; set; }

        [DataMapping("StoreCompanyCode", DbType.String)]
        public string StoreCompanyCode { get; set; }

        [DataMapping("LanguageCode", DbType.String)]
        public string LanguageCode { get; set; }

        [DataMapping("InDate", DbType.DateTime)]
        public DateTime InDate { get; set; }

        [DataMapping("InUser", DbType.String)]
        public string InUser { get; set; }

        [DataMapping("EditDate", DbType.DateTime)]
        public DateTime? EditDate { get; set; }

        [DataMapping("EditUser", DbType.String)]
        public string EditUser { get; set; }

        [DataMapping("CurrentPrice", DbType.Single)]
        public decimal? CurrentPrice { get; set; }

        [DataMapping("BasicPrice", DbType.Single)]
        public decimal? BasicPrice { get; set; }

        [DataMapping("GrossMarginRate", DbType.Single)]
        public decimal? GrossMarginRate { get; set; }

        [DataMapping("IsConsign", DbType.Int32)]
        public int IsConsign { get; set; }

        [DataMapping("PMUserSysNo", DbType.Int32)]
        public int PMUserSysNo { get; set; }

        [DataMapping("C3Name", DbType.String)]
        public string C3Name { get; set; }

        [DataMapping("C2Name", DbType.String)]
        public string C2Name { get; set; }

        [DataMapping("C1Name", DbType.String)]
        public string C1Name { get; set; }

        [DataMapping("PMName", DbType.String)]
        public string PMName { get; set; }

        [DataMapping("BrandName", DbType.String)]
        public string BrandName { get; set; }

        [DataMapping("ManufacturerName", DbType.String)]
        public string ManufacturerName { get; set; }

        [DataMapping("SellerName", DbType.String)]
        public string SellerName { get; set; }

        [DataMapping("SellerSite", DbType.String)]
        public string SellerSite { get; set; }

        [DataMapping("ProductGroupMode", DbType.String)]
        public string ProductGroupMode { get; set; }

        [DataMapping("ProductDescLong", DbType.String)]
        public string ProductDescLong { get; set; }

        [DataMapping("ProcessCount", DbType.Int32)]
        public Int32 ProcessCount { get; set; }
    }
}
