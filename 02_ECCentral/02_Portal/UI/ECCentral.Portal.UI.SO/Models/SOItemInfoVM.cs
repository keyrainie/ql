using System;
using System.Windows;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity.SO;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.SO.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.SO.Models
{
    public class SOItemInfoVM : ModelBase
    {
        private bool isChecked;
        public bool IsChecked
        {
            get { return this.isChecked; }
            set { this.SetValue("IsChecked", ref isChecked, value); }
        }
        private Int32? m_SysNo;
        public Int32? SysNo
        {
            get { return this.m_SysNo; }
            set { this.SetValue("SysNo", ref m_SysNo, value); }
        }

        private Int32? m_SOSysNo;
        public Int32? SOSysNo
        {
            get { return this.m_SOSysNo; }
            set { this.SetValue("SOSysNo", ref m_SOSysNo, value); }
        }

        private Int32? m_ProductSysNo;
        public Int32? ProductSysNo
        {
            get { return this.m_ProductSysNo; }
            set { this.SetValue("ProductSysNo", ref m_ProductSysNo, value); }
        }

        private String m_ProductID;
        public String ProductID
        {
            get { return this.m_ProductID; }
            set { this.SetValue("ProductID", ref m_ProductID, value); }
        }

        private String m_ProductName;
        public String ProductName
        {
            get { return this.m_ProductName; }
            set { this.SetValue("ProductName", ref m_ProductName, value); }
        }

        private SOProductType? m_ProductType;
        public SOProductType? ProductType
        {
            get { return this.m_ProductType; }
            set { this.SetValue("ProductType", ref m_ProductType, value); }
        }
        /// <summary>
        /// 优惠券
        /// </summary>
        public bool IsNotCoupon
        {
            get
            {
                return !(ProductType.HasValue && ProductType.Value == SOProductType.Coupon);
            }
        }
        /// <summary>
        /// 延保
        /// </summary>
        public bool IsExtendWarranty
        {
            get
            {
                return ProductType.HasValue && ProductType.Value == SOProductType.ExtendWarranty;
            }
        }
        /// <summary>
        /// 不是优惠券和延保
        /// </summary>
        public bool IsProduct
        {
            get
            {
                return IsNotCoupon && !IsExtendWarranty;
            }
        }

        public string SOProductTypeDispaly
        {
            get
            {
                string returnValue = string.Empty;
                switch (ProductType)
                {
                    case SOProductType.Product:
                        break;
                    case SOProductType.Gift:
                        returnValue = ResSOMaintain.Info_SOProductType_Gift;// "赠品"
                        break;
                    case SOProductType.Award:
                        returnValue = ResSOMaintain.Info_SOProductType_Gift;//(说明：枚举虽然是奖品  但是  前台对应的是赠品此处也就改为赠品了)
                        break;
                    case SOProductType.Coupon:
                        returnValue = ResSOMaintain.Info_SOProductType_Coupon;// "优惠券"
                        break;
                    case SOProductType.ExtendWarranty:
                        returnValue = ResSOMaintain.Info_SOProductType_ExtendWarranty;//"延保"
                        break;
                    case SOProductType.Accessory:
                        returnValue = ResSOMaintain.Info_SOProductType_Accessory;//"附件"
                        break;
                    case SOProductType.SelfGift:
                        returnValue = ResSOMaintain.Info_SOProductType_Gift;//(说明：枚举虽然是礼品  但是  前台对应的是赠品此处也就改为赠品了)
                        break;
                    default:
                        break;
                }
                return returnValue;
            }
        }

        /// <summary>
        /// 单据订购数量
        /// </summary>
        private Int32? m_Quantity;
        [Validate(ValidateType.Interger)]
        [Validate(ValidateType.Required)]
        public Int32? Quantity
        {
            get { return this.m_Quantity ?? 0; }
            set { this.SetValue("Quantity", ref m_Quantity, value); }
        }

        /// <summary>
        /// Online库存数量
        /// </summary>
        private Int32? m_OnlineQty;
        public Int32? OnlineQty
        {
            get { return this.m_OnlineQty ?? 0; }
            set { this.SetValue("OnlineQty", ref m_OnlineQty, value); }
        }

        public string OnlineQtyDescription
        {
            get
            {
                string result = string.Empty;
                if (m_OnlineQty.HasValue && m_ProductType != SOProductType.ExtendWarranty)
                {
                    result = m_OnlineQty.ToString();
                }
                return result;
            }
        }

        /// <summary>
        /// 可用库存数量
        /// </summary>
        private Int32? m_AvailableQty;
        public Int32? AvailableQty
        {
            get { return this.m_AvailableQty ?? 0; }
            set { this.SetValue("AvailableQty", ref m_AvailableQty, value); }
        }

        private Decimal? m_Price;
        public Decimal? Price
        {
            get { return this.m_Price ?? 0.00M; }
            set { this.SetValue("Price", ref m_Price, value); }
        }

        /// <summary>
        /// 除优惠券的促销活动折扣总额(&lt;=0) 
        /// </summary>
        private Decimal? m_PromotionAmount;
        public Decimal? PromotionAmount//对应IPP的：DiscountAmt
        {
            get { return this.m_PromotionAmount; }
            set { this.SetValue("PromotionAmount", ref m_PromotionAmount, value); }
        }

        /// <summary>
        /// 优惠券平均折扣到每个商品上的金额(&lt;=0)
        /// </summary>
        private Decimal? m_CouponAverageDiscount;
        public Decimal? CouponAverageDiscount
        {
            get { return this.m_CouponAverageDiscount; }
            set { this.SetValue("CouponAverageDiscount", ref m_CouponAverageDiscount, value); }
        }

        /// <summary>
        /// 优惠券折扣总额(== CouponAverageDiscount * Quantity)，注意：在使用时请不要与数量(Quantity)相乘，此结果已经是此种商品的优惠券总折扣；
        /// 如果要用单个商品的优惠券折扣，请用CouponAverageDiscount
        /// </summary>
        public Decimal? CouponAmount
        {
            get
            {
                return CouponAverageDiscount * Quantity;
            }
        }


        private Decimal? m_OriginalPrice;
        public Decimal? OriginalPrice
        {
            get { return this.m_OriginalPrice ?? 0.00M; }
            set { this.SetValue("OriginalPrice", ref m_OriginalPrice, value); }
        }

        private Decimal? m_CostPrice;
        public Decimal? CostPrice
        {
            get { return this.m_CostPrice; }
            set { this.SetValue("CostPrice", ref m_CostPrice, value); }
        }

        private Decimal? m_NoTaxCostPrice;
        public Decimal? NoTaxCostPrice
        {
            get { return this.m_NoTaxCostPrice; }
            set { this.SetValue("NoTaxCostPrice", ref m_NoTaxCostPrice, value); }
        }

        private SOProductPriceType m_PriceType;
        /// <summary>
        /// 价格类型
        /// </summary>
        public SOProductPriceType PriceType
        {
            get { return this.m_PriceType; }
            set { this.SetValue("PriceType", ref m_PriceType, value); }
        }

        private Int32? m_GainAveragePoint;
        public Int32? GainAveragePoint
        {
            get { return this.m_GainAveragePoint ?? 0; }
            set { this.SetValue("GainAveragePoint", ref m_GainAveragePoint, value); }
        }

        /// <summary>
        /// 单种商品可获得积分总计
        /// </summary>
        public Int32? GainAveragePointSum
        {
            get
            {
                return GainAveragePoint * Quantity;
            }
        }

        /// <summary>
        /// 毛利额
        /// </summary>
        private Decimal? m_GrossProfit;
        public Decimal? GrossProfit
        {
            get { return this.m_GrossProfit; }
            set { this.SetValue("GrossProfit", ref m_GrossProfit, value); }
        }

        /// <summary>
        /// 毛利率
        /// </summary>
        private Decimal? m_GrossProfitRate;
        public Decimal? GrossProfitRate
        {
            get { return this.m_GrossProfitRate ?? 0; }
            set { this.SetValue("GrossProfitRate", ref m_GrossProfitRate, value); }
        }

        /// <summary>
        /// 毛利率
        /// </summary>;
        public String GrossProfitRateDisplay
        {
            get
            {
                return Math.Round(GrossProfitRate.Value, 2) + "%";
            }
        }

        private ProductPayType? m_PayType;
        public ProductPayType? PayType
        {
            get { return this.m_PayType; }
            set { this.SetValue("PayType", ref m_PayType, value); }
        }

        private String m_Warranty;
        public String Warranty
        {
            get { return string.IsNullOrEmpty(this.m_Warranty) ? ResSOMaintain.Info_SOMaintain_Warranty_HaveNot : this.m_Warranty; }
            set { this.SetValue("Warranty", ref m_Warranty, value); }
        }

        private Decimal? m_Weight;
        public Decimal? Weight
        {
            get { return this.m_Weight ?? 0M; }
            set { this.SetValue("Weight", ref m_Weight, value); }
        }

        private Int32? m_StockSysNo;
        public Int32? StockSysNo
        {
            get { return this.m_StockSysNo; }
            set { this.SetValue("StockSysNo", ref m_StockSysNo, value); }
        }

        private String m_StockName;
        public String StockName
        {
            get { return this.m_StockName; }
            set { this.SetValue("StockName", ref m_StockName, value); }
        }

        public String IsShippedOutDispaly
        {
            get
            {
                string result = string.Empty;
                if (this.ProductType == SOProductType.Product
                    && (!IsShippedOut.HasValue || IsShippedOut.Value == false))
                {
                    return "未出库";
                }
                return result;
            }
        }
        public String StockInfoDispaly
        {
            get
            {
                if (IsShippedOut.HasValue && IsShippedOut.Value == true)
                {
                    return "[" + ShippedOutTime + "]";
                }
                else
                {
                    return "";
                }
            }
        }

        private String m_SHDSysNo;
        public String SHDSysNo
        {
            get { return this.m_SHDSysNo; }
            set { this.SetValue("SHDSysNo", ref m_SHDSysNo, value); }
        }

        private String m_MasterProductSysNo;
        public String MasterProductSysNo
        {
            get { return this.m_MasterProductSysNo; }
            set { this.SetValue("MasterProductSysNo", ref m_MasterProductSysNo, value); }
        }

        /// <summary>
        /// 主商品 对应的 赠送赠品基数 （如 基数为2  则表示一个主商品 赠送2个赠品 一次类推）
        /// </summary>
        private Int32? m_RuleQty;
        public Int32? RuleQty
        {
            get { return this.m_RuleQty; }
            set { this.SetValue("RuleQty", ref m_RuleQty, value); }
        }

        private Boolean? m_IsShippedOut;
        public Boolean? IsShippedOut
        {
            get { return this.m_IsShippedOut ?? false; }
            set { this.SetValue("IsShippedOut", ref m_IsShippedOut, value); }
        }

        private DateTime? m_ShippedOutTime;
        public DateTime? ShippedOutTime
        {
            get { return this.m_ShippedOutTime; }
            set { this.SetValue("ShippedOutTime", ref m_ShippedOutTime, value); }
        }

        //赠品数量
        int? m_gainQuantity;
        public int? GainQuantity
        {
            get { return this.m_gainQuantity; }
            set { this.SetValue("GainQuantity", ref m_gainQuantity, value); }
        }

        ProductInventoryType m_InventoryType;
        public ProductInventoryType InventoryType
        {
            get { return this.m_InventoryType; }
            set { this.SetValue("InventoryType", ref m_InventoryType, value); }
        }
        public string InventoryTypeDisplay
        {
            get
            {
                //string returnValue = string.Empty;
                return EnumConverter.GetDescription(InventoryType);
                //switch (InventoryType)
                //{
                //    case ProductInventoryType.Normal:
                //        returnValue = "自营";
                //        break;
                //    case ProductInventoryType.GetShopInventory:
                //        returnValue = "柜台门店";
                //        break;
                //    case ProductInventoryType.GetShopUnInventory:
                //        returnValue = "柜台非门店";
                //        break;
                //    case ProductInventoryType.Company:
                //        returnValue = "总部家电";
                //        break;
                //    case ProductInventoryType.Factory:
                //        returnValue = "厂家送货";
                //        break;
                //    case ProductInventoryType.TwoDoor:
                //        returnValue = "双开门";
                //        break;
                //    case ProductInventoryType.Merchent:
                //        returnValue = "租赁商品";
                //        break;
                //    default:
                //        break;
                //}
                //return returnValue;
            }
        }
        #region 价格补偿 所需属性

        Decimal? m_price_End;
        /// <summary>
        /// 价格补偿后的商品销售单价
        /// </summary>
        public Decimal? Price_End
        {
            get { return this.m_price_End; }
            set { this.SetValue("Price_End", ref m_price_End, value); }
        }

        /// <summary>
        /// 价格补偿 数量（既：价格将要减掉 的差额）
        /// </summary>
        private Decimal? m_AdjustPrice;
        public Decimal? AdjustPrice
        {
            get { return this.m_AdjustPrice ?? 0; }
            set { this.SetValue("AdjustPrice", ref m_AdjustPrice, value); }
        }

        /// <summary>
        /// 价格补偿 原因
        /// </summary>
        private String m_AdjustPriceReason;
        public String AdjustPriceReason
        {
            get { return this.m_AdjustPriceReason; }
            set { this.SetValue("AdjustPriceReason", ref m_AdjustPriceReason, value); }
        }

        /// <summary>
        /// 单种商品 合计价格（数量 乘以 单价）
        /// </summary>
        public Decimal? ProductPriceSum
        {
            get
            {
                return OriginalPrice * Quantity;
            }
        }

        public decimal TruePrice
        {
            set;
            get;
        }

        #endregion

        #region 积分补偿 所需属性

        /// <summary>
        /// 积分补偿 数量（既客户为收货时 商品降价  ，泰隆优选以积分的形式补偿给客户）
        /// </summary>
        private Int32? m_CompensationPoint;
        public Int32? CompensationPoint
        {
            get { return this.m_CompensationPoint ?? 0; }
            set { this.SetValue("CompensationPoint", ref m_CompensationPoint, value); }
        }

        /// <summary>
        /// 积分补偿 备注
        /// </summary>
        private String m_CompensationPointMemo;
        public String CompensationPointMemo
        {
            get { return this.m_CompensationPointMemo; }
            set { this.SetValue("CompensationPointMemo", ref m_CompensationPointMemo, value); }
        }

        #endregion

        #region 团购商品相关信息
        SettlementStatus? m_SettlementStatus;
        /// <summary>
        /// 团购处理状态（后台使用）
        /// </summary>
        public SettlementStatus? SettlementStatus
        {
            get { return this.m_SettlementStatus; }
            set { this.SetValue("SettlementStatus", ref m_SettlementStatus, value); }
        }

        int? m_ReferenceSysNo;
        /// <summary>
        /// 团购的编号
        /// </summary>
        public int? ReferenceSysNo
        {
            get { return this.m_ReferenceSysNo; }
            set { this.SetValue("ReferenceSysNo", ref m_ReferenceSysNo, value); }
        }

        public string SOProductGroupBuyDisplay
        {
            get
            {
                string returnValue = string.Empty;
                if (ReferenceSysNo.HasValue)
                {
                    switch (SettlementStatus)
                    {
                        case ECCentral.BizEntity.SO.SettlementStatus.Fail:
                            returnValue = ResSOMaintain.Info_SOProductGroupBuyStatus_Fail;
                            break;
                        case ECCentral.BizEntity.SO.SettlementStatus.PlanFail:
                            returnValue = ResSOMaintain.Info_SOProductGroupBuyStatus_PlanFail;
                            break;
                        case ECCentral.BizEntity.SO.SettlementStatus.Success:
                            returnValue = "";
                            break;
                        default:
                            returnValue = ResSOMaintain.Info_SOProductGroupBuyStatus_Default;
                            break;
                    }
                }
                return returnValue;
            }
        }

        #endregion

        bool m_isLessStock;
        public bool IsLessStock
        {
            get { return this.m_isLessStock; }
            set { this.SetValue("IsLessStock", ref m_isLessStock, value); }
        }

        bool m_isWaitPO;
        public bool IsWaitPO
        {
            get { return this.m_isWaitPO; }
            set { this.SetValue("IsWaitPO", ref m_isWaitPO, value); }
        }

        public SOVM SOVM
        {
            get;
            set;
        }

        public bool IsEditItem
        {
            get
            {
                bool result = true;
                if (SOVM != null)
                {
                    result = (SOVM.BaseInfoVM.Status == SOStatus.Origin || !SOVM.BaseInfoVM.Status.HasValue);
                }
                return result;
            }
        }

        public Visibility VisibilityUpdateQty
        {
            get
            {
                Visibility result = Visibility.Collapsed;
                if (this.ProductType == SOProductType.Product
                    && this.IsEditItem
                    && SOVM.BaseInfoVM.SplitType != SOSplitType.SubSO
                    && SOVM.SysNo.GetValueOrDefault() <= 0 ) //改单不能修改订单中的商品
                {
                    result = Visibility.Visible;
                }
                return result;
            }
        }

        public Visibility VisibilityNoUpdateQty
        {
            get
            {
                return VisibilityUpdateQty == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        Decimal? m_TariffAmount;
        /// <summary>
        /// 关税
        /// </summary>
        public Decimal? TariffAmount
        {
            get { return this.m_TariffAmount; }
            set { this.SetValue("TariffAmount", ref m_TariffAmount, value); }
        }

        /// <summary>
        /// 存储运输方式
        /// </summary>
        public StoreType? StoreType { get; set; }

        /// <summary>
        /// 显示的优惠券抵扣金额
        /// </summary>
        private Decimal? m_ShowCouponDiscount;
        public Decimal? ShowCouponDiscount
        {
            get { return this.m_ShowCouponDiscount; }
            set { this.SetValue("ShowCouponDiscount", ref m_ShowCouponDiscount, value); }
        }

        Decimal? m_TariffRate;
        /// <summary>
        /// 税率
        /// </summary>
        public Decimal? TariffRate
        {
            get { return this.m_TariffRate; }
            set { this.SetValue("TariffRate", ref m_TariffRate, value); }
        }
    }
}
