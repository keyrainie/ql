using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Nesoft.ECWeb.MobileService.Models.Product.Config
{
    [Serializable]
    public class ImageSizeConfig
    {
        [XmlArray("Settings")]
        [XmlArrayItem("ImageSizeSetting",typeof(ImageSizeSetting))]
        public List<ImageSizeSetting> Settings { get; set; }
    }
}