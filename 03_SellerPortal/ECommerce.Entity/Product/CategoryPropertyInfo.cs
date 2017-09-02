using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 商品分类属性
    /// </summary>
    [Serializable]
    [DataContract]
    public class CategoryPropertyInfo : EntityBase
    {
        /// <summary>
        /// 属性编号
        /// </summary>
        [DataMember]
        public int PropertySysNo { get; set; }
        /// <summary>
        /// 属性名
        /// </summary>
        [DataMember]
        public string PropertyName { get; set; }
        /// <summary>
        /// 属性组编号
        /// </summary>
        [DataMember]
        public int PropertyGroupSysNo { get; set; }
        /// <summary>
        /// 属性组名
        /// </summary>
        [DataMember]
        public string PropertyGroupName { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        [DataMember]
        public int Priority { get; set; }
        /// <summary>
        /// 是否必须输入
        /// </summary>
        [DataMember]
        public int IsMustInput { get; set; }
        /// <summary>
        /// 属性类型
        /// </summary>
        [DataMember]
        public string PropertyType { get; set; }
        /// <summary>
        /// C3
        /// </summary>
        [DataMember]
        public int CategorySysNo { get; set; }
        /// <summary>
        /// 属性状态
        /// </summary>
        [DataMember]
        public int PropertyStatus { get; set; }
        /// <summary>
        /// 是否是分组属性
        /// </summary>
        [DataMember]
        public int IsSplitGroupProperty { get; set; }
    }
}
