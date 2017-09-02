using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 属性值信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class PropertyValueInfo : IIdentity
    {
        /// <summary>
        /// 属性
        /// </summary>
        [DataMember]
        public PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// 属性值描述
        /// </summary>
        [DataMember]
        public LanguageContent ValueDescription { get; set; }

        /// <summary>
        /// 属性值状态
        /// </summary>
        [DataMember]
        public PropertyStatus ValueStatus { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        [DataMember]
        public int Priority { get; set; }

        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

    }
}
