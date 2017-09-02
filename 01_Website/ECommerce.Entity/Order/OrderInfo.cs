using ECommerce.Entity.Product;
using ECommerce.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Utility;
using ECommerce.Utility.DataAccess.SearchEngine;
using ECommerce.Entity.GiftCard;

namespace ECommerce.Entity.Order
{
    public class OrderInfo : EntityBase
    {
        public string MemoForCustomer { get; set; }
        public int SoSysNo { get; set; }
        public SOStatus Status { get; set; }
        public int CustomerSysNo { get; set; }
        public DateTime OrderDate { get; set; }

        public SOType SOType { get; set; }

        public decimal SoAmt { get; set; }

        public int IsNet { get; set; }

        public int IsPayWhenRecv { get; set; }

        public int IsNetPayed { get; set; }

        public NetPayType NetPayType { get; set; }

        public Shipping.ShipTypeInfo ShipType { get; set; }

        public PaymentInfo Payment { get; set; }

        public DateTime DeliveryDate { get; set; }

        public DateTime OutTime { get; set; }

        public int DeliveryTimeRange { get; set; }

        public string Memo { get; set; }

        public string DeliverySection { get; set; }

        public string ReceiveAreaSysNo { get; set; }

        public string ReceiveAreaName { get; set; }

        public string ReceiveContact { get; set; }

        public string ReceiveName { get; set; }

        public string ReceivePhone { get; set; }

        public string ReceiveCellPhone { get; set; }

        public string ReceiveAddress { get; set; }

        public string ReceiveZip { get; set; }

        public List<SOItemInfo> SOItemList { get; set; }
        public List<SOLog> SOLogList { get; set; }

        public List<GiftCardRedeemLog> GiftCardRedeemLogList { get; set; }

        public SOAmountInfo Amount { get; set; }

        public decimal TariffAmt { get; set; }

        public decimal PointAmt { get; set; }

        public decimal BankPointPayAmount { get; set; }

        public decimal PromotionAmt { get; set; }

        /// <summary>
        /// 是否显示netpay
        /// </summary>
        public bool IsNetPay
        {
            get
            {
                var result = false;
                if (Status == SOStatus.Original
                        || Status == SOStatus.WaitingManagerAudit
                        || Status == SOStatus.WaitingPay)
                {
                    if (IsNetPayed != 1
                        && IsPayWhenRecv != 1
                        && IsNet == 1)
                    {
                        result = true;
                    }
                }
                return result;
            }
        }

        public decimal PointPay { get; set; }

        public string StatusText
        {
            get
            {
                if (Status == SOStatus.Original)
                {
                    if (IsPayWhenRecv == 0)
                    {
                        if (IsNetPayed == 1)
                        {
                            return "已支付";
                        }
                        else
                        {
                            return "未支付";
                        }
                    }
                    else
                    {
                        return EnumHelper.GetDescription(Status);
                    }
                }
                else
                {
                    if (this.SOType == Enums.SOType.PhysicalCard)
                    {
                        if ((int)Status < (int)Enums.SOStatus.OutStock)
                        {
                            return "待出库";
                        }
                        else if (Status == Enums.SOStatus.OutStock)
                        {
                            return "已出库";
                        }
                        else
                        {
                            return "已完成";
                        }
                    }
                    else
                    {
                        return EnumHelper.GetDescription(Status);
                    }
                }
            }
        }

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
                          - Math.Abs(PromotionAmt) //优惠金额
                          - Math.Abs(Amount.PrepayAmt) //余额支付
                          - Math.Abs(PointPay / decimal.Parse(ConstValue.PointExhangeRate)) //积分支付
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
        /// 订单总金额
        /// </summary>
        public decimal SOTotalAmt
        {
            get
            {
                return RealPayAmt + Math.Abs(Amount.PrepayAmt); //余额支付
            }
        }

        public bool IsShowPay
        {
            get
            {
                var result = false;
                if (this.Status == SOStatus.Original
                        || this.Status == SOStatus.WaitingManagerAudit
                        || this.Status == SOStatus.WaitingPay)
                {
                    if (this.IsNetPayed != 1
                        && this.IsPayWhenRecv != 1
                        && this.IsNet == 1)
                    {
                        result = true;
                    }
                }
                return result;
            }
        }

        public bool IsShowVoid
        {
            get
            {
                //后台锁单，前台不允许再操作
                if (HoldMark)
                {
                    return false;
                }
                if (IsPayWhenRecv == 1)
                {
                    return Status == SOStatus.Original;
                }
                return IsNetPay;
            }
        }

        public bool HoldMark { get; set; }

        public int SellerSysNo { get; set; }

        public string SellerName { get; set; }

        public int ShoppingCartID { get; set; }

        /// <summary>
        /// 发货仓库编号
        /// </summary>
        public int StockSysNo { get; set; }

        /// <summary>
        /// 订单所属商家
        /// </summary>
        public int MerchantSysNo { set; get; }
    }

    public class SOItemInfo
    {
        public int SOSysNo { get; set; }

        public int ProductSysNo { get; set; }

        public string ProductID { get; set; }

        public string ProductName { get; set; }

        public string ProductTitle { get; set; }

        public int Quantity { get; set; }

        public int Weight { get; set; }

        public decimal Price { get; set; }

        public decimal Cost { get; set; }

        public int Point { get; set; }

        public int PointType { get; set; }

        public decimal DiscountAmt { get; set; }

        public decimal OriginalPrice { get; set; }

        public string Warranty { get; set; }

        public SOItemType ProductType { get; set; }

        public string DefaultImage { get; set; }

        public int GiftID { get; set; }

        public GroupPropertyInfo GroupPropertyInfo { get; set; }

        //public decimal TariffRate { get; set; }
        public decimal PromotionDiscount { get; set; }
        public decimal TariffAmt { get; set; }
        public decimal TariffPrice
        {
            get { return TariffAmt; }
        }
        /// <summary>
        /// 订单所属商家
        /// </summary>
        public int MerchantSysNo { set; get; }
    }

    /// <summary>
    /// 支付订单信息
    /// </summary>
    public class PayOrderInfo : OrderInfo
    {
        public string ReceiveProvinceName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.ReceiveAreaName)
                    && this.ReceiveAreaName.Split(' ').Length > 0)
                    return this.ReceiveAreaName.Split(' ')[0];
                return "";
            }
        }
        public string ReceiveCityName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.ReceiveAreaName)
                    && this.ReceiveAreaName.Split(' ').Length > 1)
                    return this.ReceiveAreaName.Split(' ')[1];
                return "";
            }
        }
        public string ReceiveDistrictName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.ReceiveAreaName)
                    && this.ReceiveAreaName.Split(' ').Length > 2)
                    return this.ReceiveAreaName.Split(' ')[2];
                return "";
            }
        }

        /// <summary>
        /// 货币类型
        /// </summary>
        public int CurrencySysNo { get; set; }
        /// <summary>
        /// 海关关区代码
        /// </summary>
        public string CustomsCode { get; set; }

        public List<PaySOItemInfo> SOItemList { get; set; }
    }
    /// <summary>
    /// 支付订单商品
    /// </summary>
    public class PaySOItemInfo : SOItemInfo
    {
        /// <summary>
        /// 仓库编号WarehouseName
        /// </summary>
        public string WarehouseNumber { get; set; }

        public string BriefName { get; set; }

        /// <summary>
        /// 商品备案号
        /// </summary>
        public string EntryCode { get; set; }

        /// <summary>
        /// 商品税则号
        /// </summary>
        public string TariffCode { get; set; }

        /// <summary>
        /// 单个商品的税费
        /// </summary>
        public decimal TariffAmt { get; set; }

        /// <summary>
        /// 仓库国家代码
        /// </summary>
        public string CountryCode { get; set; }
    }
}
