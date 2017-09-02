using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECommerce.Entity
{
    [XmlRoot("FloorItemBanner")]
    public class FloorItemBanner : FloorItemBase
    {
        public FloorItemBanner() { }


        /// <summary>
        /// 获取或设置广告图片信息
        /// </summary>
        [XmlElement("ImageSrc")]
        public string ImageSrc { get; set; }

        /// <summary>
        /// 获取或设置广告文字
        /// </summary>
        [XmlElement("BannerText")]
        public string BannerText { get; set; }

        /// <summary>
        /// 获取或设置广告链接地址
        /// </summary>
        [XmlElement("LinkUrl")]
        public string LinkUrl { get; set; }
    }
}
