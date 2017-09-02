using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.SearchEngine
{
    public class ProductSearchResultItem
    {
        /// <summary>
        /// 商品系统编号
        /// </summary>
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }
        
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 商品展示名称，如果是聚合商品，商品展示名称为商品组名+聚合属性名
        /// </summary>
        public string ProductDisplayName { get; set; }

        /// <summary>
        /// 聚合商品数量，大于1表示同个属性下有多个聚合商品
        /// </summary>
        public int PolymericProductCount { get; set; }

        /// <summary>
        /// 商品组名称
        /// </summary>
        public string ProductGroupName { get; set; }

        /// <summary>
        /// 前台商品3级类别编号
        /// </summary>
        public int ProductCategoryID { get; set; }

        /// <summary>
        /// 商品商家编号
        /// </summary>
        public int MerchantSysNo { get; set; }

        /// <summary>
        /// 商品商家名称
        /// </summary>
        public string MerchantBriefName { get; set; }

        /// <summary>
        /// 时效促销语
        /// </summary>
        public string PromotionTitle { get; set; }

        /// <summary>
        /// 商品简要介绍
        /// </summary>
        public string ProductShortDescription { get; set; }

        /// <summary>
        /// 商品默认图片
        /// </summary>
        public string ProductDefaultImage { get; set; }

        /// <summary>
        /// 进口税(不加CashRebate)
        /// </summary>
        public decimal ProductTariffAmt { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        public decimal TariffRate { get; set; }

        /// <summary>
        /// 进口税(加了CashRebate)
        /// </summary>
        //public decimal ProductTariffAmtWithRebate { get; set; }

        /// <summary>
        /// 商品可售库存
        /// </summary>
        public int OnlineQty { get; set; }

        /// <summary>
        /// 商品销售价
        /// </summary>
        public decimal SalesPrice { get; set; }

        /// <summary>
        /// 市场价
        /// </summary>
        public decimal MarketPrice { get; set; }

        /// <summary>
        /// 购买商品送积分
        /// </summary>
        public int Point { get; set; }

        /// <summary>
        /// 商品返现
        /// </summary>
        public decimal CashRebate { get; set; }

        /// <summary>
        /// 是否包含赠品
        /// </summary>
        public bool IsHaveValidGift { get; set; }

        /// <summary>
        /// 是否是限时促销商品
        /// </summary>
        public bool IsCountDown { get; set; }

        /// <summary>
        /// 商品总价
        /// </summary>
        public decimal TotalPrice { get; set; }


        /// <summary>
        /// 评论分数
        /// </summary>
        public decimal ReviewScore { get; set; }

        /// <summary>
        /// 评论数量
        /// </summary>
        public int ReviewCount { get; set; }

        /// <summary>
        /// 是否是新上架商品
        /// </summary>
        public bool IsNewproduct { get; set; }

        /// <summary>
        /// 是否是团购商品
        /// </summary>
        public bool IsGroupBuyProduct { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>
        public decimal Discount { get; set; }

        /// <summary>
        /// 商品贸易类型（直邮、自贸、其他）
        /// </summary>
        public TradeType ProductTradeType { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public ProductStatus Status { get; set; }

        public decimal RealPrice
        {

            get
            {
                if (ProductTariffAmt <= 50)
                {
                    return SalesPrice + CashRebate;
                }
                else
                {
                    return SalesPrice + ProductTariffAmt + CashRebate;
                }
            }
        }

    }
}
