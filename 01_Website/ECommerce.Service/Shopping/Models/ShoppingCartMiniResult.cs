using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Facade.Shopping
{
    /// <summary>
    /// Mini购物车
    /// </summary>
    public class ShoppingCartMiniResult
    {
        /// <summary>
        /// 总商品数量
        /// </summary>
        public int ProductCount { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalAmount { get; set; }
        /// <summary>
        /// 总关税
        /// </summary>
        public decimal TotalTaxFee { get; set; }
        /// <summary>
        /// 是否免关税，True-免
        /// </summary>
        public bool IsFreeTaxFee
        {
            get
            {
                return this.TotalTaxFee > 0m && this.TotalTaxFee <= 50m;
            }
        }
        /// <summary>
        /// 商品列表
        /// </summary>
        public List<ShoppingCartMiniItem> ItemList { get; set; }
    }
    /// <summary>
    /// Mini购物车商品
    /// </summary>
    public class ShoppingCartMiniItem
    {
        /// <summary>
        /// 商品所在的套餐编号，0-不在套餐中
        /// </summary>
        public int PackageSysNo { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public int ProductSysNo { get; set; }
        /// <summary>
        /// 默认图片
        /// </summary>
        public string DefaultImage { get; set; }
        /// <summary>
        /// 关税
        /// </summary>
        public decimal TaxFee { get; set; }
        /// <summary>
        /// 商品标题
        /// </summary>
        public string ProductTitle { get; set; }
        /// <summary>
        /// 商品单价，扣除折扣
        /// </summary>
        public decimal ProductPrice { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }
    }
}
