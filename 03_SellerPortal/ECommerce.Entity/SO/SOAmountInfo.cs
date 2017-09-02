using System;

namespace ECommerce.Entity.SO
{
    public class SOAmountInfo
    {
        /// <summary>
        /// 现金支付
        /// </summary>
        public decimal CashPay { get; set; }

        /// <summary>
        /// 折扣金额
        /// </summary>
        public decimal DiscountAmt { get; set; }

        /// <summary>
        /// 礼品卡支付
        /// </summary>
        public decimal GiftCardPay { get; set; }

        public decimal PayPrice { get; set; }


        public int PointAmt { get; set; }

        public int PointPay { get; set; }

        public decimal PremiumAmt { get; set; }

        /// <summary>
        /// 余额支付
        /// </summary>
        public decimal PrepayAmt { get; set; }

        /// <summary>
        /// 商品总金额
        /// </summary>
        public decimal SOAmt { get; set; }

        /// <summary>
        /// 运费
        /// </summary>
        public decimal ShipPrice { get; set; }

        /// <summary>
        /// 积分抵扣(PointPay/ConstValue.PointExhangeRate)
        /// </summary>
        public decimal PointPayAmt
        {
            get
            {
                if (PointExhangeRate == 0m)
                {
                    return 0m;
                }
                return Math.Abs(PointPay / PointExhangeRate);
            }
        }

        public decimal PointExhangeRate
        {
            get
            {
                return (decimal)100;
            }
            set { throw new NotImplementedException(); }
        }
    }
}
