using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;

namespace ECommerce.Entity.Seckill
{
    public class CountDownInfo
    {
        /// <summary>
        /// 抢购编号 
        /// </summary>
        public int CountDownSysNo { get; set; }

        /// <summary>
        /// 显示优先级
        /// </summary>
        public int ShowPriority { get; set; }

        /// <summary>
        /// 抢购价格
        /// </summary>
        public decimal CountDownPrice { get; set; }

        /// <summary>
        /// 市场价
        /// </summary>
        public decimal MarketPrice { get; set; }

        /// <summary>
        /// 进口关税
        /// </summary>
        public decimal TariffPrice { get; set; }

        /// <summary>
        /// 返还现金
        /// </summary>
        public decimal CountDownCashRebate { get; set; }

        /// <summary>
        /// 赠送积分
        /// </summary>
        public int CountDownPoint { get; set; }

        /// <summary>
        /// 是否包含赠品
        /// </summary>
        public bool IsHaveValidGift { get; set; }

        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 可售库存
        /// </summary>
        public int OnlineQty { get; set; }

        /// <summary>
        /// 抢购商品编号
        /// </summary>
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 抢购商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 商品标题
        /// </summary>
        public string ProductTitle { get; set; }

        /// <summary>
        /// 促销标题
        /// </summary>
        public string PromotionTitle { get; set; }

        /// <summary>
        ///抢购商品默认图片
        /// </summary>
        public string DefaultImage { get; set; }

        /// <summary>
        /// 商品原价
        /// </summary>
        public decimal SnapShotCurrentPrice { get; set; }

        /// <summary>
        /// 商品原返现金额
        /// </summary>
        public decimal SnapShotCashRebate { get; set; }

        /// <summary>
        /// 商品原赠送积分
        /// </summary>
        public decimal SnapShotPoint { get; set; }

        /// <summary>
        /// 商品原进口关税
        /// </summary>
        public decimal SnapShotTariffPrice { get; set; }


        /// <summary>
        /// Gets or sets the type of the product trade.
        /// </summary>
        /// <value>
        /// The type of the product trade.
        /// </value>
        public TradeType ProductTradeType { get; set; }

        /// <summary>
        /// Gets or sets the merchant system no.
        /// </summary>
        /// <value>
        /// The merchant system no.
        /// </value>
        public int MerchantSysNo { get; set; }

        /// <summary>
        /// Gets or sets the name of the merchant brief.
        /// </summary>
        /// <value>
        /// The name of the merchant brief.
        /// </value>
        public string MerchantBriefName { get; set; }


        public decimal RealPrice
        {
            get
            {
                if (TariffPrice > 50)
                {
                    return CountDownPrice + CountDownCashRebate + TariffPrice;
                }
                return CountDownPrice + CountDownCashRebate;
            }
        }
    }
}
