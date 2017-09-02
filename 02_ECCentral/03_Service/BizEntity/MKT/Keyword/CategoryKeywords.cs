using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 分类关键字
    /// </summary>
    public class CategoryKeywords : IIdentity, IWebChannel
    {
        /// <summary>
        /// 商品一级类编号
        /// </summary>
        public int? Category1SysNo { get; set; }

        /// <summary>
        /// 商品二级类编号
        /// </summary>
        public int? Category2SysNo { get; set; }

        /// <summary>
        /// 商品三级类编号
        /// </summary>
        public int? Category3SysNo { get; set; }

        /// <summary>
        /// 通用关键字
        /// </summary>
        public LanguageContent CommonKeywords { get; set; }

        /// <summary>
        /// 属性关键字ID集合
        /// </summary>
        //public string PropertyKeywords { get; set; }

        /// <summary>
        /// 属性关键字    属性ID集合
        /// </summary>
        public string PropertyKeywords { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 对应的渠道
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }
    }

    /// <summary>
    /// 产品关键字队列表
    /// </summary>
    public class ProductKeywordsQueue
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// C3类别编号
        /// </summary>
        public int? C3SysNo { get; set; }

        /// <summary>
        /// 产品编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }
    }

    /// <summary>
    /// 产品属性对应表
    /// </summary>
    public class PropertyInfo
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 产品编号
        /// </summary>
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 属性编号
        /// </summary>
        public int PropertySysNo { get; set; }

        /// <summary>
        /// 属性值对应编号
        /// </summary>
        public int ValueSysNo { get; set; }

        /// <summary>
        /// 手动输入值
        /// </summary>
        public string ManualInput { get; set; }

        /// <summary>
        /// 输入值描述
        /// </summary>
        public string ValueDescription { get; set; }
    }
}
