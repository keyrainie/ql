using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Product
{
    public class GiftCardProductInfo
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public int SysNo { get; set; }
        /// <summary>
        /// 商品Code
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 礼品卡标题
        /// </summary>
        public string ProductTitle { get; set; }

        /// <summary>
        /// 商品默认图片
        /// </summary>
        public string DefaultImage { get; set; }

        /// <summary>
        /// 礼品卡面值
        /// </summary>
        public decimal CurrentPrice { get; set; }
    }
}
