using Newegg.Oversea.Framework.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace IPP.ContentMgmt.SellerPortalProductDescAndImage.BusinessEntities
{
    public class ProductGroupInfoEntity
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("ProductName", DbType.String)]
        public string ProductName { get; set; }

        [DataMapping("BrandSysNo", DbType.Int32)]
        public int BrandSysNo { get; set; }

        [DataMapping("Category3SysNo", DbType.Int32)]
        public int Category3SysNo { get; set; }

        [DataMapping("ProductModel", DbType.String)]
        public string ProductModel { get; set; }

        [DataMapping("ProductDesc", DbType.String)]
        public string ProductDesc { get; set; }

        [DataMapping("Is360Show", DbType.String)]
        public string Is360Show { get; set; }

        [DataMapping("IsVideo", DbType.String)]
        public string IsVideo { get; set; }

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
        public DateTime EditDate { get; set; }

        [DataMapping("EditUser", DbType.String)]
        public string EditUser { get; set; }

        [DataMapping("ProductDescLong", DbType.String)]
        public string ProductDescLong { get; set; }

        [DataMapping("ProductPhotoDesc", DbType.String)]
        public string ProductPhotoDesc { get; set; }

        [DataMapping("BriefName", DbType.String)]
        public string BriefName { get; set; }

        [DataMapping("ProductMode", DbType.String)]
        public string ProductMode { get; set; }

        [DataMapping("ProductType", DbType.String)]
        public string ProductType { get; set; }

        [DataMapping("Type", DbType.String)]
        public string Type { get; set; }

        [DataMapping("IsConsign", DbType.Int32)]
        public int IsConsign { get; set; }

        [DataMapping("IsTakePictures", DbType.Int32)]
        public string IsTakePictures { get; set; }

        [DataMapping("ManufacturerSysNo", DbType.Int32)]
        public int? ManufacturerSysNo { get; set; }

        [DataMapping("CompanyProduct", DbType.String)]
        public string CompanyProduct { get; set; }

        [DataMapping("PreviousProductSysNo", DbType.Int32)]
        public int? PreviousProductSysNo { get; set; }

        [DataMapping("PMUserSysNo", DbType.Int32)]
        public int PMUserSysNo { get; set; }

        [DataMapping("PropertySysno1", DbType.Int32)]
        public int? PropertySysno1 { get; set; }

        [DataMapping("PropertySysno2", DbType.Int32)]
        public int? PropertySysno2 { get; set; }

        [DataMapping("IsPolymeric1", DbType.String)]
        public string IsPolymeric1 { get; set; }

        [DataMapping("IsPolymeric2", DbType.String)]
        public string IsPolymeric2 { get; set; }

        [DataMapping("IsDisplayPic1", DbType.String)]
        public string IsDisplayPic1 { get; set; }

        [DataMapping("IsDisplayPic2", DbType.String)]
        public string IsDisplayPic2 { get; set; }

        [DataMapping("ShowName1", DbType.String)]
        public string ShowName1 { get; set; }

        [DataMapping("ShowName2", DbType.String)]
        public string ShowName2 { get; set; }

        public List<ItemGroupCommonInfoEntity> CommItemList
        {
            get;
            set;
        }
    }
}
