using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.ContentMgmt.BatchUpdateItemKeywords.Entities
{
    public class ProductKeywordsQueue
    {
        [DataMapping("ProductSysNo", DbType.Int32)]
        public int? ProductSysNo { get; set; }

        [DataMapping("C3SysNo", DbType.Int32)]
        public int? C3SysNo { get; set; }
    }
}
