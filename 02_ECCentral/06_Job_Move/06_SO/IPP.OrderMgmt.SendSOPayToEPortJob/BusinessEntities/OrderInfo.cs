using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.SO;
using ECCentral.Service.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SendSOPayToEPortJob.BusinessEntities
{
    public class OrderInfo : EntityBase
    {
        public string MemoForCustomer { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 区
        /// </summary>
        public string DistrictName { get; set; }
        /// <summary>
        /// 购买者姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 购买者身份证
        /// </summary>
        public string IDCardNumber { get; set; }
        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime PayTime { get; set; }
        /// <summary>
        /// 交易流水号
        /// </summary>
        public string PayTransNumber { get; set; }
        /// <summary>
        /// 顾客账号
        /// </summary>
        public string CustomerID { get; set; }
        /// <summary>
        /// 顾客手机号
        /// </summary>
        public string CellPhone { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public int SoSysNo { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public SOStatus Status { get; set; }
        /// <summary>
        /// 顾客系统编号
        /// </summary>
        public int CustomerSysNo { get; set; }
        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime OrderDate { get; set; }
        /// <summary>
        /// 顾客邮箱
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// 订单类型
        /// </summary>
        public SOType SOType { get; set; }
        /// <summary>
        /// 商品总金额
        /// </summary>
        public decimal SoAmt { get; set; }

        public int IsNet { get; set; }

        public int IsPayWhenRecv { get; set; }

        public int IsNetPayed { get; set; }

        public NetPayType NetPayType { get; set; }

        public ShipTypeInfo ShipType { get; set; }

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
                if (Status == SOStatus.Origin
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
                          - Math.Abs(PointPay / decimal.Parse(System.Configuration.ConfigurationManager.AppSettings["Product_PointExhangeRate"])) //积分支付
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
        /// 订单参与的销售活动详细信息。
        /// </summary>
        public List<SOPromotionInfo> SOPromotions
        {
            get;
            set;
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
                if (this.Status == SOStatus.Origin
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
                    return Status == SOStatus.Origin;
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
        /// <summary>
        /// 计量单位（需与商品备案时的单位一致）
        /// </summary>
        public string ApplyUnit { get; set; }
        /// <summary>
        /// 货号（跨境平台商品备案时产生的唯一编码）
        /// </summary>
        public string Product_SKUNO { get; set; }

        public int SOSysNo { get; set; }

        public int ProductSysNo { get; set; }

        public string ProductID { get; set; }

        public string BriefName { get; set; }
        
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
    }

    public enum SOItemType
    {
        /// <summary>
        /// 正常商品
        /// </summary>
        [Description("For Sale")]
        [XmlEnum("0")]
        ForSale = 0,
        /// <summary>
        /// 厂商赠品
        /// </summary>
        [Description("Gift")]
        [XmlEnum("1")]
        Gift = 1,
        /// <summary>
        /// 奖品
        /// </summary>
        [Description("Customer Gift")]
        [XmlEnum("2")]
        AwardItem = 2,
        /// <summary>
        /// 优惠券
        /// </summary>
        [Description("Promotion")]
        [XmlEnum("3")]
        Promotion = 3,
        /// <summary>
        /// 延保
        /// </summary>
        [Description("Warranty")]
        [XmlEnum("4")]
        Warranty = 4,
        /// <summary>
        /// 附件
        /// </summary>
        [Description("Accessory")]
        [XmlEnum("5")]
        HiddenGift = 5,
        /// <summary>
        /// 活动赠品
        /// </summary>
        [Description("NeweggGift")]
        [XmlEnum("6")]
        ActivityGift = 6,
        /// <summary>
        /// 虚拟商品
        /// </summary>
        [Description("VirtualProduct")]
        [XmlEnum("7")]
        VirtualProduct = 7,
    }

    /// <summary>
    /// 分组属性信息
    /// </summary>
    public class GroupPropertyInfo
    {
        #region [ properties ]

        public int PropertySysNo1 { get; set; }


        public string PropertyDescription1 { get; set; }


        public int ValueSysNo1 { get; set; }


        public string ValueDescription1 { get; set; }


        public string IsPolymeric1 { get; set; }


        public int PropertySysNo2 { get; set; }


        public string PropertyDescription2 { get; set; }


        public int ValueSysNo2 { get; set; }


        public string ValueDescription2 { get; set; }


        public string IsPolymeric2 { get; set; }


        public string ProductGroupKeyword { get; set; }

        #endregion
    }

    public class SOLog : EntityBase
    {
        public int SysNo { get; set; }
        public DateTime OptTime { get; set; }
        public int OptUserSysNo { get; set; }
        public string OptIP { get; set; }
        public int OptType { get; set; }
        public int SOSysNo { get; set; }
        public string Note { get; set; }
        public string OptTimeString
        {
            get
            {
                return OptTime.ToString("yyyy年MM月dd日");
            }
        }
        public string TrackingNumber { set; get; }
    }

    public class SOAmountInfo
    {
        /// <summary>
        /// 现金支付总额
        /// </summary>
        public decimal CashPay { get; set; }
        /// <summary>
        /// 折扣总额
        /// </summary>
        public decimal DiscountAmt { get; set; }
        /// <summary>
        /// 礼品卡支付
        /// </summary>
        public decimal GiftCardPay { get; set; }
        /// <summary>
        /// 支付费率
        /// </summary>
        public decimal PayPrice { get; set; }
        /// <summary>
        /// 订单获得积分
        /// </summary>
        public int PointAmt { get; set; }
        /// <summary>
        /// 积分支付总额
        /// </summary>
        public int PointPay { get; set; }
        /// <summary>
        /// 保价费
        /// </summary>
        public decimal PremiumAmt { get; set; }
        /// <summary>
        /// 预付金额
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
                if (decimal.Parse(System.Configuration.ConfigurationManager.AppSettings["Product_PointExhangeRate"]) == 0m)
                {
                    return 0m;
                }
                return Math.Abs(PointPay / decimal.Parse(System.Configuration.ConfigurationManager.AppSettings["Product_PointExhangeRate"]));
            }
        }
    }
}
