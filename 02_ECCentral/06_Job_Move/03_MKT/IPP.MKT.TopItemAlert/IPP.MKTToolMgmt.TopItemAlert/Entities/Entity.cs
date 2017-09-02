using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;

namespace IPP.Oversea.CN.ContentMgmt.Baidu.Entities
{
    public class TopItemEntity
    {
        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }

        [DataMapping("ProductID", DbType.String)]
        public string ProductID { get; set; }

        [DataMapping("ProductName", DbType.String)]
        public string ProductName { get; set; }

        [DataMapping("EmailAddress", DbType.String)]
        public string EmailAddress { get; set; }

        [DataMapping("UserSysNo", DbType.Int32)]
        public int UserSysNo { get; set; }

        [DataMapping("CategorySysNo", DbType.Int32)]
        public int CategorySysNo { get; set; }
    }

    public class AdminEntity
    {
        [DataMapping("EmailAddress", DbType.String)]
        public string EmailAddress { get; set; }

        [DataMapping("CategorySysNo", DbType.Int32)]
        public int CategorySysNo { get; set; }

        [DataMapping("CategoryName", DbType.String)]
        public string CategoryName { get; set; }

        [DataMapping("UserSysNo", DbType.Int32)]
        public int UserSysNo { get; set; }
    }
}
