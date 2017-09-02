using System;
using System.Collections.Generic;
using ECCentral.BizEntity.Invoice;
using System.Runtime.Serialization;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.PO;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.MKT;

namespace ECCentral.BizEntity.SO
{
    /// <summary>
    /// 订单信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class SOInfo : IIdentity, ICompany, IWebChannel, IMarketingPlace
    {
        #region 接口字段

        private int? sysNo;

        /// <summary>
        /// 订单系统编号
        /// </summary>
        [DataMember]
        public int? SysNo
        {
            get { return sysNo; }
            set
            {
                this.sysNo = value;
                if (this.BaseInfo != null) { this.BaseInfo.SysNo = value; this.BaseInfo.SOID = value.ToString(); }
                if (this.FPInfo != null) this.FPInfo.SOSysNo = value;
                if (this.InvoiceInfo != null) this.InvoiceInfo.SOSysNo = value;
                if (this.ReceiverInfo != null) this.ReceiverInfo.SOSysNo = value;
                if (this.ShippingInfo != null) this.ShippingInfo.SOSysNo = value;
                if (this.Items != null && this.Items.Count > 0)
                {
                    foreach (var item in this.Items)
                    {
                        item.SOSysNo = value;
                    }
                }
            }
        }

        private string companyCode;
        [DataMember]
        public string CompanyCode
        {
            get { return companyCode; }
            set
            {
                this.companyCode = value;
                if (this.BaseInfo != null) this.BaseInfo.CompanyCode = value;
            }
        }

        private Common.WebChannel webChannel;
        [DataMember]
        public Common.WebChannel WebChannel
        {
            get { return webChannel; }
            set
            {
                webChannel = value;
            }
        }

        private Common.Merchant merchant;
        [DataMember]
        public Common.Merchant Merchant
        {
            get { return merchant; }
            set
            {
                merchant = value;
                if (this.BaseInfo != null) this.BaseInfo.Merchant = value;
            }
        }
        #endregion

        public SOInfo()
        {
            BaseInfo = new SOBaseInfo();
            StatusChangeInfoList = new List<SOStatusChangeInfo>();
            ReceiverInfo = new SOReceiverInfo();
            InvoiceInfo = new SOInvoiceInfo();
            ShippingInfo = new SOShippingInfo();
            FPInfo = new SOFPInfo();
            ClientInfo = new SOClientInfo();
            Items = new List<SOItemInfo>();
            SOPromotions = new List<SOPromotionInfo>();
            SOGiftCardList = new List<IM.GiftCardRedeemLog>();
            ItemGrossProfitList = new List<ItemGrossProfitInfo>();
            SOInterceptInfoList = new List<SOInterceptInfo>();
        }

        [DataMember]
        public SOBaseInfo BaseInfo { get; set; }

        /// <summary>
        /// 订单状态更改记录
        /// </summary>
        [DataMember]
        public List<SOStatusChangeInfo> StatusChangeInfoList { get; set; }

        /// <summary>
        /// 订单收货人相关信息
        /// </summary>
        [DataMember]
        public SOReceiverInfo ReceiverInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 发票信息
        /// </summary>
        [DataMember]
        public SOInvoiceInfo InvoiceInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 配送信息
        /// </summary>
        [DataMember]
        public SOShippingInfo ShippingInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 订单欺诈验证信息
        /// </summary>
        [DataMember]
        public SOFPInfo FPInfo { get; set; }

        /// <summary>
        /// 订单客户端信息
        /// </summary>
        [DataMember]
        public SOClientInfo ClientInfo { get; set; }

        /// <summary>
        /// 订单商品信息
        /// </summary>
        [DataMember]
        public List<SOItemInfo> Items
        {
            get;
            set;
        }

        /// <summary>
        /// 订单参与的销售活动详细信息。
        /// </summary>
        [DataMember]
        public List<SOPromotionInfo> SOPromotions
        {
            get;
            set;
        }

        /// <summary>
        /// 订单使用礼品卡列表。
        /// </summary>
        [DataMember]
        public List<ECCentral.BizEntity.IM.GiftCardRedeemLog> SOGiftCardList
        {
            get;
            set;
        }
        /// <summary>
        /// 订单毛利
        /// </summary>
        [DataMember]
        public List<ItemGrossProfitInfo> ItemGrossProfitList
        {
            get;
            set;
        }

        [DataMember]
        public List<SOInterceptInfo> SOInterceptInfoList
        {
            get;
            set;
        }

        #region  下面的属性仅作为数据传输或临时的存储用。

        /// <summary>
        /// 优惠券代码。
        /// </summary>
        [DataMember]
        public string CouponCode { get; set; }

        public string UnionParams { get; set; }

        [DataMember]
        [Obsolete("此字段已弃用", true)]
        public int? EnergySysNo { get; set; }

        #endregion

        #region 可不要属性

        /*
        #region 优惠券作为商品存放在Item中

        /// <summary>
        /// 优惠券所属用户编号
        /// </summary>
        public string PromotionCustomerSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 应用优惠券的ID
        /// </summary>
        public int? PromotionCodeSysNo
        {
            get;
            set;
        }

        #endregion 优惠券作为商品存放在Item中

        /// <summary>
        ///
        /// </summary>
        public int? PositiveSOSysNo // 是否有用？
        { get; set; }

        /// <summary>
        /// 后台是否锁定
        /// </summary>
        public bool? HoldMark //订单扩展信息中的 HoldStatus 来判断是不是后台锁单。
        { get; set; }

        /// <summary>
        /// 订单锁定时间
        /// </summary>
        public DateTime? HoldDate //订单扩展信息中有此属性
        { get; set; }
        ////*/

        #region 可以不要  //SO_CheckShipping
        /*////
        /// <summary>
        ///  
        /// </summary>
        public int? IsDirectAlipay //不知道是什么，好像没有用到
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public decimal? TenpayCoupon //不知道是什么，好像没有用到
        {
            get;
            set;
        }

        /// <summary>
        /// 订单创建时间
        /// </summary>
        public DateTime? CreateTime //同OrderTime
        {
            get;
            set;
        }
        /// <summary>
        /// MKT活动类型
        /// </summary>
        public int? MKTActivityType //是否需要，如果 一个订单参与多个MKT活动，此值表示什么，更应该与Item相关
        {
            get;
            set;
        }

        /// <summary>
        /// 是否是最后一次增值税发票
        /// </summary>
        public bool? IsLastVAT
        {
            get;
            set;
        }
         //*/

        /// <summary>
        /// 是否是复制订单(审核中需要判断)
        /// </summary>
        public bool? IsDuplicateOrder //建议不要
        {
            get;
            set;
        }

        #endregion

        #endregion 可不要属性

        /// <summary>
        /// 订单中快照的下单人实名认证信息表
        /// </summary>
        [DataMember]
        public SOCustomerAuthentication CustomerAuthentication { get; set; }
    }
    /// <summary>
    /// 订单基本信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class SOBaseInfo
    {
        public SOBaseInfo()
        {
            Merchant = new Common.Merchant();
        }

        [DataMember]
        public int? SysNo
        {
            get;
            set;
        }

        [DataMember]
        public string CompanyCode
        {
            get;
            set;
        }

        [DataMember]
        public Common.Merchant Merchant
        {
            get;
            set;
        }

        [DataMember]
        public string LanguageCode
        {
            get;
            set;
        }

        #region 订单基本信息

        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMember]
        public string SOID
        {
            get;
            set;
        }

        /// <summary>
        /// 订单类型
        /// </summary>
        [DataMember]
        public SOType? SOType
        {
            get;
            set;
        }

        /// <summary>
        /// 客户系统编号
        /// </summary>
        [DataMember]
        public int? CustomerSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 客户系统账户
        /// </summary>
        [DataMember]
        public string CustomerID
        {
            get;
            set;
        }

        /// <summary>
        /// 客户名称
        /// </summary>
        [DataMember]
        public string CustomerName
        {
            get;
            set;
        }

        /// <summary>
        /// 用户积分
        /// </summary>
        [DataMember]
        public Int32? CustomerPoint
        {
            get;
            set;
        }

        /// <summary>
        /// 用户类型
        /// </summary>
        [DataMember]
        public Int32? KFCStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 订单状态
        /// </summary>
        [DataMember]
        public SOStatus? Status
        {
            get;
            set;
        }

        /// <summary>
        /// 订单创建时间
        /// </summary>
        [DataMember]
        public DateTime? _createTime;
        public DateTime? CreateTime
        {
            get { return _createTime ?? DateTime.Now; }
            set { _createTime = value; }
        }

        /// <summary>
        /// 是否是批发价购买其中的商品
        /// </summary>
        private bool _isWholeSale;
        [DataMember]
        public bool? IsWholeSale
        {
            get { return _isWholeSale; }
            set { _isWholeSale = value ?? false; }
        }

        /// <summary>
        /// 是否大货
        /// </summary>
        private bool _isLarge;
        [DataMember]
        public bool? IsLarge
        {
            get { return _isLarge; }
            set { _isLarge = value ?? false; }
        }

        private int _gainPoint;
        /// <summary>
        /// 订单获得积分
        /// </summary>
        [DataMember]
        public int? GainPoint //PointAmt
        {
            get { return _gainPoint; }
            set { _gainPoint = value ?? 0; }
        }

        /// <summary>
        /// 用户下单备注
        /// </summary>
        [DataMember]
        public string Memo
        {
            get;
            set;
        }

        [DataMember]
        public string Note
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        [DataMember]
        public int? SalesManSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 给客户的留言信息
        /// </summary>
        [DataMember]
        public string MemoForCustomer
        {
            get;
            set;
        }

        /// <summary>
        /// 订单修改时间
        /// </summary>
        [DataMember]
        public DateTime? UpdateTime
        {
            get;
            set;
        }

        /// <summary>
        /// 修改订单用户编号
        /// </summary>
        [DataMember]
        public int? UpdateUserSysNo
        {
            get;
            set;
        }

        private bool _needInvoice;
        /// <summary>
        /// 是否开票
        /// </summary>
        [DataMember]
        public bool? NeedInvoice
        {
            get { return _needInvoice; }
            set { _needInvoice = value ?? false; }
        }

        ///// <summary>
        ///// 是否物流拒收,是否自动RMA
        ///// 与订单状态SOStatus.Decline 是同一个意思
        ///// </summary>
        //public bool? HaveAutoRMA //通过新加的状态（物流拒收：Decline）来标识
        //{
        //    get;
        //    set;
        //}

        #endregion 订单基本信息

        #region 特殊订单信息

        private bool _isPhoneOrder;
        /// <summary>
        /// 是否电话订购
        /// </summary>
        [DataMember]
        public bool? IsPhoneOrder //IsMobilePhone
        {
            get { return _isPhoneOrder; }
            set { _isPhoneOrder = value ?? false; }
        }

        private bool _isBackOrder;
        /// <summary>
        /// 主要是因为前后台库存不同步,产生的并发异常单.即前台认为有库存,下单成功了，但到后台发现实际已经没有库存了,就会标记为BackOrder.
        /// </summary>
        [DataMember]
        public bool? IsBackOrder
        {
            get { return _isBackOrder; }
            set { _isBackOrder = value ?? false; }
        }

        private bool _isVIP;
        /// <summary>
        /// 是否是VIP订单
        /// </summary>
        [DataMember]
        public bool? IsVIP
        {
            get { return _isVIP; }
            set { _isVIP = value ?? false; }
        }


        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string VIPSOType
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string VIPUserType
        {
            get;
            set;
        }

        private bool _isExtendWarrantyOrder;
        /// <summary>
        /// 是否是延保订单 ？？
        /// </summary>
        [DataMember]
        public bool? IsExtendWarrantyOrder
        {
            get { return _isExtendWarrantyOrder; }
            set { _isExtendWarrantyOrder = value ?? false; }
        }

        private bool _isExpiateOrder;
        /// <summary>
        /// 是否补偿订单
        /// </summary>
        [DataMember]
        public bool? IsExpiateOrder
        {
            get { return _isExpiateOrder; }
            set { _isExpiateOrder = value ?? false; }
        }

        private bool _isExperienceOrder;
        /// <summary>
        /// 是否体验厅订单
        /// </summary>
        [DataMember]
        public bool? IsExperienceOrder
        {
            get { return _isExperienceOrder; }
            set { _isExperienceOrder = value ?? false; }
        }

        private bool _isDCOrder;
        /// <summary>
        /// 是否是秒杀订单
        /// </summary>
        [DataMember]
        public bool? IsDCOrder
        {
            get { return _isDCOrder; }
            set { _isDCOrder = value ?? false; }
        }

        /// <summary>
        /// 秒杀状态
        /// </summary>
        [DataMember]
        public int? DCStatus  //DC_Status
        {
            get;
            set;
        }

        /// <summary>
        /// 团购处理状态（后台使用）
        /// </summary>
        [DataMember]
        public SettlementStatus? SettlementStatus //SettlementStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 团购的编号
        /// </summary>
        [DataMember]
        public int? ReferenceSysNo// ReferenceSysno
        {
            get;
            set;
        }

        /// <summary>
        /// 第三方订单标识。默认为ECCentral.BizEntity.SO.SpecialSOType.Normal
        /// </summary>
        private SpecialSOType _specialSOType;
        [DataMember]
        public SpecialSOType? SpecialSOType
        {
            get { return _specialSOType; }
            set { _specialSOType = value ?? ECCentral.BizEntity.SO.SpecialSOType.Normal; }
        }
        #endregion

        #region 拆分订单信息

        /// <summary>
        ///  订单拆分类型
        /// </summary>
        [DataMember]
        public SOSplitType? SplitType //SoSplitType
        {
            get;
            set;
        }

        /// <summary>
        /// 拆分前的主订单编号。SOSplitType.SubSO 时才有值
        /// </summary>
        [DataMember]
        public int? SOSplitMaster
        {
            get;
            set;
        }
        #endregion 拆分订单信息

        #region 订单费用信息

        private bool _isPremium;
        /// <summary>
        /// 是否有保价
        /// </summary>
        [DataMember]
        public bool? IsPremium
        {
            get { return _isPremium; }
            set { _isPremium = value ?? false; }
        }

        private decimal _premiumAmount;
        /// <summary>
        /// 保价费(>=0)
        /// </summary>
        [DataMember]
        public decimal? PremiumAmount
        {
            get { return _premiumAmount; }
            set { _premiumAmount = value ?? 0.00M; }
        }

        private decimal _shipPrice;
        /// <summary>
        /// 运费(>=0)
        /// </summary>
        [DataMember]
        public decimal? ShipPrice
        {
            get { return _shipPrice; }
            set { _shipPrice = value ?? 0.00M; }
        }

        private decimal _manualShipPrice;
        /// <summary>
        /// 手工设置运费(>=0)
        /// </summary>
        [DataMember]
        public decimal? ManualShipPrice
        {
            get { return _manualShipPrice; }
            set { _manualShipPrice = value ?? 0.00M; }
        }

        private decimal _payPrice;
        /// <summary>
        /// 手续率(>=0)
        /// </summary>
        [DataMember]
        public decimal? PayPrice
        {
            get { return _payPrice; }
            set { _payPrice = value ?? 0.00M; }
        }

        private decimal _promotionAmount;
        /// <summary>
        /// 所有促销活动折扣总额(&lt;=0)。注：除优惠券折扣外的所有活动折扣总额
        /// </summary>
        [DataMember]
        public decimal? PromotionAmount // DiscountAmt
        {
            get { return _promotionAmount; }
            set { _promotionAmount = value ?? 0.00M; }
        }

        private decimal _soAmount;
        /// <summary>
        /// 订单商品总额(>=0)
        /// </summary>
        [DataMember]
        public decimal? SOAmount //SOAmt
        {
            get { return _soAmount; }
            set { _soAmount = value ?? 0.00M; }
        }

        private int _payTypeSysNo;
        /// <summary>
        /// 支付方式系统编号
        /// </summary>
        [DataMember]
        public int? PayTypeSysNo
        {
            get { return _payTypeSysNo; }
            set { _payTypeSysNo = value ?? 0; }
        }

        private bool _isUseGiftCard;
        /// <summary>
        /// 是否礼品卡支付
        /// </summary>
        [DataMember]
        public bool? IsUseGiftCard
        {
            get { return _isUseGiftCard; }
            set { _isUseGiftCard = value ?? false; }
        }

        private bool _isUsePrePay;
        /// <summary>
        /// 是否使用余额支付
        /// </summary>
        [DataMember]
        public bool? IsUsePrePay
        {
            get { return _isUsePrePay; }
            set { _isUsePrePay = value ?? false; }
        }

        private bool _payWhenReceived;
        /// <summary>
        /// 是否是货到付款，ture表示是。在取得数据的时候必须要赋值与IPP3映射的时候要注意
        /// </summary>
        [DataMember]
        public bool? PayWhenReceived
        {
            get { return _payWhenReceived; }
            set { _payWhenReceived = value ?? false; }
        }

        private bool _isNet;

        [DataMember]
        public bool? IsNet
        {
            get { return _isNet; }
            set { _isNet = value ?? false; }
        }

        #region 已支付费用

        private int _pointPay;
        /// <summary>
        /// 支付的积分
        /// </summary>
        [DataMember]
        public int? PointPay
        {
            get { return _pointPay; }
            set { _pointPay = value ?? 0; }
        }

        private decimal _pointPayAmount;
        /// <summary>
        /// 支付的积分（PointPay）转换成的钱(>=0)
        /// </summary>
        [DataMember]
        public decimal? PointPayAmount
        {
            get { return _pointPayAmount; }
            set { _pointPayAmount = value ?? 0.00M; }
        }

        private decimal _prepayAmount;
        /// <summary>
        /// 余额支付值(>=0)
        /// </summary>
        [DataMember]
        public decimal? PrepayAmount
        {
            get { return _prepayAmount; }
            set { _prepayAmount = value ?? 0.00M; }
        }

        private decimal _giftCardPay;
        /// <summary>
        /// 礼品卡支付值(>=0)
        /// </summary>
        [DataMember]
        public decimal? GiftCardPay
        {
            get { return _giftCardPay; }
            set { _giftCardPay = value ?? 0.00M; }
        }

        private decimal _couponAmount;
        /// <summary>
        /// 优惠券抵消金额(&lt;=0)
        /// </summary>
        [DataMember]
        public decimal? CouponAmount
        {
            get { return _couponAmount; }
            set { _couponAmount = value ?? 0.00M; }
        }

        private decimal _tariffAmount;
        /// <summary>
        /// 关税(>=0)
        /// </summary>
        [DataMember]
        public decimal? TariffAmount
        {
            get { return _tariffAmount; }
            set { _tariffAmount = value ?? 0.00M; }
        }

        /// <summary>
        /// 现金支付(>=0):SOAmount - PointPayAmount - CouponAmount
        /// </summary>
        public decimal CashPay
        {
            get
            {
                decimal returnValue = SOAmount.Value - Math.Abs(PointPayAmount.Value) - Math.Abs(CouponAmount.Value);
                return returnValue >= 0 ? returnValue : 0.00M;
            }
        }

        /// <summary>
        /// 订单总金额(>=0):SOAmount - PointPayAmount - CouponAmount + PayPrice + ShipPrice + PremiumAmount - DiscountAmount
        /// </summary>
        public decimal SOTotalAmount
        {
            get
            {
                decimal amount = SOAmount.Value
                     - Math.Abs(PointPayAmount.Value)
                     - Math.Abs(CouponAmount.Value)
                     + Math.Abs(PayPrice.Value)
                     + Math.Abs(ShipPrice.Value)
                     + Math.Abs(PremiumAmount.Value)
                     + Math.Abs(TariffAmount.Value)
                     - Math.Abs(PromotionAmount.Value);
                return amount >= 0 ? amount : 0.00M;
            }
        }

        ///// <summary>
        ///// 发票金额
        ///// </summary>
        //public decimal InvoiceAmount
        //{
        //    get
        //    {
        //        decimal amount = SOTotalAmount
        //            - GiftCardPay.Value;
        //        amount = amount >= 0 ? amount : 0;
        //        if (PayWhenReceived.HasValue && PayWhenReceived.Value)
        //        {
        //            amount = Convert.ToInt32(amount * 10) / 10M;
        //        }
        //        return amount;
        //    }
        //}

        /// <summary>
        /// 应收款(>=0):SOAmount - PointPayAmount - CouponAmount + PayPrice + ShipPrice + PremiumAmount - DiscountAmount-PrepayAmount-GiftCardPay。
        /// 如果订单是货到付款,则是去“分”后的金额。如果要用原始的未去分的金额请使用OriginalReceivableAmount。
        /// </summary> 
        public decimal ReceivableAmount
        {
            get
            {
                decimal amount = OriginalReceivableAmount;
                //泰隆银行  不抹掉零头
                //if (PayWhenReceived.HasValue && PayWhenReceived.Value)
                //{
                //    amount = (int)(amount * 10) / 10M;
                //}
                return amount >= 0M ? Math.Round(amount,2) : 0.00M;
            }
        }

        /// <summary>
        /// 应收款(>=0):SOAmount - PointPayAmount - CouponAmount + PayPrice + ShipPrice + PremiumAmount - DiscountAmount-PrepayAmount-GiftCardPay。
        /// </summary> 
        public decimal OriginalReceivableAmount
        {
            get
            {
                decimal amount = SOTotalAmount
                    - Math.Abs(PrepayAmount.Value)
                    - Math.Abs(GiftCardPay.Value);
                return amount >= 0 ? amount : 0.00M;
            }
        }

        #endregion

        #endregion 订单费用信息

        #region 锁单操作

        /// <summary>
        /// 订单锁定用户
        /// </summary>
        [DataMember]
        public int? HoldUser
        {
            get;
            set;
        }

        /// <summary>
        /// 锁定状态
        /// </summary>
        [DataMember]
        public SOHoldStatus? HoldStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 订单锁定原因
        /// </summary>
        [DataMember]
        public string HoldReason
        {
            get;
            set;
        }

        /// <summary>
        /// 锁定时间
        /// </summary>
        [DataMember]
        public DateTime? HoldTime
        {
            get;
            set;
        }

        ///// <summary>
        ///// 锁单多少分钟
        ///// </summary>
        //public int? HoldMinutes
        //{
        //    get;
        //    set;
        //}
        #endregion 锁单操作


        /// <summary>
        /// 订单分仓主表编号。
        /// </summary>
        [DataMember]
        public int? ShoppingMasterSysNo ////Shopping.dbo.ShoppingMaster.SysNo，订单分仓是订单的一个逻辑，用来确定订单商品的发货仓库
        {
            get;
            set;
        }

        #region 订单审核　不要下面字段通过订单状态更改记录来记录
        /*
        /// <summary>
        /// 审核人
        /// </summary>
        public int? AuditUserSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 订单审核时间
        /// </summary>
        public DateTime? AuditTime
        {
            get;
            set;
        }

        /// <summary>
        /// 主管审核时间
        /// </summary>
        public DateTime? ManagerAuditTime
        {
            get;
            set;
        }

        /// <summary>
        /// 主管系统编号
        /// </summary>
        public int? ManagerAuditUserSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 订单自动审核是否发送审核邮件
        /// </summary>
        public bool? AuditSOSendMailFlag
        {
            get;
            set;
        }

        /// <summary>
        /// 自动审核备注
        /// </summary>
        public string AutoAuditMemo 
        {
            get;
            set;
        } 

       
         //*/
        #endregion

        /// <summary>
        /// 订单审核类型，0表示已自动审核，1表示需要手动审核
        /// </summary>
        public AuditType? AuditType //可由审核人来判断是系统审核，还是人为审核
        {
            get;
            set;
        }

    }

    /// <summary>
    /// 订单收件人信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class SOReceiverInfo
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMember]
        public int? SOSysNo
        {
            get;
            set;
        }

        private int _areaSysNo;
        /// <summary>
        /// 收货地址对应编号
        /// </summary>
        [DataMember]
        public int? AreaSysNo //ReceiveAreaSysNo
        {
            get { return _areaSysNo; }
            set { _areaSysNo = value ?? 0; }
        }

        /// <summary>
        /// 收货人名称
        /// </summary>
        [DataMember]
        public string Name //ReceiveContact
        {
            get;
            set;
        }

        /// <summary>
        /// 收货详细地址
        /// </summary>
        [DataMember]
        public string Address
        {
            get;
            set;
        }

        /// <summary>
        /// 收货人手机
        /// </summary>
        [DataMember]
        public string MobilePhone
        {
            get;
            set;
        }

        /// <summary>
        /// 收货人电话
        /// </summary>
        [DataMember]
        public string Phone
        {
            get;
            set;
        }

        /// <summary>
        /// 收货人邮编
        /// </summary>
        [DataMember]
        public string Zip
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 订单配送信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class SOShippingInfo
    {
        private int? soSysNo;
        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMember]
        public int? SOSysNo
        {
            get { return soSysNo; }
            set
            {
                soSysNo = value;
                if (PostInfo != null) PostInfo.SOSysNo = value;
            }
        }
        #region 基本信息

        private int _shipTypeSysNo;
        /// <summary>
        /// 配送方式系统编号
        /// </summary>
        [DataMember]
        public int? ShipTypeSysNo
        {
            get { return _shipTypeSysNo; }
            set { _shipTypeSysNo = value ?? 0; }
        }

        /// <summary>
        /// 分配员系统编号
        /// </summary>
        [DataMember]
        public int? AllocatedManSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 配送员系统编号
        /// </summary>
        [DataMember]
        public int? FreightUserSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 包裹编号
        /// </summary>
        [DataMember]
        public string PackageID
        {
            get;
            set;
        }

        /// <summary>
        /// 出库员系统编号
        /// </summary>
        [DataMember]
        public int? OutUserSysNo
        {
            get;
            set;
        }

        /// <summary>
        /// 出库时间
        /// </summary>
        [DataMember]
        public DateTime? OutTime
        {
            get;
            set;
        }

        /// <summary>
        /// 配送备注
        /// </summary>
        [DataMember]
        public string DeliveryMemo
        {
            get;
            set;
        }
        #endregion

        #region 配送信息

        /// <summary>
        /// 用户下订单时选择的配送日期
        /// </summary>
        [DataMember]
        public DateTime? DeliveryDate
        {
            get;
            set;
        }

        /// <summary>
        /// 预约配送类型
        /// </summary>
        [DataMember]
        public ShipDeliveryType? DeliveryType { get; set; }

        /// <summary>
        /// 配送日期内的 上午 或者 下午
        /// </summary>
        [DataMember]
        public int? DeliveryTimeRange
        {
            get;
            set;
        }

        /// <summary>
        /// A: 工作日/非工作日 N: 工作日 W: 双休日
        /// </summary>
        [DataMember]
        public string RingOutShipType
        {
            get;
            set;
        }

        /// <summary>
        /// 具体的配送时间段描述
        /// </summary>
        [DataMember]
        public string DeliverySection
        {
            get;
            set;
        }

        /// <summary>
        /// 配送承诺
        /// </summary>
        [DataMember]
        public SODeliveryPromise DeliveryPromise
        {
            get;
            set;
        }
        private ECCentral.BizEntity.Invoice.DeliveryType _shippingType = Invoice.DeliveryType.SELF;
        /// <summary>
        /// 配送类型
        /// </summary>
        [DataMember]
        public ECCentral.BizEntity.Invoice.DeliveryType ShippingType
        {
            get { return _shippingType; }
            set { _shippingType = value; }
        }

        /*
        /// <summary>
        /// 配送频率
        /// </summary>
        public SODeliveryFrequency DeliveryFrequency  //DeliveryType 由物流确定，与订单无关
        {
            get;
            set;
        }
        //*/

        #endregion 配送信息

        #region 运输相关

        /// <summary>
        /// 是否有包裹费
        /// </summary>
        [DataMember]
        public bool IsWithPackFee
        {
            get;
            set;
        }

        private decimal _weight;
        /// <summary>
        /// 订单总重量
        /// </summary>
        [DataMember]
        public decimal? Weight //WeightSO
        {
            get { return _weight; }
            set { _weight = value ?? 0.00M; }
        }

        private decimal _shippingFee;
        /// <summary>
        /// 订单运费
        /// </summary>
        [DataMember]
        public decimal? ShippingFee
        {
            get { return _shippingFee; }
            set { _shippingFee = value ?? 0.00M; }
        }

        private decimal _packageFee;
        /// <summary>
        /// 订单包裹费
        /// </summary>
        [DataMember]
        public decimal? PackageFee
        {
            get { return _packageFee; }
            set { _packageFee = value ?? 0.00M; }
        }

        private decimal _registeredFee;
        /// <summary>
        ///
        /// </summary>
        [DataMember]
        public decimal? RegisteredFee
        {
            get { return _registeredFee; }
            set { _registeredFee = value ?? 0.00M; }
        }

        private decimal _weight3PL;
        /// <summary>
        /// 第三方物流重量
        /// </summary>
        [DataMember]
        public decimal? Weight3PL
        {
            get { return _weight3PL; }
            set { _weight3PL = value ?? 0.00M; }
        }

        private decimal _shipCost;
        /// <summary>
        /// 运送成本
        /// </summary>
        [DataMember]
        public decimal? ShipCost
        {
            get { return _shipCost; }
            set { _shipCost = value ?? 0.00M; }
        }

        private decimal _shipCost3PL;
        /// <summary>
        /// 第三方运费成本
        /// </summary>
        [DataMember]
        public decimal? ShipCost3PL
        {
            get { return _shipCost3PL; }
            set { _shipCost3PL = value ?? 0.00M; }
        }

        private decimal _originShipPrice;
        /// <summary>
        /// 原始运费
        /// </summary>
        [DataMember]
        public decimal? OriginShipPrice
        {
            get { return _originShipPrice; }
            set { _originShipPrice = value ?? 0.00M; }
        }

        #endregion 运输相关

        #region 仓库有关

        /// <summary>
        /// 虚库采购单状态
        /// </summary> 
        private VirtualPurchaseOrderStatus? m_VPOStatus;
        [DataMember]
        public VirtualPurchaseOrderStatus? VPOStatus
        {
            get { return m_VPOStatus ?? VirtualPurchaseOrderStatus.Normal; }
            set { m_VPOStatus = value; }
        }
        private bool isCombine;

        /// <summary>
        /// 订单商品在不同仓库但不拆分订单，即要并单。true:表示并单；false:表示非并单
        /// </summary>
        [DataMember]
        public bool? IsCombine
        {
            get { return isCombine; }
            set { isCombine = value ?? false; }
        }

        private bool isMergeComplete;
        /// <summary>
        /// 订单中不同仓库商品是否已经合并到提货仓库
        /// </summary>
        [DataMember]
        public bool? IsMergeComplete
        {
            get { return isMergeComplete; }
            set { isMergeComplete = value ?? false; }
        }

        /// <summary>
        /// 订单中不同仓库商品合并到提货仓库的时间
        /// </summary>
        [DataMember]
        public DateTime? MergeCompleteTime
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public DateTime? MergeOutTime
        {
            get;
            set;
        }
        /// <summary>
        ///  并单的目标仓库
        /// </summary>
        [DataMember]
        public int? MergeToStockSysNo //DestWarehouseNumber
        {
            get;
            set;
        }

        /// <summary>
        /// 库存状态
        /// </summary>
        [DataMember]
        public int? StockStatus
        {
            get;
            set;
        }

        private ECCentral.BizEntity.Invoice.StockType _stockType = ECCentral.BizEntity.Invoice.StockType.SELF;
        /// <summary>
        /// 仓储类型
        /// </summary>
        [DataMember]
        public ECCentral.BizEntity.Invoice.StockType? StockType
        {
            get
            { return _stockType; }
            set
            { _stockType = value ?? ECCentral.BizEntity.Invoice.StockType.SELF; }
        }
        #endregion

        /// <summary>
        /// 是否打印包裹单
        /// </summary>
        private bool _isPrintPackageCover;
        [DataMember]
        public bool? IsPrintPackageCover
        {
            get { return _isPrintPackageCover; }
            set { _isPrintPackageCover = value ?? false; }
        }

        /// <summary>
        /// 邮政自提信息
        /// </summary>
        [DataMember]
        public SOChinaPostInfo PostInfo { get; set; }


        /// <summary>
        /// 本地仓库编号
        /// </summary>
        [DataMember]
        public int? LocalWHSysNo  //按照仓库进行拆单后，订单的发货仓
        {
            get;
            set;
        }
        /*//
        /// <summary>
        /// 本地仓库编号
        /// </summary>
        public int? LocalStockSysNo  //LocalWHSysNo
        {
            get;
            set;
        }
        
        /// <summary>
        /// 配送状态，WAT已出库待发货|SED发往泰隆优选仓库|ARE已送达|DEC仓库拒收
        /// </summary>
        public ReceivingStatus ReceivingStatus //由物流来记录，而不是在订单中记录
        {
            get;
            set;
        }
        //*/

        [DataMember]
        public string TrackingNumberStr { get; set; }
        /// <summary>
        /// 配送公司网址
        /// </summary>
        [DataMember]
        public string OfficialWebsite { get; set; }
    }

    /// <summary>
    /// 订单发票信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class SOInvoiceInfo
    {
        public SOInvoiceInfo()
        {
            VATInvoiceInfo = new SOVATInvoiceInfo();
        }
        private int? soSysNo;
        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMember]
        public int? SOSysNo
        {
            get { return soSysNo; }
            set
            {
                soSysNo = value;
                if (VATInvoiceInfo != null) VATInvoiceInfo.SOSysNo = value;
            }
        }

        /// <summary>
        /// 发票抬头
        /// </summary>
        [DataMember]
        public string Header //ReceiveName
        {
            get;
            set;
        }

        private bool _isVAT;
        /// <summary>
        /// 是否增值税订单
        /// </summary>
        [DataMember]
        public bool? IsVAT
        {
            get { return _isVAT; }
            set { _isVAT = value ?? false; }
        }

        /// <summary>
        /// 发票备注
        /// </summary>
        [DataMember]
        public string InvoiceNote
        {
            get;
            set;
        }

        /// <summary>
        /// 增值税发票号
        /// </summary>
        [DataMember]
        public string InvoiceNo
        {
            get;
            set;
        }

        /// <summary>
        /// 账务备注
        /// </summary>
        [DataMember]
        public string FinanceNote
        {
            get;
            set;
        }
        /// <summary>
        /// 增值税发票信息
        /// </summary>
        [DataMember]
        public SOVATInvoiceInfo VATInvoiceInfo
        {
            get;
            set;
        }


        #region 发票相关
        private ECCentral.BizEntity.Invoice.InvoiceType _invoiceType = ECCentral.BizEntity.Invoice.InvoiceType.SELF;
        /// <summary>
        /// 发票类型
        /// </summary>
        [DataMember]
        public ECCentral.BizEntity.Invoice.InvoiceType? InvoiceType
        {
            get { return _invoiceType; }
            set { _invoiceType = value ?? ECCentral.BizEntity.Invoice.InvoiceType.SELF; }
        }

        /// <summary>
        /// 发票拆分用户编号
        /// </summary>
        [DataMember]
        public int? SplitUserSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 发票拆分时间
        /// </summary>
        [DataMember]
        public DateTime? SplitDateTime
        {
            get;
            set;
        }

        /// <summary>
        /// 是否有多张发票
        /// </summary>
        private int _IsMultiInvoice;
        [DataMember]
        public int? IsMultiInvoice
        {
            get { return _IsMultiInvoice; }
            set { _IsMultiInvoice = value ?? 0; }
        }

        /// <summary>
        /// 是否已开具增值税发票
        /// </summary>
        [DataMember]
        public bool? IsVATPrinted
        {
            get;
            set;
        }

        /// <summary>
        /// 是否需要运费发票
        /// </summary>
        [DataMember]
        public bool? IsRequireShipInvoice
        {
            get;
            set;
        }
        #endregion
    }

    /// <summary>
    /// 订单欺诈验证信息, FP(可能是:Fake Proving 的缩写):欺诈验证, KFC(可能是: Known Fake Customers 的缩写):已知的欺诈用户
    /// </summary>
    [Serializable]
    [DataContract]
    public class SOFPInfo
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMember]
        public int? SOSysNo
        {
            get;
            set;
        }

        #region 判断订单是否欺诈订单 信息

        /// <summary>
        /// 是不是欺诈订单 
        /// </summary>
        [DataMember]
        public FPSOType? FPSOType
        {
            get;
            set;
        }

        /// <summary>
        /// 是欺诈验证的原因
        /// </summary>
        [DataMember]
        public string FPReason
        {
            get;
            set;
        }

        /// <summary>
        /// 此是否已经查询过
        /// </summary>
        [DataMember]
        public bool? IsFPCheck
        {
            get;
            set;
        }

        /// <summary>
        /// 检查是否是欺诈订单的时间
        /// </summary>
        [DataMember]
        public DateTime? FPCheckTime
        {
            get;
            set;
        }

        /// <summary>
        /// 欺诈扩展信息，“RED” 表示红色标识
        /// </summary>
        [DataMember]
        public string FPExtend
        {
            get;
            set;
        }

        #endregion 判断订单是否欺诈订单 信息
    }

    /// <summary>
    /// 订单客户端信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class SOClientInfo
    {
        #region 客户端信息

        /// <summary>
        /// 客户下单IP
        /// </summary>
        [DataMember]
        public string CustomerIPAddress
        {
            get;
            set;
        }

        /// <summary>
        /// 客户下单的Cookie信息
        /// </summary>
        [DataMember]
        public string CustomerCookie
        {
            get;
            set;
        }

        /// <summary>
        /// 移动设备下单
        /// </summary>
        [DataMember]
        public PhoneType PhoneType //IsPhoneOrder
        {
            get;
            set;
        }

        #endregion 客户端信息
    }

    /// <summary>
    /// 订单状态更改信息
    /// </summary>
    [Serializable]
    [DataContract]
    public class SOStatusChangeInfo
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        [DataMember]
        public int? SOSysNo
        {
            get;
            set;
        }
        /// <summary>
        /// 操作业类型
        /// </summary>
        [DataMember]
        public SOOperatorType OperatorType
        {
            get;
            set;
        }

        /// <summary>
        /// 操作者编号
        /// </summary>
        [DataMember]
        public int? OperatorSysNo
        {
            get;
            set;
        }
        /// <summary>
        ///  订单原来状态
        /// </summary>
        [DataMember]
        public SOStatus? OldStatus
        { get; set; }

        /// <summary>
        /// 订单当前状态，要更改到的状态
        /// </summary>
        [DataMember]
        public SOStatus? Status
        { get; set; }


        /// <summary>
        /// 状态更改时间
        /// </summary>
        [DataMember]
        public DateTime? ChangeTime
        { get; set; }

        /// <summary>
        /// 是否发送邮件给客户
        /// </summary>
        [DataMember]
        public bool? IsSendMailToCustomer { get; set; }

        /// <summary>
        /// 状态更改备注
        /// </summary>
        [DataMember]
        public string Note
        {
            get;
            set;
        }

    }

    /// <summary>
    /// 订单参与的促销记录
    /// </summary>
    [Serializable]
    [DataContract]
    public class SOPromotionInfo
    {
        public SOPromotionInfo()
        {
            GiftList = new List<GiftInfo>();
            MasterList = new List<MasterInfo>();
            CouponCodeList = new List<string>();
            SOPromotionDetails = new List<SOPromotionDetailInfo>();
        }

        /// <summary>
        /// 促销规则信息
        /// </summary>
        public string PromoRuleData { get; set; }

        /// <summary>
        /// 所属商家
        /// </summary>
        public int VendorSysNo { get; set; }

        [DataMember]
        public int? SOSysNo { get; set; }

        /// <summary>
        /// 促销类型。
        /// </summary>
        [DataMember]
        public ECCentral.BizEntity.SO.SOPromotionType? PromotionType { get; set; }

        /// <summary>
        /// Combo信息
        /// </summary>
        [DataMember]
        public ComboInfo Combo { get; set; }
        /// <summary>
        /// 促销活动编号。 PromotionType==PromotionType.Gift时表示赠品规则编号 
        /// </summary>
        [DataMember]
        public int? PromotionSysNo { get; set; }

        /// <summary>
        /// 促销活动名称
        /// </summary>
        [DataMember]
        public string PromotionName { get; set; }

        /// <summary>
        /// 当前促销活动内部定义类型，如赠品类促销的内部类型：单品买赠，满额赠送等
        /// </summary>
        [DataMember]
        public string InnerType
        {
            get;
            set;
        }

        /// <summary>
        /// 活动次数
        /// </summary>
        [DataMember]
        public int? Time { get; set; }

        private int _priority;
        /// <summary>
        /// 参加同类活动（由PromotionType确定）的先后次序
        /// </summary>
        [DataMember]
        public int? Priority { get { return _priority; } set { _priority = value ?? 0; } }

        /// <summary>
        /// 折扣总额,包含所有次数
        /// </summary>
        [DataMember]
        public decimal? DiscountAmount { get; set; }

        [DataMember]
        public decimal? Discount { get; set; }

        /// <summary>
        /// 此活动获取的积分
        /// </summary>
        [DataMember]
        public int? GainPoint { get; set; }

        /// <summary>
        /// 此活动获取的优惠券列表
        /// </summary>
        [DataMember]
        public List<string> CouponCodeList { get; set; }

        /// <summary>
        /// 目前为赠品的主商品列表
        /// </summary>
        [DataMember]
        public List<MasterInfo> MasterList { get; set; }

        /// <summary>
        /// 此活动获得的赠品列表
        /// </summary>
        [DataMember]
        public List<GiftInfo> GiftList { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Note
        {
            get;
            set;
        }

        /// <summary>
        /// 促销活动详细
        /// </summary>
        [DataMember]
        public List<SOPromotionDetailInfo> SOPromotionDetails
        {
            get;
            set;
        }

        [DataMember]
        public int ReferenceType
        {
            get;
            set;
        }

        /// <summary>
        /// 取得的赠品记录信息
        /// </summary>
        [Serializable]
        [DataContract]
        public class GiftInfo
        {
            /// <summary>
            /// 赠品编号
            /// </summary>
            [DataMember]
            public int ProductSysNo { get; set; }

            /// <summary>
            /// 赠品数量（实际数量）
            /// </summary>
            [DataMember]
            public int Quantity { get; set; }

            /// <summary>
            /// 单次活动赠送的数量
            /// </summary>
            [DataMember]
            public int QtyPreTime { get; set; }
        }

        [Serializable]
        [DataContract]
        public class MasterInfo
        {
            /// <summary>
            /// 主商品编号
            /// </summary>
            [DataMember]
            public int ProductSysNo { get; set; }

            /// <summary>
            /// 主商品数量
            /// </summary>
            [DataMember]
            public int Quantity { get; set; }

            /// <summary>
            /// 主商品类型
            /// </summary>
            [DataMember]
            public SOProductType? ProductType { get; set; }


            /// <summary>
            /// 单次活动主商品数量
            /// </summary>
            [DataMember]
            public int QtyPreTime { get; set; }

        }
    }



    /// <summary>
    /// 订单参与的促销活动取得的优惠分摊到主商品的明细
    /// </summary>
    [Serializable]
    [DataContract]
    public class SOPromotionDetailInfo
    {
        public SOPromotionDetailInfo()
        {
            //  GiftList = new List<ECCentral.BizEntity.SO.SOPromotionInfo.GiftInfo>();
            //  CouponCodeList = new List<string>();
        }

        /// <summary>
        /// 主商品编号
        /// </summary>
        [DataMember]
        public int? MasterProductSysNo { get; set; }

        ///// <summary>
        ///// 主商品类型
        ///// </summary>
        //[DataMember]
        //public SOProductType? MasterProductType { get; set; }

        /// <summary>
        /// 参与活动的主商品数量
        /// </summary>
        [DataMember]
        public int? MasterProductQuantity { get; set; }

        /// <summary>
        /// 分摊到主商品的总额
        /// </summary>
        [DataMember]
        public decimal? DiscountAmount { get; set; }

        /// <summary>
        /// 分摊到主商品获取的总积分
        /// </summary>
        [DataMember]
        public int? GainPoint { get; set; }

        ///// <summary>
        ///// 此活动获取的优惠券列表
        ///// </summary>
        //[DataMember]
        //public List<string> CouponCodeList { get; set; }

        ///// <summary>
        ///// 此活动获得的赠品列表
        ///// </summary>
        //[DataMember]
        //public List<ECCentral.BizEntity.SO.SOPromotionInfo.GiftInfo> GiftList { get; set; }

    }

    [Serializable]
    [DataContract]
    public class SOChinaPostInfo
    {
        [DataMember]
        public int? SysNo { get; set; }
        /// <summary>
        /// 订单系统编号
        /// </summary>
        [DataMember]
        public int? SOSysNo { get; set; }
        /// <summary>
        /// 自提点唯一ID
        /// </summary>
        [DataMember]
        public int? PickupPointID { get; set; }
        /// <summary>
        /// 自提点编码
        /// </summary>
        [DataMember]
        public string PointCode { get; set; }
        /// <summary>
        /// 自提点名称
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public string Description { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        [DataMember]
        public string Address { get; set; }
        /// <summary>
        /// 到达天数
        /// </summary>
        [DataMember]
        public int? ArrivalDays { get; set; }
        /// <summary>
        /// 能够接受的最大长
        /// </summary>
        [DataMember]
        public decimal? Length { get; set; }
        /// <summary>
        /// 能够接受的最大宽
        /// </summary>
        [DataMember]
        public decimal? Width { get; set; }
        /// <summary>
        /// 能够接受的最大高
        /// </summary>
        [DataMember]
        public decimal? Height { get; set; }
        /// <summary>
        /// 能够接受的最大重量
        /// </summary>
        [DataMember]
        public decimal? Weight { get; set; }
        /// <summary>
        /// 是否能够代收款
        /// </summary>
        [DataMember]
        public bool? CanAgentFund { get; set; }
        /// <summary>
        /// 自提点的RegionID
        /// </summary> 
        [DataMember]
        public int? RegionID { get; set; }
        /// <summary>
        /// 自提点所在经度
        /// </summary>
        [DataMember]
        public decimal? Longitude { get; set; }
        /// <summary>
        /// 自提点所在纬度
        /// </summary>
        [DataMember]
        public decimal? Latitude { get; set; }
        /// <summary>
        ///  自提点联系邮政编码
        /// </summary>
        [DataMember]
        public string ZipCode { get; set; }
        /// <summary>
        ///  自提点联系电话
        /// </summary>
        [DataMember]
        public string Phone { get; set; }
        /// <summary>
        /// 自提点联系人
        /// </summary>
        [DataMember]
        public string ContactName { get; set; }
        /// <summary>
        /// 自提点所在国家
        /// </summary>
        [DataMember]
        public string State { get; set; }
        /// <summary>
        /// 自提点所在省
        /// </summary>
        [DataMember]
        public string Province { get; set; }
        /// <summary>
        /// 自提点所在城市
        /// </summary>
        [DataMember]
        public string City { get; set; }
        /// <summary>
        /// 自提点所在区
        /// </summary>
        [DataMember]
        public string District { get; set; }
    }
}
