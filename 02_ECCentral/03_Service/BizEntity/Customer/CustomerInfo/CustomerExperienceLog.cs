using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Customer
{
    /// <summary>
    /// 将验证日志
    /// </summary>
    [Serializable]
    [DataContract]
    public class CustomerExperienceLog : IIdentity
    {
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

        #region IIdentity Members
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        #endregion
    }
}
