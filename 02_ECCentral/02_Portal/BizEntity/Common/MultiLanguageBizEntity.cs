using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Common
{

     /// <summary>
    /// 多语言业务对象实体
    /// </summary>
    [Serializable]
    [DataContract]
    public class MultiLanguageDataContract
    {
        /// <summary>
        /// 业务对象主键：系统编号
        /// </summary>
        [DataMember]
        public int SysNo { get; set; }

        /// <summary>
        /// 业务对象类型，用字符串标识
        /// </summary>
        [DataMember]
        public string BizEntityType { get; set; }

    }

    /// <summary>
    /// 多语言业务对象实体
    /// </summary>
    [Serializable]
    [DataContract]
    public class MultiLanguageBizEntity
    {
        /// <summary>
        /// 业务对象主键：系统编号
        /// </summary>
        [DataMember]
        public int SysNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string LanguageName { get; set; }

        /// <summary>
        /// 业务对象类型，用字符串标识
        /// </summary>
        [DataMember]
        public string BizEntityType { get; set; }

        /// <summary>
        /// 业务对象对应的Table名
        /// </summary>
        [DataMember]
        public string MappingTable { get; set; }

        /// <summary>
        /// 业务对象的语言编码
        /// </summary>
        [DataMember]
        public string LanguageCode { get; set; }

        [DataMember]
        public List<PropertyItem> PropertyItemList { get; set; }
    }


    /// <summary>
    /// 多语言业务对象的属性
    /// </summary>
    [Serializable]
    [DataContract]
    public class PropertyItem
    {
        /// <summary>
        /// 属性Label
        /// </summary>
        [DataMember]
        public string Label { get; set; }

        /// <summary>
        /// 属性MaxLength
        /// </summary>
        [DataMember]
        public int MaxLength { get; set; }

        /// <summary>
        /// 属性是否必填，Y为必填，N为非必填
        /// </summary>
        [DataMember]
        public string IsRequired { get; set; }

        /// <summary>
        /// 属性字段
        /// </summary>
        [DataMember]
        public string Field { get; set; }

        /// <summary>
        /// 属性值
        /// </summary>
        [DataMember]
        public string Value { get; set; }

        /// <summary>
        /// 输入框模式：S=单行文本框，M=多行文本框
        /// </summary>
        [DataMember]
        public string InputType { get; set; }
 
    }
}
