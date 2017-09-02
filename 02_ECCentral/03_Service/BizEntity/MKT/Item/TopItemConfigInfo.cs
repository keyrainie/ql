using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 置顶商品相关的配置
    /// </summary>
    public class TopItemConfigInfo : IWebChannel
    {
        /// <summary>
        /// 引用系统编号
        /// </summary>
        public int? RefSysNo { get; set; }
        /// <summary>
        /// 页面类型
        /// </summary>
        public int? PageType { get; set; }
        /// <summary>
        /// 是否发送过期邮件提醒
        /// </summary>
        public bool? ISSendMailStore { get; set; }
        /// <summary>
        /// 是否无货后前台取消置顶
        /// </summary>
        public bool? ISShowTopStore { get; set; }
        /// <summary>
        /// 所属渠道
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }
        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode { get; set; }


    }
}
