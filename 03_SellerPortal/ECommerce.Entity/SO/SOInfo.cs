using System;
using System.Collections.Generic;
using ECommerce.Entity.Order;
using ECommerce.Entity.Common;
using ECommerce.Entity.Store.Vendor;
using ECommerce.Enums;
using ECommerce.Utility;
using ECommerce.WebFramework;

namespace ECommerce.Entity.SO
{
    public class SOInfo : EntityBase
    {
        public string MemoForCustomer { get; set; }
        public int SOSysNo { get; set; }
        public SOStatus Status { get; set; }
        public int CustomerSysNo { get; set; }
        public string CustomerID { get; set; }
        public DateTime? OrderDate { get; set; }

        //public SOType SOType { get; set; }

        public decimal SoAmt { get; set; }

        public int IsNet { get; set; }

        public int IsPayWhenRecv { get; set; }

        public int IsNetPayed { get; set; }

        public NetPayType NetPayType { get; set; }

        public ShipTypeInfo ShipType { get; set; }

        public PaymentInfo Payment { get; set; }

        public NetPayStatusType? NetPayStatus { get; set; }

        public SOIncomeStatus? SOIncomeStatus { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public DateTime? OutTime { get; set; }

        public int DeliveryTimeRange { get; set; }

        public string Memo { get; set; }

        public string DeliverySection { get; set; }

        public int? ReceiveAreaSysNo { get; set; }

        public AreaInfo ReceiveArea { get; set; }

        public string ReceiveContact { get; set; }

        public string ReceiveName { get; set; }

        public string ReceivePhone { get; set; }

        public string ReceiveCellPhone { get; set; }

        public string ReceiveAddress { get; set; }

        public string ReceiveZip { get; set; }

        public DateTime? AuditTime { get; set; }

        //public SOPaymentStatus PaymentStatus { get; set; }

        public List<SOItemInfo> SOItemList { get; set; }
        //public List<SOLog> SOLogList { get; set; }

        public SOAmountInfo Amount { get; set; }

        /// <summary>
        /// 订单总重量
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        ///     关税
        /// </summary>
        public decimal TariffAmt { get; set; }

        /// <summary>
        ///     花费的积分数
        /// </summary>
        public decimal PointAmt { get; set; }

        /// <summary>
        ///     优惠金额
        /// </summary>
        public decimal PromotionAmt { get; set; }

        public decimal PointPay { get; set; }

        public string StatusText
        {
            get
            {
                //if (Status == SOStatus.Original)
                //{
                //    if (IsNetPayed == 1)
                //    {
                //        return "已支付";
                //    }
                //    else
                //    {
                //        return "未支付";
                //    }
                //}
                //else
                //{
                return Status.GetDescription();
                //}
            }
        }

        public string PaymentStatusText
        {
            get
            {
                if (SOIncomeStatus == null || SOIncomeStatus == Enums.SOIncomeStatus.Abandon)
                {
                    if (NetPayStatus != null && NetPayStatus == NetPayStatusType.Origin)
                    {
                        return LanguageHelper.GetText("已支付未审核");
                    }
                    return LanguageHelper.GetText("未支付");
                }
                return SOIncomeStatus.Value.GetDescription();
            }
        }

        /// <summary>
        ///     最后的支付金额
        /// </summary>
        public decimal RealPayAmt
        {
            get
            {
                decimal amt = 0;
                amt = Amount.SOAmt //商品总金额
                      + Amount.ShipPrice //运费
                      - Math.Abs(PromotionAmt) //优惠金额
                      - Math.Abs(Amount.PrepayAmt) //余额支付
                      - Math.Abs(Amount.PointPayAmt) //积分支付
                      - Math.Abs(Amount.GiftCardPay) //礼品卡金额
                      - Math.Abs(Amount.DiscountAmt); //折扣金额
                if (TariffAmt > 50)
                {
                    amt += TariffAmt; //关税
                }
                return amt;
            }
        }
        /// <summary>
        /// 应收款
        /// </summary>
        public decimal OriginalReceivableAmount
        {
            get
            {
                decimal amount = RealPayAmt
                    - Math.Abs(Amount.PrepayAmt)
                    - Math.Abs(Amount.GiftCardPay);
                return amount >= 0 ? amount : 0.00M;
            }
        }

        /// <summary>
        /// 应收款(>=0):SOAmount - PointPayAmount - CouponAmount + PayPrice + ShipPrice + PremiumAmount - DiscountAmount-PrepayAmount-GiftCardPay。
        /// 如果订单是货到付款,则是去“分”后的金额。如果要用原始的未去分的金额请使用OriginalReceivableAmount。
        /// </summary> 
        public decimal ReceivableAmount
        {
            get
            {
                decimal amount = OriginalReceivableAmount;
                if (IsPayWhenRecv == 1)
                {
                    amount = (int)(amount * 10) / 10M;
                }
                return amount >= 0M ? Math.Round(amount, 2) : 0.00M;
            }
        }

        /// <summary>
        ///     订单总金额
        /// </summary>
        public decimal SOTotalAmt
        {
            get
            {
                return RealPayAmt + Math.Abs(Amount.PrepayAmt); //余额支付
            }
        }

        public int? MerchantSysNo { get; set; }

        public string StoreCompanyCode { get; set; }

        public int? StockSysNo { get; set; }

        public bool? IsCombine { get; set; }

        public VendorInvoiceType? InvoiceType { get; set; }
        public ShippingStockType? StockType { get; set; }
        public VendorShippingType? ShippingType { get; set; }

        public string TrackingNumber { get; set; }
    } // end class
} // end nampespace