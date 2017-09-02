using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECommerce.Entity
{
    /// <summary>
    /// 品牌信息
    /// </summary>
    [XmlRoot("FloorItemBrand")]
    public class FloorItemBrand : FloorItemBase
    {
        /// <summary>
        /// 获取或设置品牌编号
        /// </summary>
        [XmlElement("BrandSysNo")]
        public int BrandSysNo { get; set; }

        /// <summary>
        /// 获取或设置图片地址
        /// </summary>
        [XmlElement("ImageSrc")]
        public string ImageSrc { get; set; }

        /// <summary>
        /// 获取或设置品牌文字信息
        /// </summary>
        [XmlElement("BrandText")]
        public string BrandText { get; set; }

        /// <summary>
        /// 获取或设置链接地址
        /// </summary>
        [XmlElement("LinkUrl")]
        public string LinkUrl { get; set; }
    }
}
