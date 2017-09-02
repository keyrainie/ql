using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Newegg.Oversea.Framework.Entity;

namespace IPP.Oversea.CN.ContentMgmt.Baidu.Entities
{
    public class ItemPropertyEntity
    {
        public string CompanyCode { get; set; }

        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }

        [DataMapping("ManualInput", DbType.String)]
        public string ManualInput { get; set; }

        [DataMapping("ValueDescription", DbType.String)]
        public string ValueDescription { get; set; }
    }
}
