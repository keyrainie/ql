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
    public class CategoryPropertyValueInfo : EntityBase
    {
        /// <summary>
        /// 属性值编号
        /// </summary>
        [DataMember]
        public int PropertyValueSysNo { get; set; }
        /// <summary>
        /// 属性编号
        /// </summary>
        [DataMember]
        public int PropertySysNo { get; set; }
        /// <summary>
        /// 属性值名称
        /// </summary>
        [DataMember]
        public string ValueDescription { get; set; }
        /// <summary>
        /// 优先级
        /// </summary>
        [DataMember]
        public int Priority { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        public int IsActive { get; set; }
    }
}
