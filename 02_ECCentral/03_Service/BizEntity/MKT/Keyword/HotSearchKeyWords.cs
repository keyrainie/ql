using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 热门关键字
    /// </summary>
    public class HotSearchKeyWords : IIdentity, IWebChannel
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public LanguageContent Keywords { get; set; }

        /// <summary>
        /// 页面类型    提供选择PageID
        /// </summary>
        public int? PageType { get; set; }

        /// <summary>
        /// 页面ID
        /// </summary>
        public int? PageID { get; set; }

        /// <summary>
        /// 编辑时间
        /// </summary>
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 屏蔽时间
        /// </summary>
        public DateTime? HiddenDate { get; set; }

        /// <summary>
        /// 扩展生效
        /// </summary>
        public bool? Extend { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public NYNStatus? IsOnlineShow { get; set; }

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
    }
}
