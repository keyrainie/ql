using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Nesoft.ECWeb.MobileService.Models.Product.Config
{
    [Serializable]
    public class ImageSizeSetting
    {
        public string ImageResolution { get; set; }

        [XmlArray("ImageSizes")]
        [XmlArrayItem("ImageDeviceInfo", typeof(ImageDeviceInfo))]
        public List<ImageDeviceInfo> ImageSizes { get; set; }
    }
}