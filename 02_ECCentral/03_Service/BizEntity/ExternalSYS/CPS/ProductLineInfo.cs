using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.ExternalSYS
{
    public class ProductLineInfo : ICompany, ILanguage
    {


        public int SysNo { get; set; }
        /// <summary>
        /// 产品线分类SysNO
        /// </summary>
        public int ProductLineCategorySysNo { get; set; }
        /// <summary>
        /// 产品线名称
        /// </summary>
        public string ProductLineName { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 使用范围描述
        /// </summary>
        public string UseScopeDescription { get; set; }

        public UserInfo User { get; set; }



        public string CompanyCode
        {
            get;
            set;
        }

        public string LanguageCode
        {
            get;
            set;
        }
    }
}
