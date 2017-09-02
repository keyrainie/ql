using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.Entity;
using System.Data;
using System.Xml.Linq;

namespace IPP.ContentMgmt.BatchUpdateItemKeywords.Entities
{
    public class CategoryKeyWordsEntity
    {
        [DataMapping("SysNo", DbType.Int32)]
        public int? SysNo { get; set; }

        /// <summary>
        /// c3 sysno
        /// </summary>
        [DataMapping("C3SysNo", DbType.Int32)]
        public int? C3SysNo { get; set; }

        /// <summary>
        /// 类别关键字
        /// </summary>
        [DataMapping("CommonKeywords", DbType.String)]
        public string CommonKeywords { get; set; }


        private string m_propertyKeywords;
        /// <summary>
        /// 类别属性sysno
        /// </summary>
        [DataMapping("PropertyKeywords", DbType.String)]
        public string PropertyKeywords { get; set; }        

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMapping("InDate", DbType.DateTime)]
        public DateTime? InDate { get; set; }

        /// <summary>
        /// 创建者名称
        /// </summary>
        [DataMapping("InUser", DbType.String)]
        public string InUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMapping("EditDate", DbType.DateTime)]
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 修改者名称
        /// </summary>
        [DataMapping("EditUser", DbType.String)]
        public string EditUser { get; set; }

        [DataMapping("CompanyCode", DbType.String)]
        public string CompanyCode { get; set; }

        [DataMapping("LanguageCode", DbType.String)]
        public string LanguageCode { get; set; }

        [DataMapping("StoreCompanyCode", DbType.String)]
        public string StoreCompanyCode { get; set; }
    }
}
