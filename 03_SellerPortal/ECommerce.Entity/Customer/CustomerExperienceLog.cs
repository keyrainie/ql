using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Customer
{
    /// <summary>
    /// 将验证日志
    /// </summary>
    [Serializable]
    [DataContract]
    public class CustomerExperienceLog 
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }
        [DataMember]
        public int? CustomerSysNo { get; set; }
        /// <summary>
        /// 日志类型
        /// </summary>
        [DataMember]
        public ExperienceLogType? Type { get; set; }
        /// <summary>
        /// 经验值数量
        /// </summary>
        [DataMember]
        public decimal? Amount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Memo { get; set; }

    }
}
