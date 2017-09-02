using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Newegg.Oversea.Framework.Entity;
using System.Data;

namespace IPP.Oversea.CN.ContentMgmt.Baidu.Entities
{
    [XmlRoot(ElementName = "BaiduPlatformCategory1Data")]
    public class BaiduPlatformCategory1DataConfigurationEntity  
    {
        [XmlElement(ElementName = "Category1")]
        public List<Category1ConfigurationEntity> BaiduCategory1List { get; set; }
    }

    public class Category1ConfigurationEntity
    {
        [XmlAttribute(AttributeName = "Name")]
        public string CategoryName { get; set; }

        [XmlAttribute(AttributeName = "Address")]
        public string CategoryAddress { get; set; }

        [XmlAttribute(AttributeName = "CategorySysNo")]
        public int CategorySysNo { get; set; }
    }

    public class Category2or3ConfigurationEntity
    {
        [DataMapping("Name", DbType.String)]
        public string CategoryName { get; set; }

        [DataMapping("SysNo", DbType.Int32)]
        public int CategorySysNo { get; set; }
    }
}
