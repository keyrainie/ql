using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace ECommerce.Entity.Product
{
    [Serializable]
    [DataContract]
    public class ProductPropertyInfo : EntityBase
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
        /// 属性值编号
        /// </summary>
        [DataMember]
        public int PropertyValueSysNo { get; set; }
        /// <summary>
        /// 属性值名称
        /// </summary>
        [DataMember]
        public string ValueDescription { get; set; }
        /// <summary>
        /// 是否是分组属性
        /// </summary>
        [DataMember]
        public int IsSplitGroupProperty { get; set; }
    }

    public class LongDescription
    {
        [XmlElement("Group")]
        public List<Group> Groups { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
    }

    public class Group
    {

        [XmlElement("Property")]
        public List<Property> Propertys { get; set; }
        [XmlAttribute]
        public string GroupName { get; set; }
    }

    public class Property
    {
        [XmlAttribute]
        public string Key { get; set; }
        [XmlAttribute]
        public string Value { get; set; }
    }
}
