using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECommerce.Entity
{
    [XmlRoot("FloorItemTextLink")]
    public class FloorItemTextLink : FloorItemBase
    {
        /// <summary>
        /// 获取或设置文字内容
        /// </summary>
        [XmlElement("Text")]
        public string Text { get; set; }

        /// <summary>
        /// 获取或设置链接地址
        /// </summary>
        [XmlElement("LinkUrl")]
        public string LinkUrl { get; set; }

        /// <summary>
        /// 获取或设置热点文字（标红）
        /// </summary>
        [XmlElement("IsHot")]
        public bool IsHot { get; set; }
    }
}
