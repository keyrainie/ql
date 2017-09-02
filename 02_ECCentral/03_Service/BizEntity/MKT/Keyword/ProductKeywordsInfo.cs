using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 关键字对应商品
    /// </summary>
    public class ProductKeywordsInfo : IIdentity, IWebChannel
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public LanguageContent Keywords { get; set; }

        /// <summary>
        /// 对应的商品编号
        /// </summary>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// 状态      D=无效  A=有效
        /// </summary>
        public ADStatus Status { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int? Priority { get; set; }

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
}
