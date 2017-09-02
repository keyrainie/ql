using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.MKT.Adv
{
    /// <summary>
    /// 广告监视
    /// </summary>
    public class AdvEffectMonitor : IIdentity, IWebChannel, ILanguage
    {
        /// <summary>
        /// cm_mmc关键字
        /// </summary>
        public string CMP { get; set; }

        /// <summary>
        /// 监视动作类型
        /// </summary>
        public string OperationType { get; set; }

        /// <summary>
        ///  广告效果监视对应的客户信息
        /// </summary>
        public ECCentral.BizEntity.Customer.CustomerInfo CustomerInfo { get; set; }

        /// <summary>
        /// 广告效果监视对应的SO信息
        /// </summary>
        public ECCentral.BizEntity.SO.SOInfo So { get; set; }

        /// <summary>
        /// 对应的SysNO
        /// </summary>
        public int? ReferenceSysNo { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public int? SysNo{ get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 语言
        /// </summary>
        public string LanguageCode { get; set; }

        /// <summary>
        /// 渠道
        /// </summary>
        public Common.WebChannel WebChannel { get; set; }
    }
}
