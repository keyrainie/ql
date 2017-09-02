using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Nesoft.ECWeb.MobileService.Models.Category.Config
{
    [Serializable]
    public class CategoryIconsConfig
    {
        [XmlElement("category", Type=typeof(CategoryItemConfig))]
        public List<CategoryItemConfig> CatList { get; set; }
    }
}