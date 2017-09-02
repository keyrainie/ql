using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 跳转关键字
    /// </summary>
    public class AdvancedKeywordsInfo : IIdentity, IWebChannel
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public LanguageContent Keywords { get; set; }

        /// <summary>
        /// 状态  D=无效  A=有效
        /// </summary>
        public ADStatus Status { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int? Priority { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        public string LinkUrl { get; set; }

        /// <summary>
        /// 显示名称
        /// </summary>
        public LanguageContent ShowName { get; set; }

        /// <summary>
        /// 是否自动转换？？0=否  1=是
        /// </summary>
        public NYNStatus AutoRedirectSwitch { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? BeginDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

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
    /// 关键字操作日志表
    /// </summary>
    public class AdvancedKeywordsLog : IIdentity
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 对应的跳转关键字编号
        /// </summary>
        public int? AdvancedKeywordsInfoSysNo { get; set; }

        /// <summary>
        /// 操作
        /// </summary>
        public string Operation { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime InDate { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public string InUser { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 多语言
        /// </summary>
        public string LanguageCode { get; set; }
    }
}
