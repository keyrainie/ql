using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Category;
using ECommerce.Enums;

namespace ECommerce.Entity.Product
{
    public class ProductCompareInfo
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public int ProductSysNo { get; set; }
        /// <summary>
        /// 商品Code
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// 商品类别ID
        /// </summary>
        public int CategoryID { get; set; }

        public string CategoryName { get; set; }

        public int ECCategoryID { get; set; }
        public string ProductMode { get; set; }
        public string ProductName { get; set; }
        public string ProductDesc { get; set; }
        public string Performance { get; set; }
        public string Warranty { get; set; }
        public string Weight { get; set; }
        public string Attention { get; set; }
        public string PackageList { get; set; }

        public int RemarkCount { get;set; }
        public decimal RemarkScore { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ProductStatus ProductStatus { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public ProductType ProductType { get; set; }
        /// <summary>
        ///默认图片
        /// </summary>
        public string DefaultImage { get; set; }
        /// <summary>
        /// 促销标题
        /// </summary>
        public string PromotionTitle { get; set; }
        /// <summary>
        /// 商品标题
        /// </summary>
        public string ProductTitle { get; set; }

        /// <summary>
        /// 当前销售价
        /// </summary>
        public decimal CurrentPrice { get; set; }

        /// <summary>
        /// 返现
        /// </summary>
        public decimal CashRebate { get; set; }

        /// <summary>
        /// 市场价
        /// </summary>
        public decimal BasicPrice { get; set; }

        public int OnlineQty { get; set; }

        public int HostWarrantyDay { get; set; }
        public int PartWarrantyDay { get; set; }
        public string WarrantyDetail { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        public decimal TariffRate { get; set; }

        public decimal TotalPirce {

            get {
                if (this.CurrentPrice * this.TariffRate <= 50)
                {

                    return this.CurrentPrice + CashRebate;
                }
                else
                {
                    return this.CurrentPrice + CashRebate + this.CurrentPrice * this.TariffRate;
                }
            }
        }


    }
}
