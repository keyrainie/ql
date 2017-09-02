using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Member
{
    /// <summary>
    ///【 Ecommerce.[dbo].ThirdPartyUser】
    /// </summary>
    public class ThirdPartyUserInfo
    {
        /// <summary>
        /// 用户主键
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 合作伙伴ID
        /// </summary>
        public string PartnerUserID { get; set; }

        /// <summary>
        /// 合作伙伴邮件地址
        /// </summary>
        public string PartnerEmail { get; set; }

        /// <summary>
        /// 用户来源
        /// </summary>
        public string UserSource { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string CustomerID { get; set; }

        /// <summary>
        /// 用户主键
        /// </summary>
        public int CustomerSysNo{get;set;}

        /// <summary>
        /// 用户来源2
        /// </summary>
        public string SubSource { get; set; }

        /// <summary>
        /// 用户来源2
        /// </summary>
        public string SourceName { get; set; }
    }
}
