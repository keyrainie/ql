using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Nesoft.ECWeb.MobileService.Models.Product.Config
{
    [Serializable]
    public class ImageDeviceInfo
    {
        [XmlAttribute("ImageType")]
        public string ImageType { get; set; }

        [XmlAttribute("ImageSize")]
        public string ImageSize { get; set; }
    }
}