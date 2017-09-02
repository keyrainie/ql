using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 广告商管理
    /// </summary>
    public class Advertisers : IIdentity, IWebChannel, ILanguage
    {
        /// <summary>
        /// 广告商名称
        /// </summary>
        public string AdvertiserName { get; set; }

        /// <summary>
        /// 监测代码 特定格式：至少输入两层代码（以-_-分隔）
        /// </summary>
        public string MonitorCode { get; set; }

        /// <summary>
        /// Cookie有效期 单位：天
        /// </summary>  
        public int? CookiePeriod { get; set; }

        /// <summary>
        /// 广告效果查询账号
        /// </summary>
        public string AdvertiserUserName { get; set; }

        /// <summary>
        /// 广告效果密码
        /// </summary>
        public string AdvertiserPassword { get; set; }

        /// <summary>
        /// 效果查询链接
        /// </summary>
        public string EffectLink { get; set; }

        /// <summary>
        /// 状态  A：有效；D：无效
        /// </summary>
        public ADStatus Status { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 多语言
        /// </summary>
        public string LanguageCode { get; set; }

        /// <summary>
        /// 对应的渠道
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }
    }
}
