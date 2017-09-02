using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 收藏商品信息
    /// </summary>
    public class ProductFavorite
    {
        /// <summary>
        /// 获取或设置商品SysNo
        /// </summary>
        public int SysNo { get; set; }
        /// <summary>
        /// 获取或设置商品Name
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductTitle { get; set; }
        /// <summary>
        /// 获取或设置当前价格
        /// </summary>
        public decimal CurrentPrice { get; set; }
        /// <summary>
        /// 返现金额
        /// </summary>
        public decimal CashRebate { get; set; }
        /// <summary>
        /// 获取或设置税率
        /// </summary>
        public decimal TariffRate { get; set; }
        /// <summary>
        /// 获取或设置商品图片
        /// </summary>
        public string DefaultImage { get; set; }

        public int WishSysNo { get; set; }
       
         /// <summary>
        /// 关税价格
        /// </summary>
        public decimal TaxPrice { get { return CurrentPrice * TariffRate; } }

        /// <summary>
        /// 真实价格
        /// </summary>
        public decimal RealPrice
        {

            get
            {
                if (TariffRate * CurrentPrice <= 50)
                {
                    return CurrentPrice + CashRebate;
                }
                else
                {
                    return (CurrentPrice + TaxPrice + CashRebate);
                }
            }
        }

        /// <summary>
        /// 库存
        /// </summary>
        public int OnlineQty { get; set; }
    }
}
