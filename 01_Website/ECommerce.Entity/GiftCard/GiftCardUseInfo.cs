using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.Order;
using ECommerce.Enums;

namespace ECommerce.Entity.GiftCard
{
    public class GiftCardUseInfo
    {
        /// <summary>
        /// 礼品卡编号
        /// </summary>
        public int TransactionNumber { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 可用金额
        /// </summary>
        public decimal AvailAmount { get; set; }

        /// <summary>
        /// 使用者
        /// </summary>
        public int CustomerSysNo { get; set; }

        /// <summary>
        /// 使用编号
        /// SO SysNo
        /// </summary>
        public int ActionSysNo { get; set; }

        /// <summary>
        /// 使用类型
        /// </summary>
        public string ActionType { get; set; }

        /// <summary>
        /// 礼品卡支付金额
        /// </summary>
        public decimal DeductiblePay { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public SOStatus SOStatus { get; set; }

        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime OrderDate { get; set; }

        public SOAmountInfo Amount { get; set; }

        public decimal TariffAmt { get; set; }

        public decimal PointAmt { get; set; }

        public decimal PromotionAmt { get; set; }

        public decimal PointPay { get; set; }

        /// <summary>
        /// 最后的支付金额
        /// </summary>
        public decimal RealPayAmt
        {
            get
            {
                decimal amt = 0;
                amt = Amount.SOAmt //商品总金额
                          + Amount.ShipPrice //运费
                          //- Math.Abs(PromotionAmt) //优惠金额
                          //- Math.Abs(Amount.PrepayAmt) //余额支付
                          //- Math.Abs(PointPay / decimal.Parse(ConstValue.PointExhangeRate)) //积分支付
                          //- Math.Abs(Amount.GiftCardPay) //礼品卡金额
                          - Math.Abs(Amount.DiscountAmt); //折扣金额
                if (TariffAmt > 50)
                {
                    amt += TariffAmt; //关税
                }
                return amt;
            }
        }


    }
}
