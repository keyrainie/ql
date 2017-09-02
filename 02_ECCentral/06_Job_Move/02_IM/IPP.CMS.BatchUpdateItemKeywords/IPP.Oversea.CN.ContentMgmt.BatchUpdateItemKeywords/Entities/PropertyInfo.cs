using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.ContentMgmt.BatchUpdateItemKeywords.Entities
{
    public class PropertyInfo
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int SysNo { get; set; }

        [DataMapping("ProductSysNo", DbType.Int32)]
        public int ProductSysNo { get; set; }

        [DataMapping("PropertySysNo", DbType.Int32)]
        public int PropertySysNo { get; set; }

        [DataMapping("ValueSysNo", DbType.Int32)]
        public int ValueSysNo { get; set; }

        [DataMapping("ManualInput", DbType.String)]
        public string ManualInput { get; set; }

        [DataMapping("ValueDescription", DbType.String)]
        public string ValueDescription { get; set; }

    }
}
