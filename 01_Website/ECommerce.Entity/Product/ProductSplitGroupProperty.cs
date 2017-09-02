using System;
using System.Runtime.Serialization;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 商品分组属性
    /// </summary>
    [Serializable]
    [DataContract]
    public class ProductSplitGroupProperty
    {
        /// <summary>
        /// 属性组编号
        /// </summary>
        [DataMember]
        public int ProductGroupSysno { get; set; }
        /// <summary>
        /// 属性编号
        /// </summary>
        [DataMember]
        public int PropertySysno { get; set; }
        /// <summary>
        /// 属性名
        /// </summary>
        [DataMember]
        public string PropertyDescription { get; set; }
        /// <summary>
        /// 属性值编号
        /// </summary>
        [DataMember]
        public int ValueSysno { get; set; }
        /// <summary>
        /// 属性值名称
        /// </summary>
        [DataMember]
        public string ValueDescription { get; set; }
    }
}
