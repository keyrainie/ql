using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Order
{
    public class SOAmountInfo
    {
        public decimal CashPay { get; set; }

        public decimal DiscountAmt { get; set; }

        public decimal GiftCardPay { get; set; }

        public decimal PayPrice { get; set; }

        public int PointAmt { get; set; }

        public int PointPay { get; set; }

        public decimal PremiumAmt { get; set; }

        public decimal PrepayAmt { get; set; }

        public decimal SOAmt { get; set; }
        public decimal ShipPrice { get; set; }

        /// <summary>
        /// 积分抵扣(PointPay/ConstValue.PointExhangeRate)
        /// </summary>
        public decimal PointPayAmt
        {
            get
            {
                if (decimal.Parse(ConstValue.PointExhangeRate) == 0m)
                {
                    return 0m;
                }
                return Math.Abs(PointPay / decimal.Parse(ConstValue.PointExhangeRate));
            }
        }
    }
}
