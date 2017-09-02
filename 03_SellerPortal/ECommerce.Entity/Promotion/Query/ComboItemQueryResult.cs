using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;
using ECommerce.Utility;

namespace ECommerce.Entity.Promotion
{
    public class ComboItemQueryResult:ComboItem
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品标题
        /// </summary>
        public string ProductTitle { get; set; }

        /// <summary>
        /// 商品状态
        /// </summary>
        public ProductStatus Status { get; set; }

        /// <summary>
        /// 商品卖价
        /// </summary>
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// 商品状态
        /// </summary>
        public string StatusString
        {
            get
            {
                return EnumHelper.GetDescription(this.Status);
            }
        }
    }
}
