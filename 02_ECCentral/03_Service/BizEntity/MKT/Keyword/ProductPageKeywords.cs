using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    ///产品页面关键字
    /// </summary>
    public class ProductPageKeywords : IIdentity, IWebChannel
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// keywords0为商品所属类别关键字。维护时可以添加，编辑。对应显示页面当中 的Keywords1
        /// </summary>
        public LanguageContent Keywords { get; set; }

        /// <summary>
        /// keywords0为商品所属类别关键字。维护时可以添加，编辑。
        /// </summary>
        public LanguageContent Keywords0 { get; set; }

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

        public string ProductId { get; set; }
    }
}
