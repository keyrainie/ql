using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 商城首页区域
    /// </summary>
    public class HomePageSectionInfo :IIdentity,IWebChannel,ILanguage
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 所属渠道
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }

        /// <summary>
        /// 语言代码
        /// </summary>
        public string LanguageCode { get; set; }

        /// <summary>
        /// 商城首页区域编号
        /// </summary>
        public string DomainCode { get; set; }

        /// <summary>
        /// 商城首页区域名称
        /// </summary>
        public string DomainName { get; set; }

        /// <summary>
        /// 商城首页区域推荐商品不足时，从C1List中的指定一级分类自动补充商品到推荐位
        /// </summary>
        public string C1List { get; set; }

        /// <summary>
        /// 商城首页区域推荐商品不足时，从C1List中的一级分类自动补充商品到推荐位时，排除指定三级分类下的商品
        /// </summary>
        public String ExceptC3List { get; set; }

        /// <summary>
        /// 展示优先级
        /// </summary>
        public int? Priority { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ADStatus Status { get; set; }
    }
}
