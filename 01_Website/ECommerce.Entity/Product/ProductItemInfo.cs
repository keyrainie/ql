using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Category;
using ECommerce.Enums;

namespace ECommerce.Entity.Product
{
    public class ProductItemInfo
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 商品Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 商品类别ID
        /// </summary>
        public string CategoryID { get; set; }

        /// <summary>
        /// 前台商品类别Code
        /// </summary>
        public string CategoryCode { get; set; }

        /// <summary>
        /// 商品类别名称
        /// </summary>
        public string CategoryName { get; set; }

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
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 商品型号
        /// </summary>
        public string ProductMode { get; set; }
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
        public decimal MarketPrice { get; set; }

        /// <summary>
        /// 支付类型
        /// </summary>
        public ProductPayType PointType { get; set; }
        /// <summary>
        /// 赠送积分
        /// </summary>
        public int Point { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        public decimal TariffRate { get; set; }


        public int DisplayQuantity { get; set; }

        public int Status { get; set; }

        /// <summary>
        ///  品牌
        /// </summary>
        public int BrandSysNo { get; set; }


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
                    return CurrentPrice + (CurrentPrice * TariffRate) + CashRebate;
                }
            }
        }

        /// <summary>
        /// 商品评分
        /// </summary>
        public decimal AvgScore { get; set; }

        /// <summary>
        /// 评分人数
        /// </summary>
        public int ReviewCount { get; set; }

        /// <summary>
        /// 是否包含赠品
        /// </summary>
        public int IsHaveValidGift { get; set; }
    }
}
