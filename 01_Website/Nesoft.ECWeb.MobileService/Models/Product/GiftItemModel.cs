using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Product
{
    /// <summary>
    /// 赠品
    /// </summary>
    public class GiftItemModel
    {
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int UnitQuantity { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        public string ImageUrl { get; set; }
    }
}