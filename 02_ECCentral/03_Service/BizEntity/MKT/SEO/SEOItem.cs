using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// SEO优化表
    /// </summary>
    public class SEOItem : IIdentity, IWebChannel
    {
        /// <summary>
        /// 页面类型
        /// </summary>
        public int? PageType { get; set; }

        /// <summary>
        /// 页面ID
        /// </summary>
        public int? PageID { get; set; }

        /// <summary>
        /// 状态  A=有效  D=无效
        /// </summary>
        public ADStatus Status { get; set; }

        /// <summary>
        /// 页面标题
        /// </summary>
        public string PageTitle { get; set; }

        /// <summary>
        /// 扩展生效
        /// </summary>
        public bool? IsExtendValid { get; set; }

        /// <summary>
        /// 页面描述
        /// </summary>
        public LanguageContent PageDescription { get; set; }

        /// <summary>
        /// 页面关键字
        /// </summary>
        public LanguageContent PageKeywords { get; set; }

        /// <summary>
        /// 页面附加代码（HTML 用于优化SEO的HTML代码
        /// </summary>
        public string PageAdditionContent { get; set; }

        /// <summary>
        /// 商品范围ID集合
        /// </summary>
        public List<SeoProductItem> ProductList { get; set; }

        /// <summary>
        /// 类别集合
        /// </summary>
        public List<SeoCategory> CategoryList { get; set; }
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

        public UserInfo User { get; set; }
    }

    /// <summary>
    /// 为Seo商品范围服务的product类
    /// </summary>
    public class SeoProductItem
    {
        public int? SysNo{get;set;}
        public string ProductId{get;set;}
    }
    /// <summary>
    /// 为Seo商品范围服务的Category类
    /// </summary>
    public class SeoCategory
    {
        public int? SysNo{get;set;}
        public string CategoryName{get;set;}
    }
}
