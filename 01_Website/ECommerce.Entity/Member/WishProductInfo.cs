using ECommerce.Entity.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Member
{
    public class WishProductInfo : ProductCellInfo
    {
        public DateTime WishDate { get; set; }
        public int ProductSysNo { get; set; }
        public string ProductID { get; set; }
        //public string ProductName { get; set; }
        // public string DefaultImage { get; set; }
        // public string ProductTitle { get; set; }
        public decimal CurrentPrice { get; set; }
        //public int ProductStatus { get; set; }
        public int QuantityForSale { get; set; }
        public decimal CashRebate { get; set; }
        public int PointType { get; set; }
        //public string ImageVersion { get; set; }

        /// <summary>
        /// 市场售价
        /// </summary>
        public decimal MarketPrice { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        public decimal TariffRate { get; set; }

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
                if (TaxPrice <= 50)
                {
                    return CurrentPrice + CashRebate;
                }
                else
                {
                    return (CurrentPrice + TaxPrice + CashRebate);
                }
            }
        }
    }
}
