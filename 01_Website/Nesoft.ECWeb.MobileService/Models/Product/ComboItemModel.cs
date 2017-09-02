using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Product
{
    public class ComboItemModel
    {
        public int ID { get; set; }

        public string Code { get; set; }

        /// <summary>
        /// 商品标题
        /// </summary>
        public string ProductTitle { get; set; }

        /// <summary>
        /// 一个套餐内的数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 商品图片
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// 卖价
        /// </summary>
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// 关税
        /// </summary>
        public decimal TariffPrice { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>
        public decimal Discount { get; set; }
    }
}