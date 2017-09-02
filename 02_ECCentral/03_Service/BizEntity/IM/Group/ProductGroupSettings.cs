using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 分组属性
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductGroupSettings : IIdentity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 分组属性
        /// </summary>
        [DataMember]
        public PropertyInfo ProductGroupProperty { get; set; }

        /// <summary>
        /// 属性别名
        /// </summary>
        [DataMember]
        public LanguageContent PropertyBriefName { get; set; }

        /// <summary>
        /// 是否图片显示
        /// </summary>
        [DataMember]
        public ProductGroupImageShow ImageShow { get; set; }

        /// <summary>
        /// 是否聚合
        /// </summary>
        [DataMember]
        public ProductGroupPolymeric Polymeric { get; set; }
    }
}
