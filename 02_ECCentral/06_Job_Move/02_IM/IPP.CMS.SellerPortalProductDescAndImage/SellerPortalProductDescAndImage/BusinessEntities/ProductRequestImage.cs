using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.ContentMgmt.SellerPortalProductDescAndImage.BusinessEntities
{
    [Serializable]
    public class ProductRequestImage
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("ImageUrl", DbType.String)]
        public string ImageUrl { get; set; }

        [DataMapping("ImageName", DbType.String)]
        public string ImageName { get; set; }

        [DataMapping("Status", DbType.String)]
        public string Status { get; set; }

        [DataMapping("Memo", DbType.String)]
        public string Memo { get; set; }

        [DataMapping("ProductRequestSysNo", DbType.Int32)]
        public int ProductRequestSysNo { get; set; }

        [DataMapping("ProcessCount", DbType.Int32)]
        public int ProcessCount { get; set; }
    }
}
