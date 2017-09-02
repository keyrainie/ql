using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT.Adv
{
    /// <summary>
    /// BBS推广报表
    /// </summary>
    public class AdvEffectMonitorBBS : IIdentity, IWebChannel, ILanguage
    {
        /// <summary>
        /// 对应的用户SysNo
        /// </summary>
        public int? CustomerSysNo { get; set; }

        /// <summary>
        /// 对应的BBS ID
        /// </summary>
        public string BBSID { get; set; }

        /// <summary>
        /// 点击次数
        /// </summary>
        public int? ClickCount { get; set; }

        /// <summary>
        /// 对应的IP总数量
        /// </summary>
        public int? IPCount { get; set; }

        /// <summary>
        /// 对应的图片地址
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// 多语言
        /// </summary>
        public string LanguageCode { get; set; }

        /// <summary>
        /// 对应的渠道
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public int? SysNo { get; set; }
    }
}
