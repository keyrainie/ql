using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace IPP.Oversea.CN.ContentMgmt.Baidu.Entities
{
    [XmlRoot(ElementName = "CategoryList")]
    public class CategoryConfigEntity
    {
        [XmlElement(ElementName = "Category")]
        public List<BaiduCategoryEntity> BaiduCategoryList { get; set; }
    }

    public class BaiduCategoryEntity
    {
        [XmlAttribute(AttributeName = "CategoryName")]
        public string CategoryName { get; set; }

        [XmlAttribute(AttributeName = "FirstType")]
        public string FirstType { get; set; }

        [XmlAttribute(AttributeName = "Tag")]
        public string Tag { get; set; }

        [XmlAttribute(AttributeName = "CategorySysNo")]
        public int CategorySysNo { get; set; }
    }
}
