using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT
{
    /// <summary>
    /// 新闻类型
    /// </summary>
    public class NewsType : IIdentity, IWebChannel, ILanguage
    {
        /// <summary>
        /// 类型编码
        /// </summary>
        public int NewsTypeCode
        {
            get;
            set;
        }

        /// <summary>
        /// 类型名称
        /// </summary>
        public string NewsTypeName
        {
            get;
            set;
        }
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 所属公司
        /// </summary>
        public string CompanyCode
        {
            get;
            set;
        }
        /// <summary>
        /// 语言编码
        /// </summary>
        public string LanguageCode
        {
            get;
            set;
        }
        /// <summary>
        /// 所属渠道
        /// </summary>
        public Common.WebChannel WebChannel
        {
            get;
            set;
        }
    }
}