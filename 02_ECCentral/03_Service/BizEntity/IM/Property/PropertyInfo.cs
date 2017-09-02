using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 属性信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class PropertyInfo : IIdentity
    {

        /// <summary>
        /// 属性名称
        /// </summary>
        [DataMember]
        public LanguageContent PropertyName { get; set; }

        /// <summary>
        /// 属性状态
        /// </summary>
        [DataMember]
        public PropertyStatus Status { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

    }
}
