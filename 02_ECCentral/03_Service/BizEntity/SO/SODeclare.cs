using ECCentral.BizEntity.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECCentral.BizEntity.SO
{
    /// <summary>
    /// 订单申报记录
    /// </summary>
    public class SODeclareRecords
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public int SOSysNo { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>
        public string TrackingNumber { get; set; }

        /// <summary>
        /// 状态：0-已提交申报；10-申报成功；-10-申报失败
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime InDate { get; set; }

        /// <summary>
        /// 编辑时间
        /// </summary>
        public DateTime? EditDate { get; set; }
    }

    /// <summary>
    /// 订单申报记录
    /// </summary>
    public class WaitDeclareSO
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public int SOSysNo { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>
        public string TrackingNumber { get; set; }
    }

    /// <summary>
    /// 支付订单信息
    /// </summary>
    public class DeclareOrderInfo
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

        public DeclareSOAmountInfo Amount { get; set; }

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
                          - Math.Abs(PromotionAmt) //优惠金额
                          - Math.Abs(Amount.PrepayAmt) //余额支付
                          - Math.Abs(PointPay / 100.00m) //积分支付
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

        public List<DeclareSOItemInfo> SOItemList { get; set; }

        public DeclareSOPayInfo PayInfo { get; set; }
        /// <summary>
        /// 海关关区代码
        /// </summary>
        public string CustomsCode { get; set; }
        /// <summary>
        /// 仓库类型
        /// </summary>
        public TradeType StockType { get; set; }
        /// <summary>
        /// 支付方式系统编号
        /// </summary>
        public int PayTypeSysNo { get; set; }
        /// <summary>
        /// 支付方式编号
        /// </summary>
        public string PayTypeID { get; set; }

        #region 发件信息
        public string senderName { get; set; }
        public string senderTel { get; set; }
        public string senderCompanyName { get; set; }
        public string senderAddr { get; set; }
        public string senderZip { get; set; }
        public string senderCity { get; set; }
        public string senderProvince { get; set; }
        public string senderCountry { get; set; }
        #endregion
    }
    /// <summary>
    /// 支付订单商品
    /// </summary>
    public class DeclareSOItemInfo
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

        public string DefaultImage { get; set; }

        public int GiftID { get; set; }

        public decimal PromotionDiscount { get; set; }

        public decimal TariffPrice
        {
            get { return this.TariffAmt; }
        }

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
    /// <summary>
    /// 金额信息
    /// </summary>
    public class DeclareSOAmountInfo
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
                return Math.Abs(PointPay / 100.00m);
            }
        }
    }
    /// <summary>
    /// 订单支付平台信息
    /// </summary>
    public class DeclareSOPayInfo
    {
        /// <summary>
        /// 支付平台流水号
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// 支付平台处理时间
        /// </summary>
        public string PayProcessTime { get; set; }
    }
}
