using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nesoft.ECWeb.MobileService.Models.Product
{
    public class ComboInfoModel
    {
        /// <summary>
        /// 套餐活动系统编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 套餐活动名称
        /// </summary>
        public string SaleRuleName { get; set; }

        /// <summary>
        /// 原价
        /// </summary>
        public decimal OriginalPrice { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>
        public decimal DiscountPrice { get; set; }

        /// <summary>
        /// 总卖价
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 套餐内商品列表
        /// </summary>
        public List<ComboItemModel> Items { get; set; }
    }
}