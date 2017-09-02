using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Service.Invoice
{
    public class InvoicePriceService
    {
        /// <summary>
        /// 计算发票金额
        /// </summary>
        /// <param name="cashPay">现金支付金额，即订单商品金额除去优惠券折扣和积分支付折扣的金额</param>
        /// <param name="premiumAmount">保价费(>=0)</param>
        /// <param name="shipPrice">运费(>=0)</param>
        /// <param name="payPrice">手续费(>=0)</param>
        /// <param name="promotionAmount">促销折扣(&lt;=0)</param>
        /// <param name="giftCardPay">礼品卡支付(>=0)</param>
        /// <param name="isPayWhenReceived">是否是货到付款</param>
        /// <returns></returns>
        public static decimal CalculateInvoiceAmount(decimal cashPay, decimal premiumAmount, decimal shipPrice
            , decimal payPrice, decimal promotionAmount, decimal giftCardPay, bool isPayWhenReceived)
        {
            decimal result =
                  cashPay
                + premiumAmount
                + shipPrice
                + payPrice
                + promotionAmount
                - giftCardPay;

            if (isPayWhenReceived)
            {
                result = UnifiedHelper.TruncMoney(result);
            }

            return result;
        }
    }
}
