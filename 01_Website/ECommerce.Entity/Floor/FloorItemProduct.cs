using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ECommerce.Entity
{
    /// <summary>
    /// 
    /// </summary>
    [XmlRoot("FloorItemProduct")]
    public class FloorItemProduct : FloorItemBase
    {

        public FloorItemProduct() { }

        /// <summary>
        /// 获取或设置产品编号
        /// </summary>
        [XmlElement("ProductSysNo")]
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 获取或设置产品标题
        /// </summary>
        [XmlElement("ProductTitle")]
        public string ProductTitle { get; set; }

        /// <summary>
        /// 获取或设置产品子标题
        /// </summary>
        [XmlElement("ProductSubTitle")]
        public string ProductSubTitle { get; set; }

        /// <summary>
        /// 获取或设置产品价格
        /// </summary>
        [XmlElement("ProductPrice")]
        public decimal? ProductPrice { get; set; }

        /// <summary>
        /// 获取或设置原始价格
        /// </summary>
        [XmlIgnore]
        public decimal? BasicPrice { get; set; }

        /// <summary>
        /// 获取或设置商品当前价格
        /// </summary>
        [XmlIgnore]
        public decimal? RealPrice { get; set; }

        /// <summary>
        /// 返现
        /// </summary>
        public decimal? CashRebate { get; set; }

        /// <summary>
        /// 获取或设置产品折扣信息
        /// </summary>
        [XmlElement("ProductDiscount")]
        public decimal? ProductDiscount { get; set; }

        /// <summary>
        /// 获取或设置产品图片
        /// </summary>
        [XmlElement("DefaultImage")]
        public string DefaultImage { get; set; }


        public override void FillEntityForXMLData()
        {
            base.FillEntityForXMLData();
        }
        /// <summary>
        /// 获取或设置促销语
        /// </summary>
        public string PromotionTitle { get; set; }
    }
}
