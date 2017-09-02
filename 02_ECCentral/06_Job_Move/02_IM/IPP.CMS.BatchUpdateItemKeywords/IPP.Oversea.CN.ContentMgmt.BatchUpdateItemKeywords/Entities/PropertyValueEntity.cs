using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.ContentMgmt.BatchUpdateItemKeywords.Entities
{
    public class PropertyValueEntity
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int? SysNo { get; set; }

        /// <summary>
        /// 属性sysno
        /// </summary>
        [DataMapping("PropertySysNo", DbType.Int32)]
        public int? PropertySysNo { get; set; }

        /// <summary>
        /// 属性值描述
        /// </summary>
        [DataMapping("ValueDescription", DbType.String)]
        public string ValueDescription { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        [DataMapping("Priority", DbType.Int32)]
        public int? Priority { get; set; }

        /// <summary>
        /// 最后修改者sysno
        /// </summary>
        [DataMapping("LastEditUserSysNo", DbType.Int32)]
        public int? LastEditUserSysNo { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        [DataMapping("LastEditTime", DbType.DateTime)]
        public DateTime? LastEditTime { get; set; }

        [DataMapping("CompanyCode", DbType.String)]
        public string CompanyCode { get; set; }

        [DataMapping("LanguageCode", DbType.String)]
        public string LanguageCode { get; set; }

        [DataMapping("StoreCompanyCode", DbType.String)]
        public string StoreCompanyCode { get; set; }

        /// <summary>
        /// 属性值，用户输入
        /// </summary>
        [DataMapping("ManualInput", DbType.String)]
        public string ManualInput { get; set; }

        /// <summary>
        /// 产品sysno
        /// </summary>
        [DataMapping("ProductSysNo", DbType.Int32)]
        public int? ProductSysNo { get; set; }

        ///// <summary>
        ///// 属性sysno
        ///// </summary>
        //[DataMapping("PropertySysNo", DbType.Int32)]
        //public int? PropertySysNo { get; set; }

    }
}
