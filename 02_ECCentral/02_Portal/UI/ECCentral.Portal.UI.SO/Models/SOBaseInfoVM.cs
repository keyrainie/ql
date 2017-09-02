using System;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.SO;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.SO.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
namespace ECCentral.Portal.UI.SO.Models
{
    public class SOBaseInfoVM : ModelBase
    {
        public SOBaseInfoVM()
        {
            Merchant = new Merchant();
            CustomerChannel = new WebChannel();
            CompanyCode = CPApplication.Current.CompanyCode;
        }

        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        private String m_CompanyCode;
        public String CompanyCode
        {
            get { return this.m_CompanyCode; }
            set { this.SetValue("CompanyCode", ref m_CompanyCode, value); }
        }

        private Merchant m_Merchant;
        public Merchant Merchant
        {
            get { return this.m_Merchant; }
            set { this.SetValue("Merchant", ref m_Merchant, value); }
        }

        private String m_SOID;
        public String SOID
        {
            get { return this.m_SOID; }
            set { this.SetValue("SOID", ref m_SOID, value); }
        }

        private SOType m_SOType;
        [Validate(ValidateType.Required)]
        public SOType SOType
        {
            get { return this.m_SOType; }
            set { this.SetValue("SOType", ref m_SOType, value); }
        }
        /// <summary>
        /// 手动改变sotype的值，不触发对应控件的changed事件 用于分期付款订单的创建
        /// </summary>
        public SOType SetSOType
        {
            set { m_SOType = value; }
        }

        private Int32? m_CustomerSysNo;
        [Validate(ValidateType.Required)]
        public Int32? CustomerSysNo
        {
            get { return this.m_CustomerSysNo; }
            set { this.SetValue("CustomerSysNo", ref m_CustomerSysNo, value); }
        }

        private String m_CustomerID;
        [Validate(ValidateType.Required)]
        public String CustomerID
        {
            get { return this.m_CustomerID; }
            set { this.SetValue("CustomerID", ref m_CustomerID, value); }
        }

        private WebChannel m_CustomerChannel;
        public WebChannel CustomerChannel
        {
            get { return this.m_CustomerChannel; }
            set { this.SetValue("CustomerChannel", ref m_CustomerChannel, value); }
        }

        private String m_CustomerName;
        public String CustomerName
        {
            get { return this.m_CustomerName; }
            set { this.SetValue("CustomerName", ref m_CustomerName, value); }
        }

        private Int32? m_CustomerPoint;
        /// <summary>
        /// 用户积分
        /// </summary>
        public Int32? CustomerPoint
        {
            get { return this.m_CustomerPoint ?? 0; }
            set { this.SetValue("CustomerPoint", ref m_CustomerPoint, value); }
        }

        private Int32? kfcStatus;
        /// <summary>
        /// 用户类型
        /// </summary>
        [Validate(ValidateType.Required)]
        public Int32? KFCStatus
        {
            get { return kfcStatus; }
            set { SetValue<int?>("KFCStatus", ref kfcStatus, value); }
        }

        private SOStatus? m_Status;
        public SOStatus? Status
        {
            get { return this.m_Status; }
            set { this.SetValue("Status", ref m_Status, value); }
        }

        public String SOStatusDisplay
        {
            get
            {
                string result = string.Empty;
                switch (Status)
                {
                    case SOStatus.Split:
                        result = ResSOMaintain.Info_SOStatus_Split;
                        break;
                    case SOStatus.Abandon:
                        result = ResSOMaintain.Info_SOStatus_Abandon;
                        break;
                    case SOStatus.Origin:
                        result = ResSOMaintain.Info_SOStatus_Origin;
                        break;
                    case SOStatus.WaitingOutStock:
                        result = ResSOMaintain.Info_SOStatus_WaitingOutStock;
                        break;
                    case SOStatus.Shipping:
                        result = ResSOMaintain.Info_SOStatus_Shipping;
                        break;
                    case SOStatus.WaitingManagerAudit:
                        result = ResSOMaintain.Info_SOStatus_WaitingManagerAudit;
                        break;
                    case SOStatus.OutStock:
                        result = ResSOMaintain.Info_SOStatus_OutStock;
                        break;
                    case SOStatus.Complete:
                        result = ResSOMaintain.Info_SOStatus_Complete;
                        break;
                    case SOStatus.Reported:
                        result = ResSOMaintain.Info_SOStatus_Reported;
                        break;
                    case SOStatus.Reject:
                        result = ResSOMaintain.Info_SOStatus_Reject;
                        break;
                    case SOStatus.ShippingReject:
                        result = ResSOMaintain.Info_SOStatus_ShippingReject;
                        break;
                    case SOStatus.CustomsPass:
                        result = ResSOMaintain.Info_SOStatus_CustomsPass;
                        break;
                    case SOStatus.CustomsReject:
                        result = ResSOMaintain.Info_SOStatus_CustomsReject;
                        break;
                    case SOStatus.SystemCancel:
                        result = ResSOMaintain.Info_SOStatus_Abandon;
                        break;
                    default:
                        break;
                }
                return result;
            }
        }

        private DateTime? m_OrderTime;
        public DateTime? OrderTime
        {
            get { return this.m_OrderTime; }
            set { this.SetValue("OrderTime", ref m_OrderTime, value); }
        }

        private Boolean? m_IsWholeSale;
        public Boolean? IsWholeSale
        {
            get { return this.m_IsWholeSale; }
            set { this.SetValue("IsWholeSale", ref m_IsWholeSale, value); }
        }

        private Boolean? m_IsLarge;
        public Boolean? IsLarge
        {
            get { return this.m_IsLarge; }
            set { this.SetValue("IsLarge", ref m_IsLarge, value); }
        }

        private Int32 m_GainPoint;
        public Int32 GainPoint
        {
            get { return this.m_GainPoint; }
            set { this.SetValue("GainPoint", ref m_GainPoint, value); }
        }

        private String m_Memo;
        public String Memo
        {
            get { return this.m_Memo; }
            set { this.SetValue("Memo", ref m_Memo, value); }
        }

        private String m_Note;
        public String Note
        {
            get { return this.m_Note; }
            set { this.SetValue("Note", ref m_Note, value); }
        }

        private Int32? m_SalesManSysNo;
        public Int32? SalesManSysNo
        {
            get { return this.m_SalesManSysNo; }
            set { this.SetValue("SalesManSysNo", ref m_SalesManSysNo, value); }
        }

        private String m_MemoForCustomer;
        public String MemoForCustomer
        {
            get { return this.m_MemoForCustomer; }
            set { this.SetValue("MemoForCustomer", ref m_MemoForCustomer, value); }
        }

        private DateTime? m_UpdateTime;
        public DateTime? UpdateTime
        {
            get { return this.m_UpdateTime; }
            set { this.SetValue("UpdateTime", ref m_UpdateTime, value); }
        }

        private Boolean? m_NeedInvoice;
        public Boolean? NeedInvoice
        {
            get { return this.m_NeedInvoice; }
            set { this.SetValue("NeedInvoice", ref m_NeedInvoice, value); }
        }

        private Boolean? m_IsPhoneOrder;
        public Boolean? IsPhoneOrder
        {
            get { return this.m_IsPhoneOrder; }
            set { this.SetValue("IsPhoneOrder", ref m_IsPhoneOrder, value); }
        }

        private Boolean? m_IsBackOrder;
        public Boolean? IsBackOrder
        {
            get { return this.m_IsBackOrder; }
            set { this.SetValue("IsBackOrder", ref m_IsBackOrder, value); }
        }

        private Boolean? m_IsVIP;
        public Boolean? IsVIP
        {
            get { return this.m_IsVIP; }
            set { this.SetValue("IsVIP", ref m_IsVIP, value); }
        }

        private String m_VIPSOType;
        public String VIPSOType
        {
            get { return this.m_VIPSOType; }
            set { this.SetValue("VIPSOType", ref m_VIPSOType, value); }
        }

        private String m_VIPUserType;
        public String VIPUserType
        {
            get { return this.m_VIPUserType; }
            set { this.SetValue("VIPUserType", ref m_VIPUserType, value); }
        }

        public Boolean m_Is_Enterprise_SO;
        public Boolean Is_Enterprise_SO
        {
            get
            {
                if (VIPSOType == "E")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set { this.SetValue("Is_Enterprise_SO", ref m_Is_Enterprise_SO, value); }
        }

        public Boolean m_Is_Person_SO;
        public Boolean Is_Person_SO
        {
            get
            {
                if (VIPSOType == "P" || string.IsNullOrEmpty(VIPSOType))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set { this.SetValue("Is_Person_SO", ref m_Is_Person_SO, value); }
        }

        public Boolean m_Is_Channel_SO;
        public Boolean Is_Channel_SO
        {
            get
            {
                if (VIPSOType == "C")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set { this.SetValue("Is_Channel_SO", ref m_Is_Channel_SO, value); }
        }

        public Boolean m_Is_New_Customer;
        public Boolean Is_New_Customer
        {
            get
            {
                if (VIPUserType == "N" || string.IsNullOrEmpty(VIPUserType))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set { this.SetValue("Is_New_Customer", ref m_Is_New_Customer, value); }
        }

        public Boolean m_Is_Old_Customer;
        public Boolean Is_Old_Customer
        {
            get
            {
                if (VIPUserType == "O")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set { this.SetValue("Is_Old_Customer", ref m_Is_Old_Customer, value); }
        }

        private Boolean? m_IsExtendWarrantyOrder;
        public Boolean? IsExtendWarrantyOrder
        {
            get { return this.m_IsExtendWarrantyOrder; }
            set { this.SetValue("IsExtendWarrantyOrder", ref m_IsExtendWarrantyOrder, value); }
        }

        //是否体验厅订单
        private Boolean? m_IsExperienceOrder;
        public Boolean? IsExperienceOrder
        {
            get { return this.m_IsExperienceOrder; }
            set { this.SetValue("IsExperienceOrder", ref m_IsExperienceOrder, value); }
        }

        private Boolean? m_IsExpiateOrder;
        public Boolean? IsExpiateOrder
        {
            get { return this.m_IsExpiateOrder; }
            set { this.SetValue("IsExpiateOrder", ref m_IsExpiateOrder, value); }
        }

        private Boolean? m_IsDCOrder;
        public Boolean? IsDCOrder
        {
            get { return this.m_IsDCOrder; }
            set { this.SetValue("IsDCOrder", ref m_IsDCOrder, value); }
        }

        private Int32? m_DCStatus;
        public Int32? DCStatus
        {
            get { return this.m_DCStatus; }
            set { this.SetValue("DCStatus", ref m_DCStatus, value); }
        }

        private SettlementStatus? m_SettlementStatus;
        public SettlementStatus? SettlementStatus
        {
            get { return this.m_SettlementStatus; }
            set { this.SetValue("SettlementStatus", ref m_SettlementStatus, value); }
        }

        private Int32? m_ReferenceSysno;
        public Int32? ReferenceSysNo
        {
            get { return this.m_ReferenceSysno; }
            set { this.SetValue("ReferenceSysNo", ref m_ReferenceSysno, value); }
        }

        private SOSplitType m_SplitType;
        public SOSplitType SplitType
        {
            get { return this.m_SplitType; }
            set { this.SetValue("SplitType", ref m_SplitType, value); }
        }

        private Int32? m_SOSplitMaster;
        public Int32? SOSplitMaster
        {
            get { return this.m_SOSplitMaster; }
            set { this.SetValue("SOSplitMaster", ref m_SOSplitMaster, value); }
        }

        private Boolean? m_IsPremium;
        public Boolean? IsPremium
        {
            get { return this.m_IsPremium; }
            set { this.SetValue("IsPremium", ref m_IsPremium, value); }
        }

        private Decimal? m_PremiumAmount;
        public Decimal? PremiumAmount
        {
            get { return this.m_PremiumAmount; }
            set { this.SetValue("PremiumAmount", ref m_PremiumAmount, value); }
        }

        private Decimal? m_ManualShipPrice;
        public Decimal? ManualShipPrice
        {
            get { return this.m_ManualShipPrice; }
            set { this.SetValue("ManualShipPrice", ref m_ManualShipPrice, value); }
        }

        private Decimal? m_ShipPrice;
        public Decimal? ShipPrice
        {
            get { return this.m_ShipPrice; }
            set { this.SetValue("ShipPrice", ref m_ShipPrice, value); }
        }

        private Decimal? m_PayPrice;
        public Decimal? PayPrice
        {
            get { return this.m_PayPrice; }
            set { this.SetValue("PayPrice", ref m_PayPrice, value); }
        }

        /// <summary>
        /// 所有促销活动折扣总额(&lt;=0)。注：除优惠券折扣外的所有活动折扣总额
        /// </summary>
        private Decimal? m_PromotionAmount;
        public Decimal? PromotionAmount
        {
            get { return this.m_PromotionAmount; }
            set { this.SetValue("PromotionAmount", ref m_PromotionAmount, value); }
        }

        /// <summary>
        /// 订单商品总额(>=0)
        /// </summary>
        private Decimal? m_SOAmount;
        public Decimal? SOAmount
        {
            get { return this.m_SOAmount; }
            set { this.SetValue("SOAmount", ref m_SOAmount, value); }
        }

        private Int32? m_PayTypeSysNo;
        [Validate(ValidateType.Required)]
        public Int32? PayTypeSysNo
        {
            get { return this.m_PayTypeSysNo; }
            set { this.SetValue("PayTypeSysNo", ref m_PayTypeSysNo, value); }
        }

        private Boolean? m_PayWhenReceived;
        public Boolean? PayWhenReceived
        {
            get { return this.m_PayWhenReceived; }
            set { this.SetValue("PayWhenReceived", ref m_PayWhenReceived, value); }
        }

        private Boolean? m_IsNet;
        public Boolean? IsNet
        {
            get { return this.m_IsNet; }
            set { this.SetValue("IsNet", ref m_IsNet, value); }
        }

        private Int32? m_PointPay;
        public Int32? PointPay
        {
            get { return this.m_PointPay; }
            set { this.SetValue("PointPay", ref m_PointPay, value); }
        }

        private Decimal? m_PointPayAmount;
        public Decimal? PointPayAmount
        {
            get { return this.m_PointPayAmount; }
            set { this.SetValue("PointPayAmount", ref m_PointPayAmount, value); }
        }

        /// <summary>
        /// 是否余额支付
        /// </summary>
        private Boolean? m_IsUsePrePay;
        public Boolean? IsUsePrePay
        {
            get { return this.m_IsUsePrePay; }
            set { this.SetValue("IsUsePrePay", ref m_IsUsePrePay, value); }
        }


        private Decimal? m_PrepayAmount;
        public Decimal? PrepayAmount
        {
            get { return this.m_PrepayAmount; }
            set { this.SetValue("PrepayAmount", ref m_PrepayAmount, value); }
        }


        /// <summary>
        /// 是否礼品卡支付
        /// </summary>
        private Boolean? m_IsUseGiftCard;
        public Boolean? IsUseGiftCard
        {
            get { return this.m_IsUseGiftCard; }
            set { this.SetValue("IsUseGiftCard", ref m_IsUseGiftCard, value); }
        }

        private Decimal? m_GiftCardPay;
        public Decimal? GiftCardPay
        {
            get { return this.m_GiftCardPay; }
            set { this.SetValue("GiftCardPay", ref m_GiftCardPay, value); }
        }

        /// <summary>
        /// 优惠券抵扣
        /// </summary>
        private Decimal? m_CouponAmount;
        public Decimal? CouponAmount
        {
            get { return this.m_CouponAmount; }
            set { this.SetValue("CouponAmount", ref m_CouponAmount, value); }
        }

        public Decimal CashPay
        {
            get
            {
                decimal returnValue = (SOAmount.HasValue ? SOAmount.Value : 0) - Math.Abs((PointPayAmount.HasValue ? PointPayAmount.Value : 0)) - Math.Abs((CouponAmount.HasValue ? CouponAmount.Value : 0));
                return returnValue >= 0 ? returnValue : 0.00M;
            }
        }

        /// <summary>
        /// 订单总金额(>=0):SOAmount - PointPayAmount - CouponAmount + PayPrice + ShipPrice + PremiumAmount + TariffAmount - DiscountAmount
        /// </summary>
        public Decimal SOTotalAmount
        {
            get
            {
                decimal amount = (SOAmount.HasValue ? SOAmount.Value : 0)
                     - Math.Abs((PointPayAmount.HasValue ? PointPayAmount.Value : 0))
                     - Math.Abs((CouponAmount.HasValue ? CouponAmount.Value : 0))
                     + Math.Abs((PayPrice.HasValue ? PayPrice.Value : 0))
                     + Math.Abs((ShipPrice.HasValue ? ShipPrice.Value : 0))
                     + Math.Abs((PremiumAmount.HasValue ? PremiumAmount.Value : 0))
                     + Math.Abs((TariffAmount.HasValue ? TariffAmount.Value : 0))
                     - Math.Abs((PromotionAmount.HasValue ? PromotionAmount.Value : 0));
                return amount >= 0 ? amount : 0.00M;
            }
        }

        public Decimal ReceivableAmount
        {
            get
            {
                decimal amount = OriginalReceivableAmount;
                //泰隆银行  不抹掉零头
                //if (PayWhenReceived.HasValue && PayWhenReceived.Value)
                //{
                //    amount = (int)(amount * 10) / 10M;
                //}
                return amount >= 0 ? amount : 0.00M;
            }
        }

        public Decimal OriginalReceivableAmount
        {
            get
            {
                decimal amount = SOTotalAmount
                    - Math.Abs((PrepayAmount.HasValue ? PrepayAmount.Value : 0))
                    - Math.Abs((GiftCardPay.HasValue ? GiftCardPay.Value : 0));
                return amount >= 0 ? amount : 0.00M;
            }
        }

        private Int32? m_HoldUser;
        public Int32? HoldUser
        {
            get { return this.m_HoldUser; }
            set { this.SetValue("HoldUser", ref m_HoldUser, value); }
        }

        private SOHoldStatus? m_HoldStatus;
        public SOHoldStatus? HoldStatus
        {
            get { return this.m_HoldStatus; }
            set { this.SetValue("HoldStatus", ref m_HoldStatus, value); }
        }

        private String m_HoldReason;
        public String HoldReason
        {
            get { return this.m_HoldReason; }
            set { this.SetValue("HoldReason", ref m_HoldReason, value); }
        }

        private Int32? m_HoldMinutes;
        public Int32? HoldMinutes
        {
            get { return this.m_HoldMinutes; }
            set { this.SetValue("HoldMinutes", ref m_HoldMinutes, value); }
        }

        private DateTime? m_HoldTime;
        public DateTime? HoldTime
        {
            get { return this.m_HoldTime; }
            set { this.SetValue("HoldTime", ref m_HoldTime, value); }
        }


        /// <summary>
        /// 总毛利额
        /// </summary>
        private Decimal? m_GrossProfitSum;
        public Decimal? GrossProfitSum
        {
            get { return this.m_GrossProfitSum; }
            set { this.SetValue("GrossProfitSum", ref m_GrossProfitSum, value); }
        }

        /// <summary>
        /// 总毛利率
        /// </summary>
        private Decimal? m_GrossProfitRateSum;
        public Decimal? GrossProfitRateSum
        {
            get { return this.m_GrossProfitRateSum; }
            set { this.SetValue("GrossProfitRateSum", ref m_GrossProfitRateSum, value); }
        }

        /// <summary>
        /// 毛利率
        /// </summary>;
        public String GrossProfitRateSumDisplay
        {
            get
            {
                return GrossProfitRateSum.HasValue ? Math.Round(GrossProfitRateSum.Value, 2) + "%" : "0%";
            }
        }
        private string m_LanguageCode;
        public string LanguageCode
        {
            get { return this.m_LanguageCode; }
            set { this.SetValue("LanguageCode", ref m_LanguageCode, value); }
        }


        private Decimal? m_TariffAmount;
        /// <summary>
        /// 关税
        /// </summary>
        public Decimal? TariffAmount
        {
            get { return this.m_TariffAmount; }
            set { this.SetValue("TariffAmount", ref m_TariffAmount, value); }
        }

    }
}
