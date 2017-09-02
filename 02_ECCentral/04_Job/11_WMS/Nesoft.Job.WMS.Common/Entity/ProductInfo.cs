using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nesoft.Job.WMS.Common.Entity
{
    public class ProductInfo : BaseEntity
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        [JsonProperty("itemCode")]
        public string ItemCode { get; set; }
        /// <summary>
        /// 货号
        /// </summary>
        [JsonProperty("goodsNo")]
        public string GoodsNo { get; set; }
        /// <summary>
        /// 商品中文名称
        /// </summary>
        [JsonProperty("itemNameCN")]
        public string ItemNameCN { get; set; }
        /// <summary>
        /// 商品英文名称
        /// </summary>
        [JsonProperty("itemNameEN")]
        public string ItemNameEN { get; set; }
        /// <summary>
        /// 商品型号
        /// </summary>
        [JsonProperty("itemModel")]
        public string ItemModel { get; set; }
        /// <summary>
        /// 商品规格
        /// </summary>
        [JsonProperty("itemSpec")]
        public string ItemSpec { get; set; }
        /// <summary>
        /// 商品产地
        /// </summary>
        [JsonProperty("itemCountry")]
        public string ItemCountry { get; set; }
        /// <summary>
        /// 商品计税单位
        /// </summary>
        [JsonProperty("itemUnit")]
        public string ItemUnit { get; set; }
        /// <summary>
        /// 商品计税数量
        /// </summary>
        [JsonProperty("itemQty")]
        public string ItemQty { get; set; }
        /// <summary>
        /// 商品单价
        /// </summary>
        [JsonIgnore]
        public decimal ItemPrice { get; set; }
        /// <summary>
        /// 商品单价,传入数据中心用此字段
        /// </summary>
        [JsonProperty("itemPrice")]
        public string ItemPriceP {
            get
            {
                return this.ItemPrice.ToString("#0.00");
            }
        }
        /// <summary>
        /// 商品出厂日期
        /// </summary>
        [JsonIgnore]
        public DateTime ProductFactoryDate { get; set; }
        /// <summary>
        /// 商品出厂日期,传入数据中心用此字段
        /// </summary>
        [JsonProperty("productFactoryDate")]
        public string ProductFactoryDateP
        {
            get
            {
                return this.ProductFactoryDate.ToString("yyyy-MM-dd");
            }
        }
    }
}
