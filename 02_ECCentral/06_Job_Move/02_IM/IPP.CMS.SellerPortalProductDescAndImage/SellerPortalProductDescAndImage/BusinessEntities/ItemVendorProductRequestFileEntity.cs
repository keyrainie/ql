using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.ContentMgmt.SellerPortalProductDescAndImage.BusinessEntities
{
    public class ItemVendorProductRequestFileEntity
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { set; get; }

        [DataMapping("ProductRequestSysno", DbType.Int32)]
        public int ProductRequestSysno { set; get; }

        [DataMapping("Title", DbType.String)]
        public string Title { set; get; }

        [DataMapping("Path", DbType.String)]
        public string Path { set; get; }

        [DataMapping("Memo", DbType.String)]
        public string Memo { set; get; }

        [DataMapping("Type", DbType.String)]
        public string Type { set; get; }

        [DataMapping("ImageName", DbType.String)]
        public string ImageName { set; get; }

        [DataMapping("Status", DbType.String)]
        public string Status { set; get; }

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

        [DataMapping("GroupSysNo", DbType.Int32)]
        public int GroupSysNo { set; get; }

        [DataMapping("comskuSysNo", DbType.Int32)]
        public int comskuSysNo { set; get; }

        [DataMapping("CommonSKUNumber", DbType.String)]
        public string CommonSKUNumber { set; get; }
    }
}
